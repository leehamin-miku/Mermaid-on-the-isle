using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    bool subBool;
    public bool solded;
    [SerializeField] public PhotonView PV;
    public int price;
    public string itemName;
    public GameObject item;

    void Update()
    {
        if (subBool)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money >= price)
                {
                    GameObject.Find("TotalMoney").GetComponent<MoneyManager>().PV.RPC("MoneyChange", RpcTarget.MasterClient, -price);
                    PV.RPC("BuyThisItem", RpcTarget.All);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!solded&&collision.GetComponent<Block>().BlockCode == 0&& collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = name+" "+ price+"$";
            subBool = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!solded && collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = name + " " + price+"$\n우클릭으로 구매";
            subBool = true;
        }
    }

    [PunRPC]
    public void BuyThisItem()
    {
        //구매가 확정되었을때, 모두에게서 호출되는 함수
        
        transform.GetChild(0).GetComponent<TextMeshPro>().text = "sold out";
        
        solded = true;
        subBool = false;
        if (PhotonNetwork.IsMasterClient)
        {
            item.GetComponent<Block>().rb.isKinematic = false;
            item.GetComponent<Block>().StartObject();
            item.GetComponent<Block>().rb.AddForce(Vector2.down);
            item.GetComponent<Block>().PV.RPC("PVFucIsNotGrabed", RpcTarget.All);
            item = null;
        }
    }

    [PunRPC]
    public void ArrangeSubFuc(string itemName, int price)
    {
        this.itemName = itemName;
        this.price = price;

        transform.GetChild(0).GetComponent<TextMeshPro>().text = itemName + " " + price + "$";
        subBool = false;
        solded = false;
        if (PhotonNetwork.IsMasterClient)
        {
            item.GetComponent<Block>().rb.isKinematic = true;
            item.GetComponent<Block>().PV.RPC("PVFucIsGrabed", RpcTarget.All);
        }
        
    }
}
