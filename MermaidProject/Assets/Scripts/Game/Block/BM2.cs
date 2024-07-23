using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BM2 : Block
{
    //저장 대상
    public int waitingLen = 0; //마스터만 관리
    //저장 대상
    public float a = 0;

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 1 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen++;
        }
    }

    public override IEnumerator RunningCoroutine()
    {
        while (true)
        {
            if (waitingLen > 0)
            {
                a += Time.deltaTime;
                transform.GetChild(0).Rotate(new Vector3(0, 0, Time.deltaTime * 200));
            }

            if (a >= 10)
            {
                a -= 10f;
                waitingLen--;
                PhotonNetwork.Instantiate("Prefab/Game/Brick2", transform.position - transform.up, transform.rotation);
            }
            yield return null;
        }
    }

}
