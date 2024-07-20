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
    [SerializeField] public PhotonView PV;
    public int price;
    public string name;
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
                    item.GetComponent<Block>().PV.RPC("StartObject", RpcTarget.MasterClient);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (item != null&&collision.GetComponent<Block>().BlockCode == 0&& collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = name+" "+ price+"$";
            subBool = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (item != null && collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<Block>().PV.IsMine)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = name + " " + price+"$\n��Ŭ������ ����";
            subBool = true;
        }
    }

    [PunRPC]
    public void BuyThisItem()
    {
        //���Ű� Ȯ���Ǿ�����, ��ο��Լ� ȣ��Ǵ� �Լ�
        transform.GetChild(0).GetComponent<TextMeshPro>().text = "sold out";
        item.GetComponent<Block>().rb.isKinematic = false;
        item.GetComponent<Block>().isAbleGrabed = true;
        item = null;
        subBool = false;
    }

    [PunRPC]
    public void ArrangeSubFuc()
    {
        transform.GetChild(0).GetComponent<TextMeshPro>().text = name + " " + price + "$";
        subBool = false;
        item.GetComponent<Block>().rb.isKinematic = true;
        item.GetComponent<Block>().isAbleGrabed = false;
    }
}
