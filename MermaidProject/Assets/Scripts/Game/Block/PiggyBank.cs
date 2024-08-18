using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;


public class PiggyBank : Block
{
    bool isCool = false;
    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&isCool==false&&(collision.collider.GetComponent<Block>().BlockCode == 4|| collision.collider.GetComponent<Block>().BlockCode == 5||collision.collider.GetComponent<Block>().BlockCode == 6)&& collision.collider.GetComponent<Block>().isGrabed)
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
            StartCoroutine(CoolDown(0.5f));
            collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            GameObject.Find("TotalMoney").GetPhotonView().RPC("MoneyChange", RpcTarget.MasterClient, moneyAmount);
        }
    }

    public IEnumerator CoolDown(float a)
    {
        isCool = true;
        float b = 0;
        while (b <= a)
        {
            b += Time.deltaTime;
            yield return null;
        }
        isCool = false;
    }
}
