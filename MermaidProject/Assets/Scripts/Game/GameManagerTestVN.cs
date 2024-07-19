using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTestVN : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = "������";
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ���� ���� ����");

        //�κ�����
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("�κ� ���� ����");
        PhotonNetwork.JoinOrCreateRoom("RoomTestVN", new Photon.Realtime.RoomOptions { }, null);

    }
    public override void OnJoinedRoom()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(-85, -121), Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 30, ForceMode2D.Impulse);
    }
    }
