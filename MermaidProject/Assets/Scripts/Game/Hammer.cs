using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Hammer : Block
{
    public bool isBuilding;
    public Blueprint blueprint;
    [SerializeField] GameObject cartoonPrefab;
    GameObject cartoon;
    //�÷��̾��� �������װ� ���õǾ� ������ �� Ȯ���ؾ���

    public override void UseDownAction()
    {
        base.UseDownAction();
        if (isBuilding)
        {
            StopBuilding();
        } else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Blueprint"));
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Blueprint>().CheckIsAbleBuild())
                {
                    blueprint = hit.collider.GetComponent<Blueprint>();
                    blueprint.PV.RPC("PVFucIsBuilding", RpcTarget.All);
                    StartCoroutine(WaitBuilding());
                    //n�� ��޸���
                }
            }
        }
        
    }
    public override void UseUpAction()
    {
        base.UseUpAction();
        

    }
    public void StopBuilding()
    {
        PhotonNetwork.Destroy(cartoon);
        isBuilding = false;
        blueprint.PV.RPC("PVFucIsNotBuilding", RpcTarget.All);
        p1.isAbleMove = true;
        Debug.Log("�ظ� ����");
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
    IEnumerator WaitBuilding()
    {
        Debug.Log("����� ����");
        isBuilding = true;
        p1.isAbleMove = false;
        p1.rb.angularVelocity = 0;
        p1.rb.velocity = Vector2.zero;
        cartoon = PhotonNetwork.Instantiate("Prefab/Game/Cartoon", new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), Quaternion.identity);
        

        float a = 0f;
        float b = 3f;
        cartoon.GetComponent<Cartoon>().b = b;

        while (a <= b && isBuilding)
        {
            a += Time.deltaTime;
            yield return null;
        }

        if (isBuilding)
        {
            Debug.Log("�ϼ�");
            StopBuilding();
        }

    }

}
