using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TsunamiObject : MonoBehaviour
{
    public Vector3 FlowerLocate = new Vector3(8, -134, 0);
    Vector3 TsunamiLocateFromFlower;
    List<GameObject> tsunamiList = new List<GameObject>();
    public float interTime = 0f;

    public void SummonFirstTsunami(Vector3 TsunamiLocateFromFlower, int amount, float interTime)
    {
        this.TsunamiLocateFromFlower = TsunamiLocateFromFlower;
        this.interTime = interTime;
        Vector3 TsunamiPosition = FlowerLocate + TsunamiLocateFromFlower;
        Vector3 direction = FlowerLocate - TsunamiPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        tsunamiList.Add(PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation));

        for (int i = 1; i < amount; i++)
        {
            SummonNextTsunami(TsunamiPosition, rotation, i, angle);
        }
    }

    // ������ ������ ��ȯ
    public void SummonNextTsunami(Vector3 TsunamiPosition, Quaternion rotation, int a, float angle)
    {
        a++;
        if (a % 2 == 0)
        {
            TsunamiPosition.y += (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x += (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            tsunamiList.Add(PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation));
        }
        else
        {
            TsunamiPosition.y -= (a / 2) * Mathf.Sin(angle * Mathf.Deg2Rad);
            TsunamiPosition.x -= (a / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
            tsunamiList.Add(PhotonNetwork.Instantiate("Prefab/Game/TsunamiDrop", TsunamiPosition, rotation));
        }
    }

    // ������ ���, �����̿� ������Ʈ�� �΋H���� �� �� destroy(TsunamiDrop.cs���� ����)

    public void StartTsunami()
    {
        foreach(GameObject go in tsunamiList)
        {
            go.GetComponent<TsunamiDrop>().rb.AddForce(-TsunamiLocateFromFlower*5, ForceMode2D.Impulse);
        }
    }
    public void DestroyTsunami()
    {
        foreach (GameObject go in tsunamiList)
        {
            PhotonNetwork.Destroy(go);
        }
    }

}
