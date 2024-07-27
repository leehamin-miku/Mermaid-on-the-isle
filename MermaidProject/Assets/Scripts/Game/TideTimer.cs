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
        string minute = (totalTime - a) / 60 < 10 ? "0" + ((totalTime - a) / 60).ToString() : ((totalTime - a) / 60).ToString();
        string second = (totalTime - a) % 60 < 10 ? "0" + ((totalTime - a) % 60).ToString() : ((totalTime - a) % 60).ToString();
        GetComponent<TextMeshProUGUI>().text = "���ϱ��� �����ð�\n" + minute + ':' + second;
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
        int c = Random.Range(3, 5); // ������
        int d = Random.Range(1, 3); // �������

        int a = Random.Range(12, 18); //�Ϲ� ����

        int e = Random.Range(5, 8); // ��
        int f = Random.Range(2, 5); // ����

        Transform save = GameObject.Find("SaveObjectGroup").transform;
        while (a > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                a--;
                PhotonNetwork.Instantiate("Prefab/Game/Shell", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
            }
        }
        while (b > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                b--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellBronze", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
            }
        }
        while (c > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                c--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellGold", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
            }
        }
        while (d > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                d--;
                PhotonNetwork.Instantiate("Prefab/Game/ShellRuby", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
            }
        }
        while (e > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.CircleCast(new Vector2(position.x, position.y), 1f, Vector2.down, 0.1f, LayerMask.GetMask("Sea")) == true && Physics2D.CircleCast(new Vector2(position.x, position.y), 1f, Vector2.down, 0.1f, LayerMask.GetMask("Island")) == true)
            {
                e--;
                PhotonNetwork.Instantiate("Prefab/Game/Rock", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
            }
        }
        while (f > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.CircleCast(new Vector2(position.x, position.y), 1f, Vector2.down, 0.1f, LayerMask.GetMask("Sea")) == true && Physics2D.CircleCast(new Vector2(position.x, position.y), 1f, Vector2.down, 0.1f, LayerMask.GetMask("Island")) == true)
            {
                f--;
                PhotonNetwork.Instantiate("Prefab/Game/Wood", new Vector3(position.x, position.y), Quaternion.Euler(0, 0, Random.Range(0, 360))).transform.SetParent(save);
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
                if (go.GetComponent<Block>().BlockCode != 0)//�����ϰ� ���� ������ ������ ���� �ϴ°͵� ���
                {
                    if (Physics2D.Raycast(go.transform.position, Vector3.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(go.transform.position, Vector3.down, 0.01f, LayerMask.GetMask("Island")) == false)
                    {
                        PhotonNetwork.Destroy(go);
                    }
                }
                    
            }
        }
    }
}
