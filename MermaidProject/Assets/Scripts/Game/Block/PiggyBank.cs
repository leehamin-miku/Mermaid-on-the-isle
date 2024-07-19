using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PiggyBank : Block
{

    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&(collision.collider.GetComponent<Block>().BlockCode == 17|| collision.collider.GetComponent<Block>().BlockCode == 18||collision.collider.GetComponent<Block>().BlockCode == 19)&& collision.collider.GetComponent<Block>().isGrabed)
        {
            int moneyAmount = 0;
            switch (collision.collider.GetComponent<Block>().BlockCode)
            {
                case 17:
                    moneyAmount = 5;
                    break;
                case 18:
                    moneyAmount = 3;
                    break;
                case 19:
                    moneyAmount = 1;
                    break;
            }
            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            GameObject.Find("TotalMoney").GetPhotonView().RPC("MoneyChange", RpcTarget.MasterClient, moneyAmount);
        }
    }
}
