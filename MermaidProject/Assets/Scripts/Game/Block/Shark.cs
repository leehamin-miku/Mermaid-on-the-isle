using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : TsunamiUnit
{
    private GameObject ClosestPlayer;
    private Vector3 SharkPosition;
    private Vector3 FlowerPosition;
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        StartCoroutine(SharkAi());

        //샤크 코루틴 시작
        //인공지능 구현해야함
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.collider.GetComponent<Block>() != null && collision.collider.GetComponent<Block>().BlockCode != 0 && collision.collider.GetComponent<TsunamiUnit>() == null)
            {
                collision.collider.GetComponent<Block>().PV.RPC("ChangeStrength", Photon.Pun.RpcTarget.All, -2);
                ChangeHP(-1);
            }
            else if (collision.collider.GetComponent<CannonBall>() != null)
            {
                ChangeHP(-20);
            }
        }
        
    }
    
    AudioSource soundEffect;

    private IEnumerator SharkAi()
    {
        soundEffect = GetComponent<AudioSource>();
        Vector3 ClosestPlayerPosition;
        FlowerPosition = GameObject.Find("Island").transform.position;
        float DetectDistance = 5f;
        float Power = 13f;
        while (true)
        {
            StartCoroutine(FindClosestPlayerPosition());
            ClosestPlayerPosition = ClosestPlayer.transform.position;
            if (Vector3.Distance(ClosestPlayerPosition,SharkPosition) < DetectDistance)
            {
                rb.AddForce((ClosestPlayerPosition - SharkPosition).normalized * Power, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce((FlowerPosition - SharkPosition).normalized * Power, ForceMode2D.Impulse);
            }
            soundEffect.Play();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }        
    }

    private IEnumerator FindClosestPlayerPosition()
    {
        SharkPosition = transform.position;
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        float ClosestDistance = 99999f;

        foreach (GameObject p in Players)
        {
            Vector3 PlayerPosition = p.transform.position;
            float distance = Vector3.Distance(SharkPosition, PlayerPosition);
            if (distance < ClosestDistance)
            {
                ClosestPlayer = p;
                ClosestDistance = distance;
            }
        }
        yield return null;
    }
}
