using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BM1 : Block
{
    //저장 대상
    public int waitingLen1 = 0; //마스터만 관리
    public int waitingLen2 = 0; //마스터만 관리
    //저장 대상
    public float a = 0;

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 7 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen1++;
        } else if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 3 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen2++;
        }
    }

    public override IEnumerator RunningCoroutine()
    {
        while (true)
        {
            if (waitingLen1 > 0&& waitingLen2 > 0)
            {
                a += Time.deltaTime;
                transform.GetChild(0).Rotate(new Vector3(0, 0, Time.deltaTime * 200));
            }

            if (a >= 10)
            {
                a -= 10f;
                waitingLen1--;
                waitingLen2--;
                PhotonNetwork.Instantiate("Prefab/Game/Brick1", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
            }
            yield return null;
        }
    }
    public override Block DeepCopySub(Block block)
    {
        (block as BM1).a = a;
        (block as BM1).waitingLen1 = waitingLen1;
        (block as BM1).waitingLen2 = waitingLen2;
        return block;
    }

}
