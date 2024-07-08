using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviourPunCallbacks
{

    public bool isReady;
    public float coolTime = 0f;
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
    public virtual void UseItem()
    {
    }
    public void StartCoolDown()
    {
        StartCoroutine(CoolDown());
    }


    IEnumerator CoolDown()
    {
        isReady = false;
        yield return new WaitForSeconds(coolTime);
        isReady = true;
    }

    public void Grabed(PlayerController p1)
    {

        if (this.p1 == null)
        {
            rb.mass = 0;
            rb.drag = 0;
            rb.angularDrag = 0;
            this.p1 = p1;
            PV.RPC("IsMineDelete", RpcTarget.Others);
            PV.IsMine = true;
            GrabedAction();
        }
        else
        {
            GrabedAction();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            this.p1 = null;
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
    public void IsMineDelete()
    {
        PV.IsMine = false;
    }

    
}
