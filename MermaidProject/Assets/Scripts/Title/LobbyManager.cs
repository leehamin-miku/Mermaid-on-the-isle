using EasyTransition;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject newStartWindow;
    [SerializeField] GameObject joinWindow;
    [SerializeField] GameObject noticeWindow;

    [SerializeField] InputField newStartRoomName;
    [SerializeField] InputField newStartNickName;
    [SerializeField] Button newStartButton;

    [SerializeField] InputField joinNickName;
    [SerializeField] InputField joinRoomName;
    [SerializeField] Button joinButton;


    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // ������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ֵ��� �ϰ�,
        // ���� �뿡 �ִ� ��� Ŭ���̾�Ʈ�� ������ ����ȭ�ϰ� ��
        //PhotonNetwork.AutomaticallySyncScene = true;
        //�̻��� ��ü ����?
        PhotonNetwork.ConnectUsingSettings();
        if (GameObject.Find("RoomExplode") != null)
        {
            Destroy(GameObject.Find("RoomExplode"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "ȣ��Ʈ ��ƿ��� ������ ���������ϴ�\n�Ф�";
        }

    }
    //�� ����� ��ȭ�� ���� �� ȣ��Ǵ� �Լ�

    public void OnNewStartValueChanged()
    {
        newStartButton.interactable = (newStartRoomName.text.Length > 0)&& (newStartNickName.text.Length > 0);
        if ((newStartRoomName.text == "") || (newStartNickName.text == ""))
            newStartButton.interactable = false;
    }

    public void OnJoinValueChanged()
    {
        joinButton.interactable = (joinRoomName.text.Length > 0) && (joinNickName.text.Length > 0);
        if ((joinRoomName.text == "") || (joinNickName.text == ""))
            joinButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ���� ���� ����");

        //�κ�����
        PhotonNetwork.JoinLobby();
        
    }

    //Lobby ������ ���������� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("�κ� ���� ����");
        
    }


    // ���� ��ư Ŭ���� ȣ��Ǵ� �Լ�
    public void OnClickCreateRoom()
    {
        PhotonNetwork.NickName = newStartNickName.text;

        ////�� ����
        ////PhotonNetwork.CreateRoom(newStartRoomName.text, options);
        PhotonNetwork.CreateRoom(newStartRoomName.text, new RoomOptions { CleanupCacheOnLeave = true, IsOpen = true, MaxPlayers = 4 }, null);

    }





    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        OnClickClose();
        noticeWindow.SetActive(true);
        noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "�� ����� ����!\n�̹� �����ϴ� �� �̸��� �� �־��";
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("�� ���� ����");

    }
    public void OnClickJoinRoom()
    {
        // �� ����
        PhotonNetwork.NickName = joinNickName.text;
        PhotonNetwork.JoinRoom(joinRoomName.text);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("GameScene");
        Debug.Log("�� ���� ����");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        OnClickClose();
        noticeWindow.SetActive(true);
        noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "�� ���� ����!\n��ư� �� �濡 ���⿡ ����ġ ���� ��Ȳ�ΰ�����...";
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("�������� �ʴ� ���Դϴ�"+message);
    }

    public void OnClickClose()
    {
        joinWindow.SetActive(false);
        newStartWindow.SetActive(false);
        noticeWindow.SetActive(false);
    }
}