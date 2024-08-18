using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class TsunamiObject : MonoBehaviour
{


    public Vector3 FlowerLocate;
    private void Awake()
    {
        FlowerLocate = GameObject.Find("Island").transform.position;
    }
    Vector3 TsunamiLocateFromFlower;
    public List<GameObject> tsunamiList = new List<GameObject>();
    public float interTime = 0f;

    public void SummonFirstTsunami(Vector3 TsunamiLocateFromFlower, int amount, float interTime)
    {
        this.TsunamiLocateFromFlower = TsunamiLocateFromFlower;
        this.interTime = interTime;
        Vector3 TsunamiPosition = FlowerLocate + TsunamiLocateFromFlower;
        if (amount > 0)
        {
            
            Vector3 direction = FlowerLocate - TsunamiPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GameObject go = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
            go.GetComponent<TsunamiDrop>().TsunamiLocateFromFlower = TsunamiLocateFromFlower;
            tsunamiList.Add(go);

            for (int i = 1; i < amount; i++)
            {
                SummonNextTsunami(TsunamiPosition, rotation, i, angle);
            }
        }
        
    }
    public void SummonCreature(string CreatureName, Vector3 TsunamiLocateFromFlower, int amount, float interTime)
    {
        this.TsunamiLocateFromFlower = TsunamiLocateFromFlower;
        this.interTime = interTime;
        Vector3 TsunamiPosition = FlowerLocate + TsunamiLocateFromFlower;
        Vector3 direction = FlowerLocate - TsunamiPosition;
        
        for (int i=0; i<amount; i++)
        {
            Vector3 a = UnityEngine.Random.insideUnitCircle;
            GameObject go = PhotonNetwork.Instantiate("Prefab/Game/Creature/" + CreatureName, TsunamiPosition + a, Quaternion.identity);
            go.transform.parent = GameObject.Find("CreatureGroup").transform;
            tsunamiList.Add(go);
        }


    }

    // 쓰나미 나머지 소환
    public void SummonNextTsunami(Vector3 TsunamiPosition, Quaternion rotation, int a, float angle)
    {
        a++;
        if (a % 2 == 0)
        {
            TsunamiPosition.y += (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x += (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            GameObject go = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
            go.GetComponent<TsunamiDrop>().TsunamiLocateFromFlower = TsunamiLocateFromFlower;
            tsunamiList.Add(go);
        }
        else
        {
            TsunamiPosition.y -= (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x -= (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            GameObject go = PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation);
            go.GetComponent<TsunamiDrop>().TsunamiLocateFromFlower = TsunamiLocateFromFlower;
            tsunamiList.Add(go);
        }
    }

    // 쓰나미 출발, 쓰나미와 오브젝트가 부딫히면 둘 다 destroy(TsunamiDrop.cs에서 관리)

    public void StartTsunami()
    {
        foreach (GameObject go in tsunamiList)
        {
            go.GetComponent<TsunamiUnit>().StartTunamiUnit();
        }
    }
    public void DestroyTsunami()
    {
        foreach (GameObject go in tsunamiList)
        {
            if (go != null)
            {
                PhotonNetwork.Destroy(go);
            }
        }
        Destroy(this.gameObject);
    }


}
