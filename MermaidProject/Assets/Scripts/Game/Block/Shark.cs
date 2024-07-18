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
        print("a");
        StartCoroutine(SharkAi());

        //샤크 코루틴 시작
        //인공지능 구현해야함
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("닿았다!");
            // 데미지?
        }
    }

    private IEnumerator SharkAi()
    {
        Vector3 ClosestPlayerPosition;
        FlowerPosition = GameObject.Find("Flower").transform.position;
        float DetectDistance = 10f;
        float Power = 50f;
        while (true)
        {
            StartCoroutine(FindClosestPlayerPosition());
            ClosestPlayerPosition = ClosestPlayer.transform.position;
            print(ClosestPlayerPosition);
            print(SharkPosition);
            if (Vector3.Distance(ClosestPlayerPosition,SharkPosition) < DetectDistance)
            {
                rb.AddForce((ClosestPlayerPosition - SharkPosition).normalized * Power, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce((FlowerPosition - SharkPosition).normalized * Power, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(2f);
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
