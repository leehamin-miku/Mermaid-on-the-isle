using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BM2 : Block
{
    //저장 대상
    public int waitingLen1 = 0; //마스터만 관리
    public int waitingLen2 = 0; //마스터만 관리
    //저장 대상
    public float a = 0;

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 1 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen1++;
        }
        if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 1 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen1++;
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
                PhotonNetwork.Instantiate("Prefab/Game/Brick2", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
            }
            yield return null;
        }
    }

    public override Block DeepCopySub(Block block)
    {
        BM2 bm2 = new BM2();
        bm2.a = a;
        bm2.waitingLen1 = waitingLen1;
        bm2.waitingLen2 = waitingLen2;
        bm2.BlockCode = BlockCode;
        bm2.strength = block.strength;
        bm2.savePosition = block.savePosition;
        bm2.saveRotation = block.saveRotation;
        return bm2;
    }

}
