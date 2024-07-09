using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Hammer : Block
{
    public bool isBuilding;
    public Blueprint blueprint;
    //플레이어의 공기저항과 관련되어 있으니 꼭 확인해야함

    public override void UseDownAction()
    {
        base.UseUpAction();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Blueprint"));

        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Blueprint>().CheckIsAbleBuild())
            {
                hit.collider.GetComponent<Blueprint>().isAbleGrabed = false;
                isBuilding = true;
                hit.collider.GetComponent<Blueprint>().PV.RPC("PVFucIsBuilding", RpcTarget.All);
                StartCoroutine(WaitBuilding());
                //n초 기달리기
            }
        }
    }
    public override void UseUpAction()
    {
        base.UseUpAction();
        StopBuilding();

    }
    public void StopBuilding()
    {
        isBuilding = false;
        Debug.Log("해머 정지");
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
            StopBuilding();
            GetComponent<Collider2D>().enabled = true;
            p1.GetComponent<FixedJoint2D>().connectedBody = null;
            transform.position = p1.transform.position;
            p1.GetComponent<FixedJoint2D>().connectedBody = rb;
        }
    }

    IEnumerator WaitBuilding()
    {
        Debug.Log("만들기 시작");
        float a = 0f;
        float b = 3f;
        while (a <= b && isBuilding)
        {
            a += Time.deltaTime;
            yield return null;
        }

        if (isBuilding)
        {
            Debug.Log("완성");
            StopBuilding();
        }

    }
}
