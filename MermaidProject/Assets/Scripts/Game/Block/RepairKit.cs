using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class RepairKit: Block
{
    //저장 대상
    public int a = 7;
    public void Update()
    {
        if (p1!=null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Block"));
            if (hit.collider != null)
            {
                transform.GetChild(0).GetComponent<TextMeshPro>().text = "잔여사용횟수 " + a + "\n"
                    + hit.collider.GetComponent<Block>().strength + "/" + hit.collider.GetComponent<Block>().maxStrenth;



            }
            else
            {
                transform.GetChild(0).GetComponent<TextMeshPro>().text = "잔여사용횟수 " + a;
            }
        }
    }
    //플레이어의 공기저항과 관련되어 있으니 꼭 확인해야함


    public override void UseDownAction()
    {
        base.UseDownAction();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Block"));
        if (hit.collider != null)
        {
            if(hit.collider.GetComponent<Block>().strength < hit.collider.GetComponent<Block>().maxStrenth)
            {
                hit.collider.GetComponent<Block>().PV.RPC("ChangeStrength", RpcTarget.All, 5);
                PV.RPC("UseCountChange", RpcTarget.All, -1);
            }
        }
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
            transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
        }
    }

    [PunRPC]
    void UseCountChange(int b)
    {
        a += b;
        if (a <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    public override Block DeepCopySub(Block block)
    {
        RepairKit rk = new RepairKit();
        rk.savePosition = block.savePosition;
        rk.saveRotation = block.saveRotation;
        rk.a = a;
        return rk;
    }

}
