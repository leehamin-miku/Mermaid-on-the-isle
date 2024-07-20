using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BreakwaterMaker2 : Block
{
    public int waitingLen = 0; //마스터만 관리
    float a = 0;

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&collision.collider.GetComponent<Block>().BlockCode == 1 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            waitingLen++;
            if (waitingLen <= 1)
            {
                a = 0;
                StartCoroutine(MakeBrick());
            }
        }
    }

    IEnumerator MakeBrick()
    {
        while (a <= 10f&&isRunning)
        {
            transform.GetChild(0).Rotate(new Vector3(0, 0,Time.deltaTime * 200));
            a += Time.deltaTime;
            yield return null;
        }
        waitingLen--;
        PhotonNetwork.Instantiate("Prefab/Game/Brick2", transform.position - transform.up, transform.rotation);
        if (waitingLen > 0)
        {
            StartCoroutine(MakeBrick());
        }
    }

}
