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
        //�̻� ����
    }

    //���͸� �������� ������ ����� -> GameObject.Find("GameManager").GetComponent<ChattingManager>().PV.RPC("Chatting", RPC.RpcTarget.All, context);
}
