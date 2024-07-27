using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : Block
{
    // Start is called before the first frame update

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
    private void OnDestroy()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("게임종료");
            PV.RPC("GameOver", RpcTarget.Others);
            GameObject go = new GameObject();
            go.name = "GameOver";
            DontDestroyOnLoad(go);
            PhotonNetwork.LeaveRoom();
        }
        
    }

    [PunRPC]
    void GameOver()
    {
        GameObject go = new GameObject();
        go.name = "GameOver";
        DontDestroyOnLoad(go);
        PhotonNetwork.LeaveRoom();
    }
}
