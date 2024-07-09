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
            if (go.GetComponent<Block>().p1 != null)
            {
                a = false;
                break;
            }
        }

        list.Sort();

        //if ( && a && p1 == null)
        //{
        //    foreach (GameObject go in haveBlock)
        //    {
        //        go.transform.position = new Vector2(999, 999);
        //        Destroy(go, 0.1f);
        //    }
        //    Instantiate(structure, transform.position, transform.rotation);
        //    Destroy(this.gameObject);
        //}
        return list.SequenceEqual(needs) && a && p1 == null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        haveBlock.Add(collision.gameObject);
        // 트리거에 들어온 콜라이더를 리스트에 추가
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        haveBlock.Remove(collision.gameObject);
    }



}
