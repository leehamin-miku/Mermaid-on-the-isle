using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
public class Shop : MonoBehaviour
{

    //������
    public List<string> shopItemList = new List<string>();
    
    public void InitializeShop()
    {
        //�����͸� ����
        for(int i=0; i<4; i++)
        {
            
            string prefabName = "";
            string ItemName = "";
            int price = 0;
            string a = shopItemList[Random.Range(0, shopItemList.Count)];
            Debug.Log(a);
            switch (a)
            {
                case "Brick2":
                    prefabName = "Brick2";
                    ItemName = "Ʈ��������";
                    price = 3;
                    break;
                case "Brick1":
                    prefabName = "Brick1";
                    ItemName = "�⺻ ��ũ��Ʈ";
                    price = 1;
                    break;
                case "BreakwaterMaker1Blueprint":
                    prefabName = "BreakwaterMaker1Blueprint";
                    ItemName = "�⺻������ ���� û����";
                    price = 7;
                    break;
                case "BreakwaterMaker2Blueprint":
                    prefabName = "BreakwaterMaker2Blueprint";
                    ItemName = "Ʈ�������� ���� û����";
                    price = 20;
                    break;
                case "CraftTable":
                    prefabName = "CraftTable";
                    ItemName = "���۴� û����";
                    price = 7;
                    break;
                case "Wood":
                    prefabName = "Wood";
                    ItemName = "����";
                    price = 1;
                    break;
                case "CannonBlueprint":
                    prefabName = "CannonBlueprint";
                    ItemName = "�������Ʈ";
                    price = 7;
                    break;
            }
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item = PhotonNetwork.Instantiate("Prefab/Game/" + prefabName, transform.GetChild(0).GetChild(i).position, Quaternion.identity);
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().name = ItemName;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().price = price;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().PV.RPC("ArrangeSubFuc", RpcTarget.All);
        }
        
    }
}
