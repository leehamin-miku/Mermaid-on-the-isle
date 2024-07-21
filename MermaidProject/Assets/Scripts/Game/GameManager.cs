using EasyTransition;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(-85, -121), Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle*2,ForceMode2D.Impulse);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        GameObject go = new GameObject();
        go.name = "RoomExplode";
        DontDestroyOnLoad(go);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        GameObject.Find("GameManager").GetComponent<ChattingManager>().SystemChatting("<color=yellow>" + otherPlayer.NickName + "¥‘¿Ã ∂∞≥µΩ¿¥œ¥Ÿ</color>");

        base.OnPlayerLeftRoom(otherPlayer);
        
    }

    public override void OnLeftRoom()
    {
        // This will be called when the client leaves the room
        // Optionally, load another scene or handle post-room-leave logic
        if (PhotonNetwork.IsMasterClient)
        {

        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }
}
