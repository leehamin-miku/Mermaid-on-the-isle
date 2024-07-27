using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick1 : Block
{
    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Prefab/Game/Rock", transform.position, transform.rotation);
        }
    }
}
