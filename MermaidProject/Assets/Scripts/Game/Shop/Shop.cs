using Photon.Pun;
using UnityEngine;
public class Shop : MonoBehaviour
{

    public void InitializeShop()
    {
        //�����͸� ����
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
                    ItemName = "Ʈ��������";
                    price = 3;
                    break;
                case 1:
                    prefabName = "GraShoes";
                    ItemName = "�׶�";
                    price = 10;
                    break;
                case 2:
                    prefabName = "Brick1";
                    ItemName = "�⺻ ��ũ��Ʈ";
                    price = 2;
                    break;
                case 3:
                    prefabName = "Blueprint";
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
