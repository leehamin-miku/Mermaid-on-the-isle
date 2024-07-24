using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PiggyBank : Block
{

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&(collision.collider.GetComponent<Block>().BlockCode == 5|| collision.collider.GetComponent<Block>().BlockCode == 6||collision.collider.GetComponent<Block>().BlockCode == 7)&& collision.collider.GetComponent<Block>().isGrabed)
        {
            int moneyAmount = 0;
            switch (collision.collider.GetComponent<Block>().BlockCode)
            {
                case 6:
                    moneyAmount = 5;
                    break;
                case 5:
                    moneyAmount = 3;
                    break;
                case 4:
                    moneyAmount = 2;
                    break;
            }
            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            GameObject.Find("TotalMoney").GetPhotonView().RPC("MoneyChange", RpcTarget.MasterClient, moneyAmount);
        }
    }
}
