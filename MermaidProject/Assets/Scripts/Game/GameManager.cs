using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", Vector3.zero, Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle*30,ForceMode2D.Impulse);
    }
}
