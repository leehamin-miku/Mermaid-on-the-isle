using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviourPunCallbacks
{
    public bool isInUse = false;
    public bool isGrabed = false;
    public int BlockCode;
    public Rigidbody2D rb;
    public Vector3 savePosition;
    public Quaternion saveRotation;

    //저장 대상
    public int strength;

    public int maxStrenth;
    public float mass;
    public float drag;
    public float angularDrag;
    public PlayerController p1 = null;
    public bool isAbleGrabed;
    public PhotonView PV;

    Coroutine runningCoroutine;
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
    //master에서 실행하는 코루틴!! 모든 함수가 시작 할때 다음 코루틴을 실행함
    public virtual IEnumerator RunningCoroutine()
    {
        yield return null;
    }

    //마스터에서 실행됨
    //러닝/비러닝 구분이 필요없는 아이템은 그냥 update를 사용할것
    [PunRPC]
    public void StartObject()
    {
        runningCoroutine = StartCoroutine(RunningCoroutine());
    }
    public void StopObject()
    {
        if(runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
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
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyAction();
            if (p1 != null)
            {
                p1.GetComponent<FixedJoint2D>().connectedBody = null;
                p1.GetComponent<FixedJoint2D>().enabled = false;
            }
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
    [PunRPC]
    public void ChangeStrength(int a)
    {
        
        strength += a;

        if(strength > maxStrenth)
        {
            strength = maxStrenth;
        }
        
        if (PV.IsMine)
        {
            if (strength <= 0)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    public Block DeepCopy()
    {
        Block block = new Block();
        block.strength = strength;
        block.savePosition = transform.position;
        block.saveRotation = transform.rotation;
        return DeepCopySub(block);
    }
    public virtual Block DeepCopySub(Block block)
    {
        return block;
    }

}
