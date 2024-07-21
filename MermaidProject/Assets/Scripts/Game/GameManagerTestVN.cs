using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTestVN : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = "조서형";
        
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");

        //로비진입
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("로비 진입 성공");
        PhotonNetwork.JoinOrCreateRoom("RoomTestVN", new Photon.Realtime.RoomOptions { CleanupCacheOnLeave =false }, null);

    }
    public override void OnJoinedRoom()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(-85, -121), Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 30, ForceMode2D.Impulse);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName);
        Debug.Log(otherPlayer.IsMasterClient);
        // Check if the player who left is the master client
        if (otherPlayer.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        // This will be called when the client leaves the room
        // Optionally, load another scene or handle post-room-leave logic
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
}
