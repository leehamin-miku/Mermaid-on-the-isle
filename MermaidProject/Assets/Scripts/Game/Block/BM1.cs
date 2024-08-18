using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Data;


public class BM1 : Block
{
    //저장 대상
    public int waitingLen1 = 0; //마스터만 관리
    public int waitingLen2 = 0; //마스터만 관리
    //저장 대상
    public float a = 0;
    bool isCool = false;

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&isCool==false && collision.collider.GetComponent<Block>().BlockCode == 7 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            StartCoroutine(CoolDown(0.5f));
            waitingLen1++;
        } else if (PhotonNetwork.IsMasterClient&&isCool == false && collision.collider.GetComponent<Block>().BlockCode == 3 && collision.collider.GetComponent<Block>().isGrabed)
        {

            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            StartCoroutine(CoolDown(0.5f));
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
    public override SaveBlockStruct DeepCopySub(SaveBlockStruct block)
    {
        block.w1 = waitingLen1;
        block.w2 = waitingLen2;
        block.a = a;
        return block;
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
    public IEnumerator CoolDown(float a)
    {
        isCool = true;
        float b = 0;
        while (b <= a)
        {
            b += Time.deltaTime;
            yield return null;
        }
        isCool = false;
    }
}
