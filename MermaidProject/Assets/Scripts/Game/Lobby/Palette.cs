using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class palette : MonoBehaviour
{
    [SerializeField] int a;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            collision.GetComponent<PlayerController>().PV.RPC("ChangeColor", RpcTarget.AllBuffered, a);
        }
    }
}
