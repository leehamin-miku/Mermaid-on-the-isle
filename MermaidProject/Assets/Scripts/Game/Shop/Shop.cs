using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
public class Shop : MonoBehaviour
{
    public PhotonView PV;
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
                case "BM1Blueprint":
                    prefabName = "BM1Blueprint";
                    ItemName = "�⺻������ ���� û����";
                    price = 7;
                    break;
                case "BM2Blueprint":
                    prefabName = "BM2Blueprint";
                    ItemName = "Ʈ�������� ���� û����";
                    price = 20;
                    break;
                case "CraftTableBlueprint":
                    prefabName = "CraftTableBlueprint";
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
                default:
                    Debug.Log("��ã���� ����");
                    break;
            }
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item = PhotonNetwork.Instantiate("Prefab/Game/" + prefabName, transform.GetChild(0).GetChild(i).position, Quaternion.identity);
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item.transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().name = ItemName;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().price = price;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().PV.RPC("ArrangeSubFuc", RpcTarget.All);
        }
        
    }

    [PunRPC]
    public void RerollShop()
    {
        //�����Ϳ��� ����Ǵ� �Լ�
        for (int i = 0; i < 4; i++)
        {
            if (transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item != null)
            {
                PhotonNetwork.Destroy(transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item);
            }

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
                case "BM1Blueprint":
                    prefabName = "BM1Blueprint";
                    ItemName = "�⺻������ ���� û����";
                    price = 7;
                    break;
                case "BM2Blueprint":
                    prefabName = "BM2Blueprint";
                    ItemName = "Ʈ�������� ���� û����";
                    price = 20;
                    break;
                case "CraftTableBlueprint":
                    prefabName = "CraftTableBlueprint";
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
                default:
                    Debug.Log("��ã���� ����");
                    break;
            }
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item = PhotonNetwork.Instantiate("Prefab/Game/" + prefabName, transform.GetChild(0).GetChild(i).position, Quaternion.identity);
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item.transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().name = ItemName;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().price = price;
            transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().PV.RPC("ArrangeSubFuc", RpcTarget.All);
        }
    }
}
