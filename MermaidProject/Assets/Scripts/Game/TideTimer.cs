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
        GetComponent<TextMeshProUGUI>().text = "해일까지 남은시간\n"+ "0" + (totalTime - a) / 60 + ":" + (totalTime - a) % 60;
    }


    //마스터한테 호출되는 코루틴(자동)
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
                PhotonNetwork.Destroy(GameObject.Find("Shop").transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item);
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

    //마스터한테 호출되는 함수(명시적으로 필요)
    public void TimeSetAndStart(int totalTime)
    {
        nowTime = 0;
        BuildMap();
        this.totalTime = totalTime;
        StartCoroutine(TimerFuc());
    }

    //조개 배치하는 함수(마스터만 실행)
    void BuildMap()
    {
        GameObject[] goArr = FindObjectsOfType<GameObject>();
        foreach (GameObject go in goArr)
        {
            if (go.GetComponent<Block>() != null)
            {
                go.GetComponent<Block>().isRunning = true;

            }
        }


        int a = 30;
        while (a > 0)
        {
            Vector3 position = Random.insideUnitSphere * 40 + GameObject.Find("Island").transform.position;
            if (Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Sea")) == true && Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, 0.01f, LayerMask.GetMask("Island")) == false)
            {
                a--;
                PhotonNetwork.Instantiate("Prefab/Game/Shell", new Vector3(position.x, position.y), Quaternion.identity);
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
                go.GetComponent<Block>().isRunning = false;
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
