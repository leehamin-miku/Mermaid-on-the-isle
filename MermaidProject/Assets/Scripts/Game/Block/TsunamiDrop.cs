using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiDrop : TsunamiUnit
{
    public Vector2 TsunamiLocateFromFlower;
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        rb.AddForce(-TsunamiLocateFromFlower, ForceMode2D.Impulse);
    }
}