using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TideTimer : MonoBehaviour
{
    public int totalTime;
    int nowTime;
    [PunRPC]
    public void SetTimeMark(int a)
    {
        GetComponent<TextMeshProUGUI>().text = "���ϱ��� �����ð�\n"+ "0" + (totalTime - a) / 60 + ":" + (totalTime - a) % 60;
    }


    //���������� ȣ��Ǵ� �ڷ�ƾ(�ڵ�)
    IEnumerator TimerFuc()
    {
        yield return new WaitForSeconds(1);
        nowTime += 1;
        GetComponent<PhotonView>().RPC("SetTimeMark", RpcTarget.All, nowTime);
        if (nowTime < totalTime)
        {
            StartCoroutine(TimerFuc());
        } else
        {
            Transform ts = GameObject.Find("TsunamiGroup").transform;
            foreach(Transform tsunami in ts)
            {
                tsunami.GetComponent<TsunamiObject>().StartTsunami();
                yield return new WaitForSeconds(tsunami.GetComponent<TsunamiObject>().interTime);
            }

            yield return MonCheckCoroutine();

            foreach (Transform tsunami in ts)
            {
                tsunami.GetComponent<TsunamiObject>().DestroyTsunami();
            }
            DestroyMap();
            for(int i=0; i<4; i++)
            {
                if (GameObject.Find("Shop").transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item != null)
                {
                    PhotonNetwork.Destroy(GameObject.Find("Shop").transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item);
                }
            }

            GameObject[] temp = FindObjectsOfType<GameObject>();
            foreach (GameObject go in temp)
            {
                if (go.GetComponent<Block>() != null)
                {
                    go.GetComponent<Block>().StopObject();
                }
            }

            GameObject.Find("VN").GetComponent<VNManager>().StartNextDialogue();
        }
    }

    IEnumerator MonCheckCoroutine()
    {
        while (true)
        {
            if(GameObject.Find("CreatureGroup").transform.childCount == 0)
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    //���������� ȣ��Ǵ� �Լ�(��������� �ʿ�)
    public void TimeSetAndStart(int totalTime)
    {
        nowTime = 0;
        BuildMap();
        this.totalTime = totalTime;
        StartCoroutine(TimerFuc());
    }

    //���� ��ġ�ϴ� �Լ�(�����͸� ����)
    void BuildMap()
    {
        int b = Random.Range(6, 8); //��
        int c = Random.Range(4, 6); // ������
        int d = Random.Range(2, 4); // �������

        int a = Random.Range(10, 15); //�Ϲ� ����

        while (a > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                a--;
                PhotonNetwork.Instantiate("Prefab/Game/Shell", new Vector3(position.x, position.y), Quaternion.identity);
            }
        }
        while (b > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                b--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellBronze", new Vector3(position.x, position.y), Quaternion.identity);
            }
        }
        while (c > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                c--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellGold", new Vector3(position.x, position.y), Quaternion.identity);
            }
        }
        while (d > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                d--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellRuby", new Vector3(position.x, position.y), Quaternion.identity);
            }
        }
        //Debug.Log(LayerMask.GetMask("Block"));
        //Debug.Log(LayerMask.GetMask("Sea"));
        //Debug.Log(LayerMask.GetMask("Island"));
        //Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
        //Debug.Log(new Vector2(position.x, position.y));
        //Debug.Log(Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true);
        //Debug.Log(Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false);

    }

    void DestroyMap()
    {
        GameObject[] goArr = FindObjectsOfType<GameObject>();
        foreach(GameObject go in goArr)
        {
            if (go.GetComponent<Block>() != null)
            {
                {

                }
                if (Physics2D.Raycast(go.transform.position, Vector3.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(go.transform.position, Vector3.down, 0.01f, LayerMask.GetMask("Island")) == false)
                {
                    PhotonNetwork.Destroy(go);
                }
                    
            }
        }
    }
}
