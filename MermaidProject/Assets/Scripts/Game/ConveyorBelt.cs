using ExitGames.Client.Photon;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<Block>().p1 == null)
        {
            collision.GetComponent<Block>().rb.AddForce(transform.right, ForceMode2D.Impulse);
        }
    }
}
