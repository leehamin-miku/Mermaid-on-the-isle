using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Rigidbody2D>().AddForce(Vector2.up,ForceMode2D.Impulse);
    }
}
