using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Reroll : MonoBehaviour
{
    bool subBool;
    // Start is called before the first frame update
    void Update()
    {
        if (subBool)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money >= 3)
                {
                    GameObject.Find("TotalMoney").GetComponent<MoneyManager>().PV.RPC("MoneyChange", RpcTarget.MasterClient, -3);
                    GameObject.Find("Shop").GetComponent<Shop>().PV.RPC("RerollShop", RpcTarget.MasterClient);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = "reroll 3$";
            subBool = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = "reroll 3$\n우클릭으로 구매";
            subBool = true;
        }
    }
}
