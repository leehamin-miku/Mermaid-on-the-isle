using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
public class Shop : MonoBehaviour
{
    public PhotonView PV;
    //저장대상
    public List<string> shopItemList = new List<string>();
    public void InitializeShop()
    {
        //마스터만 실행
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
                    ItemName = "트라이포드";
                    price = 3;
                    break;
                case "Brick1":
                    prefabName = "Brick1";
                    ItemName = "기본 콘크리트";
                    price = 1;
                    break;
                case "BM1Blueprint":
                    prefabName = "BM1Blueprint";
                    ItemName = "기본방파제 공장 청사진";
                    price = 7;
                    break;
                case "BM2Blueprint":
                    prefabName = "BM2Blueprint";
                    ItemName = "트라이포드 공장 청사진";
                    price = 20;
                    break;
                case "CraftTableBlueprint":
                    prefabName = "CraftTableBlueprint";
                    ItemName = "제작대 청사진";
                    price = 7;
                    break;
                case "Wood":
                    prefabName = "Wood";
                    ItemName = "나무";
                    price = 1;
                    break;
                case "CannonBlueprint":
                    prefabName = "CannonBlueprint";
                    ItemName = "블루프린트";
                    price = 7;
                    break;
                default:
                    Debug.Log("못찾았음 ㅈㅅ");
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
        //마스터에서 실행되는 함수
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
                    ItemName = "트라이포드";
                    price = 3;
                    break;
                case "Brick1":
                    prefabName = "Brick1";
                    ItemName = "기본 콘크리트";
                    price = 1;
                    break;
                case "BM1Blueprint":
                    prefabName = "BM1Blueprint";
                    ItemName = "기본방파제 공장 청사진";
                    price = 7;
                    break;
                case "BM2Blueprint":
                    prefabName = "BM2Blueprint";
                    ItemName = "트라이포드 공장 청사진";
                    price = 20;
                    break;
                case "CraftTableBlueprint":
                    prefabName = "CraftTableBlueprint";
                    ItemName = "제작대 청사진";
                    price = 7;
                    break;
                case "Wood":
                    prefabName = "Wood";
                    ItemName = "나무";
                    price = 1;
                    break;
                case "CannonBlueprint":
                    prefabName = "CannonBlueprint";
                    ItemName = "블루프린트";
                    price = 7;
                    break;
                default:
                    Debug.Log("못찾았음 ㅈㅅ");
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
