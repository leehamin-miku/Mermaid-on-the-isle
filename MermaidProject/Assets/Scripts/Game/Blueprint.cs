using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.VersionControl;
using UnityEngine;

public class Blueprint : Block
{
    public List<int> needs;
    public List<GameObject> haveBlock = new List<GameObject>();
    public GameObject structure;
    public Hammer hammer;
    public bool isBuilding;
    public void BuildAction()
    {

    }


    public bool CheckIsAbleBuild()
    {
        bool a = true;
        List<int> list = new List<int>();
        foreach (GameObject go in haveBlock)
        {
            list.Add(go.GetComponent<Block>().BlockCode);
        }

        list.Sort();

        return list.SequenceEqual(needs) && a && isGrabed==false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        haveBlock.Add(collision.gameObject);
        if(hammer != null)
        {
            hammer.StopBuilding();
            PV.RPC("PVFucIsNotBuilding", RpcTarget.All);
            hammer = null;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        haveBlock.Remove(collision.gameObject);
        if (hammer != null)
        {
            hammer.StopBuilding();
            PV.RPC("PVFucIsNotBuilding", RpcTarget.All);
            hammer = null;
        }
    }


    //이것이 제작중이라고 모든플레이어에게 선언하는 함수
    [PunRPC]
    public void PVFucIsBuilding()
    {
        isBuilding = true;
    }
    [PunRPC]
    public void PVFucIsNotBuilding()
    {
        isBuilding = false;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
}
