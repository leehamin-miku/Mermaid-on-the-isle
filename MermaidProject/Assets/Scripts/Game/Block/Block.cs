using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviourPunCallbacks
{
    public bool isRunning = true;
    public bool isInUse = false;
    public bool isGrabed = false;
    public int BlockCode;
    public Rigidbody2D rb;
    public float strength;
    public float mass;
    public float drag;
    public float angularDrag;
    public PlayerController p1 = null;
    public bool isAbleGrabed;
    public PhotonView PV;
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }

    public virtual void GrabedAction()
    {
        //경축 아무것도 안함
    }
    public virtual void UseDownAction()
    {
        isInUse = true;
    }
    public virtual void UseUpAction()
    {
        isInUse = false;
    }


    public void Grabed(PlayerController p1)
    {

        if (isGrabed==false)
        {
            PV.RPC("IsMineDelete", RpcTarget.Others);
            PV.IsMine = true;
            PV.RPC("PVFucIsGrabed", RpcTarget.All);
            this.p1 = p1;
            rb.mass = 0.1f;
            rb.drag = 0;
            rb.angularDrag = 0;
            GrabedAction();
        }
        else
        {
            if (isInUse)
            {
                UseUpAction();
            }
            GrabedAction();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            this.p1 = null;
            PV.RPC("PVFucIsNotGrabed", RpcTarget.All);

            PV.IsMine = false;
            PV.RPC("GiveMineToMaster", RpcTarget.MasterClient);
        }
    }

    private void OnDestroy()
    {
        DestroyAction();
        if (p1 != null)
        {
            p1.ToggleAction();
        }
    }

    public virtual void DestroyAction()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEnterAction(collision);
    }
    public virtual void CollisionEnterAction(Collision2D collision)
    {

    }

    [PunRPC]
    public void GiveOwner()
    {
        PV.RequestOwnership();
    }

    [PunRPC]
    public void PVFucIsGrabed()
    {
        isGrabed = true;
        
    }

    [PunRPC]
    public void PVFucIsNotGrabed()
    {
        isGrabed = false;
        
    }
    [PunRPC]
    public void IsMineDelete()
    {
        PV.IsMine = false;
    }
    [PunRPC]
    public void GiveMineToMaster()
    {
        PV.IsMine = true;
    }
    [PunRPC]
    public void DestroyFuc()
    {
        if (PV.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
