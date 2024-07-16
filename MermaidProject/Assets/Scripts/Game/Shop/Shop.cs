using Photon.Pun;
using UnityEngine;
public class Shop : MonoBehaviour
{

    public void InitializeShop()
    {
        //마스터만 실행
        for(int i=0; i<4; i++)
        {
            PhotonNetwork.Destroy(transform.GetChild(0).GetChild(i).GetComponent<ItemInfo>().item); 
            string prefabName = "";
            string ItemName = "";
            int price = 0;
            int a = Random.Range(0, 4);
            Debug.Log(a);
            switch (a)
            {
                case 0:
                    prefabName = "Brick2";
                    ItemName = "트라이포드";
                    price = 3;
                    break;
                case 1:
                    prefabName = "GraShoes";
                    ItemName = "그라슈";
                    price = 10;
                    break;
                case 2:
                    prefabName = "Brick1";
                    ItemName = "기본 콘크리트";
                    price = 2;
                    break;
                case 3:
                    prefabName = "Blueprint";
                    ItemName = "블루프린트";
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
