using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data;
using static Unity.Burst.Intrinsics.X86;

public class Cannon : Block
{

    AudioSource soundEffect;
    float a = 0;
    bool isShooting = false;
    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.collider.GetComponent<Block>().BlockCode == 7 && collision.collider.GetComponent<Block>().isGrabed&&isShooting==false)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            isShooting=true;
        }
    }

    public override IEnumerator RunningCoroutine()
    {
        soundEffect = GetComponent<AudioSource>();
        while (true)
        {
            if (isShooting)
            {
                a += Time.deltaTime;
            }

            if (a >= 5)
            {
                a = 0;
                isShooting = false;
                GameObject go = PhotonNetwork.Instantiate("Prefab/Game/CannonBall", transform.position + transform.up*2, transform.rotation);
                go.GetComponent<CannonBall>().Shot();
                GetComponent<Rigidbody2D>().AddForce(-transform.up*40, ForceMode2D.Impulse);
                soundEffect.Play();
            }
            yield return null;
        }
    }
    public override SaveBlockStruct DeepCopySub(SaveBlockStruct block)
    {
        block.a = a;
        return block;
    }
}
