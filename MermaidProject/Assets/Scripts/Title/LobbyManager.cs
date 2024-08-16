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
    [SerializeField] GameObject loadWindow;
    [SerializeField] GameObject SettingWindow;

    [SerializeField] InputField newStartRoomName;
    [SerializeField] InputField newStartNickName;
    [SerializeField] Button newStartButton;

    [SerializeField] InputField joinNickName;
    [SerializeField] InputField joinRoomName;
    [SerializeField] Button joinButton;

    public int a = -1;
    [SerializeField] InputField loadRoomName;
    [SerializeField] InputField loadNickName;
    [SerializeField] GameObject loadObject0;
    [SerializeField] GameObject loadObject1;
    [SerializeField] GameObject loadObject2;
    [SerializeField] Button loadButton;

    [SerializeField] Slider MS;
    [SerializeField] Slider MV;
    [SerializeField] Slider BV;
    [SerializeField] Slider SV;

    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // ������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ֵ��� �ϰ�,
        // ���� �뿡 �ִ� ��� Ŭ���̾�Ʈ�� ������ ����ȭ�ϰ� ��
        //PhotonNetwork.AutomaticallySyncScene = true;
        //�̻��� ��ü ����?
        PhotonNetwork.ConnectUsingSettings();
        if (GameObject.Find("GameClear") != null)
        {
            Destroy(GameObject.Find("GameClear"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "���� Ŭ����!\n�÷������ּż� �����մϴ�";
        } else if (GameObject.Find("GameOver") != null)
        {
            Destroy(GameObject.Find("GameOber"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Game Over\n����� ���� �ĵ��� �ָ��� ��������Ƚ��ϴ�";
        }
        else if (GameObject.Find("RoomExplode") != null)
        {
            Destroy(GameObject.Find("RoomExplode"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "ȣ��Ʈ ��ƿ��� ������ ���������ϴ�\n�Ф�";
        }
        

        loadWindow.SetActive(true);
        loadObject0.GetComponent<LoadObject>().DataMark();
        loadObject1.GetComponent<LoadObject>().DataMark();
        loadObject2.GetComponent<LoadObject>().DataMark();
        loadWindow.SetActive(false);

        SettingWindow.SetActive(true);
        MS.value = DataManager.Instance.data.MS;
        MV.value = DataManager.Instance.data.MV;
        SV.value = DataManager.Instance.data.SV;
        BV.value = DataManager.Instance.data.BV;
        SettingWindow.SetActive(false);


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
    public void OnLoadValueChanged()
    {
        loadButton.interactable = (loadRoomName.text.Length > 0) && (loadNickName.text.Length > 0)&&a!=-1;
        if ((loadRoomName.text == "") || (loadNickName.text == ""))
            loadButton.interactable = false;
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
    public void OnClickLoadRoom()
    {
        PhotonNetwork.NickName = loadNickName.text;

        ////�� ����
        ////PhotonNetwork.CreateRoom(newStartRoomName.text, options);
        PhotonNetwork.CreateRoom(loadRoomName.text, new RoomOptions { CleanupCacheOnLeave = true, IsOpen = true, MaxPlayers = 4 }, null);
        GameObject go = new GameObject();
        go.name = "LoadData" + a;
        DontDestroyOnLoad(go);
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
        DataManager.Instance.SaveGameData(DataManager.Instance.data);
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
        loadWindow.SetActive(false);
        SettingWindow.SetActive(false);
    }
}