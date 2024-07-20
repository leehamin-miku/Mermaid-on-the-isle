using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Sword : Block
{
    //플레이어의 공기저항과 관련되어 있으니 꼭 확인해야함
    int Durability = 3;
    bool isReady = true;
    public override void UseDownAction()
    {
        base.UseDownAction();
        if (isReady)
        {
            isReady = false;
            PV.RPC("StartSwing", RpcTarget.All);
            StartCoroutine(CoolTime(0.5f));
            RaycastHit2D hit = Physics2D.CircleCast(transform.position + p1.transform.up, 1.5f, p1.transform.up, 0.1f, LayerMask.GetMask("Block"));
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<TsunamiUnit>() != null)
                {
                    
                    hit.collider.GetComponent<TsunamiUnit>().PV.RPC("ChangeHP", RpcTarget.MasterClient, -1);
                    hit.collider.GetComponent<TsunamiUnit>().PV.RPC("AddRB", RpcTarget.MasterClient, (hit.collider.transform.position-p1.transform.position).normalized);
                    PV.RPC("DurabilityChange", RpcTarget.All, -1);
                }
            }
        }
    }
    public override void UseUpAction()
    {
        base.UseUpAction();
        

    }
    public override void GrabedAction()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            GetComponent<Collider2D>().enabled = false;

            p1.GetComponent<FixedJoint2D>().enabled = false;

            rb.rotation = p1.rb.rotation;
            transform.position = p1.transform.position+p1.transform.up;

            p1.GetComponent<FixedJoint2D>().enabled = true;

        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            p1.GetComponent<FixedJoint2D>().connectedBody = null;
            transform.position = p1.transform.position;
            p1.GetComponent<FixedJoint2D>().connectedBody = rb;
        }
    }


    //임시 코루틴
    //이거 멀티 불가능함
    //아닌가 되나
    [PunRPC]
    void StartSwing()
    {
        StartCoroutine(Swing());
    }
    IEnumerator Swing()
    {
        float a = 0;
        float b = 0.5f;
        while (a < b)
        {
            a += Time.deltaTime;
            transform.GetChild(0).transform.localRotation = new Quaternion(0, 0, a, 0);
            yield return null;
        }
        transform.GetChild(0).transform.localRotation = new Quaternion(0, 0, 0, 0);
    }
    IEnumerator CoolTime(float a)
    {
        
        yield return new WaitForSeconds(a);
        isReady = true;
    }

    [PunRPC]
    public void DurabilityChange(int a)
    {
        Durability += a;
        if (PV.IsMine&&a<=0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
