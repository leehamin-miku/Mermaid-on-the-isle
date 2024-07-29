using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick2 : Block
{
    public override void DestroyAction()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Prefab/Game/Rock", transform.position, transform.rotation);
        }
    }
}
