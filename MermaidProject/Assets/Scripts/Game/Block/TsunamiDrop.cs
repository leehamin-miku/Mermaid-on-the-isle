using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiDrop : TsunamiUnit
{
    public Vector2 TsunamiLocateFromFlower;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Block>() != null && collision.collider.GetComponent<Block>().BlockCode != 0 && collision.collider.GetComponent<TsunamiUnit>() == null)
        {
            collision.collider.GetComponent<Block>().PV.RPC("ChangeStrength", Photon.Pun.RpcTarget.All, -2f);
        }
    }
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        rb.AddForce(-TsunamiLocateFromFlower, ForceMode2D.Impulse);
    }
}
