using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blueprint : Block
{
    public List<int> needs;
    public List<GameObject> haveBlock = new List<GameObject>();
    public string structure;

    public override IEnumerator RunningCoroutine()
    {
        while(true){
            if (CheckIsAbleBuild())
            {
                int a = haveBlock.Count;
                for (int i = 0; i < a; i++)
                {
                    PhotonNetwork.Destroy(haveBlock[0]);
                }
                PhotonNetwork.Instantiate("Prefab/Game/" + structure, transform.position, transform.rotation).GetComponent<Block>().StartObject();
                PhotonNetwork.Destroy(this.gameObject);
            }
            yield return null;
        }
    }
    bool CheckIsAbleBuild()
    {
        
        if(isAbleGrabed == false)
        {
            return false;
        }

        List<int> list = new List<int>();
        foreach (GameObject go in haveBlock)
        {
            if (go.GetComponent<Block>().isGrabed)
            {
                return false;
            }
            list.Add(go.GetComponent<Block>().BlockCode);
        }

        list.Sort();
        return list.SequenceEqual(needs) && isGrabed==false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        haveBlock.Add(collision.gameObject);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        haveBlock.Remove(collision.gameObject);
    }
}
