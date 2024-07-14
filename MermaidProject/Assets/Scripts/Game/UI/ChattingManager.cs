using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PhotonView PV;

    [PunRPC]
    void Chatting(string context)
    {
        //이상 구현
    }

    //엔터를 눌렀을때 다음이 실행됨 -> GameObject.Find("GameManager").GetComponent<ChattingManager>().PV.RPC("Chatting", RPC.RpcTarget.All, context);
}
