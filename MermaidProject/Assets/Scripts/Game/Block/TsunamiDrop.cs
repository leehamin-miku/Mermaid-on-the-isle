using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiDrop : TsunamiUnit
{
    public Vector2 TsunamiLocateFromFlower;
    bool isAbleAttack = true;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PV.IsMine)
        {
            if (collision.collider.GetComponent<Block>() != null && collision.collider.GetComponent<Block>().BlockCode != 0 && collision.collider.GetComponent<TsunamiUnit>() == null && isAbleAttack)
            {
                isAbleAttack = false;
                collision.collider.GetComponent<Block>().PV.RPC("ChangeStrength", Photon.Pun.RpcTarget.All, -2);
            }
        }
        
    }
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        rb.AddForce(-TsunamiLocateFromFlower, ForceMode2D.Impulse);
    }
}
