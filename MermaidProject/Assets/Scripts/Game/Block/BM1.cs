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

    AudioSource soundEffect;
    public override IEnumerator RunningCoroutine()
    {
        soundEffect = GetComponent<AudioSource>();
        bool isWorking = false;
        
        while (true)
        {
            if (waitingLen1 > 0&& waitingLen2 > 0)
            {
                if(!isWorking)
                {
                    StartCoroutine(startSound());
                    isWorking = true;
                }
                a += Time.deltaTime;
                transform.GetChild(0).Rotate(new Vector3(0, 0, Time.deltaTime * 200));
            }

            if (a >= 15)
            {
                a -= 15f;
                waitingLen1--;
                waitingLen2--;
                PhotonNetwork.Instantiate("Prefab/Game/Brick1", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                if (isWorking)
                {
                    StartCoroutine(EndSound());
                    isWorking = false;
                }
            }
            yield return null;
        }
    }
    public override Block DeepCopySub(Block block)
    {
        BM1 bm1 = new BM1();
        bm1.a = a;
        bm1.waitingLen1 = waitingLen1;
        bm1.waitingLen2 = waitingLen2;
        bm1.BlockCode = BlockCode;
        bm1.strength = block.strength;
        bm1.savePosition = block.savePosition;
        bm1.saveRotation = block.saveRotation;
        return bm1;
    }

    private IEnumerator startSound()
    {
        float timer = 0;
        soundEffect.volume = 0;
        soundEffect.Play();
        while (timer <= 1)
        {
            timer += Time.deltaTime / 1.5f;
            soundEffect.volume = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        yield return null;
    }
    private IEnumerator EndSound()
    {
        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime / 1.5f;
            soundEffect.volume = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        soundEffect.Stop();
        yield return null;
    }
}
