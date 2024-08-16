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

    // 방 목록을 가지고 있는 Dictionaly 변수
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // 마스터 클라이언트가 PhotonNetwork.LoadLevel()을 호출할 수 있도록 하고,
        // 같은 룸에 있는 모든 클라이언트가 레벨을 동기화하게 함
        //PhotonNetwork.AutomaticallySyncScene = true;
        //이새끼 대체 뭐임?
        PhotonNetwork.ConnectUsingSettings();
        if (GameObject.Find("GameClear") != null)
        {
            Destroy(GameObject.Find("GameClear"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "게임 클리어!\n플레이해주셔서 감사합니다";
        } else if (GameObject.Find("GameOver") != null)
        {
            Destroy(GameObject.Find("GameOber"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Game Over\n노아의 꽃이 파도에 휘말려 사라져버렸습니다";
        }
        else if (GameObject.Find("RoomExplode") != null)
        {
            Destroy(GameObject.Find("RoomExplode"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "호스트 노아와의 연결이 끊어졌습니다\nㅠㅠ";
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
    //방 목록의 변화가 있을 때 호출되는 함수

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
        Debug.Log("마스터 서버 접속 성공");

        //로비진입
        PhotonNetwork.JoinLobby();
        
    }

    //Lobby 진입을 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("로비 진입 성공");
        
    }


    // 생성 버튼 클릭시 호출되는 함수
    public void OnClickCreateRoom()
    {
        PhotonNetwork.NickName = newStartNickName.text;

        ////방 생성
        ////PhotonNetwork.CreateRoom(newStartRoomName.text, options);
        PhotonNetwork.CreateRoom(newStartRoomName.text, new RoomOptions { CleanupCacheOnLeave = true, IsOpen = true, MaxPlayers = 4 }, null);

    }
    public void OnClickLoadRoom()
    {
        PhotonNetwork.NickName = loadNickName.text;

        ////방 생성
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
        noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "방 만들기 실패!\n이미 존재하는 방 이름일 수 있어용";
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("방 생성 성공");
    }
    public void OnClickJoinRoom()
    {
        // 방 참여
        PhotonNetwork.NickName = joinNickName.text;
        PhotonNetwork.JoinRoom(joinRoomName.text);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        DataManager.Instance.SaveGameData(DataManager.Instance.data);
        PhotonNetwork.LoadLevel("GameScene");
        Debug.Log("방 입장 성공");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        OnClickClose();
        noticeWindow.SetActive(true);
        noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "방 입장 실패!\n노아가 이 방에 들어가기에 적절치 못한 상황인가봐요...";
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("존재하지 않는 방입니다"+message);
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