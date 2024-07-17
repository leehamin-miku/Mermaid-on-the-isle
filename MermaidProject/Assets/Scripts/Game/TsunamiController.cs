using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TsunamiController : MonoBehaviour
{
    public Vector3 FlowerLocate;
    public Vector3 TsunamiLocateFromFlower;
    GameObject[] Tsunami = new GameObject[100];
    private void Start()
    {
        StartCoroutine(SummonFirstTsunami(FlowerLocate, TsunamiLocateFromFlower, 5));
        StartCoroutine(StartTsunami(Tsunami, 3));
    }
    

    // 꽃의 위치, 꽃부터 쓰나미의 위치, 쓰나미의 개수
    public IEnumerator SummonFirstTsunami(Vector3 FlowerLocate, Vector3 TsunamiLocateFromFlower, int amount)
    {
        // 지연은 임시
        yield return new WaitForSeconds(3);
        Vector3 TsunamiPosition = FlowerLocate + TsunamiLocateFromFlower;
        Vector3 direction = FlowerLocate - TsunamiPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Tsunami[0] = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
        
        for(int i = 1; i < amount; i++)
        {
            StartCoroutine(SummonNextTsunami(TsunamiPosition, rotation, i, angle));
        }
        yield return null;
    }

    // 쓰나미 나머지 소환
    public IEnumerator SummonNextTsunami(Vector3 TsunamiPosition, Quaternion rotation, int a, float angle)
    {
        a++;
        if (a % 2 == 0)
        {
            TsunamiPosition.y += (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x += (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            Tsunami[a - 1] = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
        }
        else
        {
            TsunamiPosition.y -= (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x -= (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            Tsunami[a - 1] = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
        }
        yield return null;
    }

    // 쓰나미 출발, 쓰나미와 오브젝트가 부딫히면 둘 다 destroy(TsunamiDrop.cs에서 관리)
    IEnumerator StartTsunami(GameObject[] Tsunami, float MoveSpeed)
    {
        // 지연은 임시
        yield return new WaitForSeconds(8);
        float time = 0;
        while (true)
        {
            for(int i = 0;i< 5; i++)
            {
                if (Tsunami[i] != null)
                {
                    Tsunami[i].transform.position += -TsunamiLocateFromFlower * MoveSpeed * Time.deltaTime;
                }
            }
            time += Time.deltaTime;
            if (time > 3) break;
            yield return null;
        }
        yield return null;
    }

}
