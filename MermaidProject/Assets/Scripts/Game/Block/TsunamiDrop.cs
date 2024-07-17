using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiDrop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.tag + "¿Í Á¢ÃËÇÔ!");
        PhotonNetwork.Destroy(collision.gameObject);
        PhotonNetwork.Destroy(transform.gameObject);
    }
}
