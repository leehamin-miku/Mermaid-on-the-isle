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


    // 방 목록을 가지고 있는 Dictionaly 변수
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // 마스터 클라이언트가 PhotonNetwork.LoadLevel()을 호출할 수 있도록 하고,
        // 같은 룸에 있는 모든 클라이언트가 레벨을 동기화하게 함
        //PhotonNetwork.AutomaticallySyncScene = true;
        //이새끼 대체 뭐임?
        PhotonNetwork.ConnectUsingSettings();
        if (GameObject.Find("RoomExplode") != null)
        {
            Destroy(GameObject.Find("RoomExplode"));
            noticeWindow.SetActive(true);
            noticeWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "호스트 노아와의 연결이 끊어졌습니다\nㅠㅠ";
        }

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
    }
}