using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.Texture2DShaderProperty;

public class TideTimer : MonoBehaviour
{
    int totalTime;
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

            foreach (Transform tsunami in ts)
            {
                tsunami.GetComponent<TsunamiObject>().DestroyTsunami();
            }
            GameObject.Find("VN").GetComponent<VNManager>().PV.RPC("StartNextDialogue", RpcTarget.All);
        }
    }

    //마스터한테 호출되는 함수(명시적으로 필요)
    public void TimeSetAndStart(int totalTime)
    {
        nowTime = 0;
        this.totalTime = totalTime;
        StartCoroutine(TimerFuc());
    }
}
