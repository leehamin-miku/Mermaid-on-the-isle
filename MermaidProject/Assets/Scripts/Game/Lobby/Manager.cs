using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class Manager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
    public override void OnJoinedRoom() {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", Vector2.zero, Quaternion.identity);
    }
}
