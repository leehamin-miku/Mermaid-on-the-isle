using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{

    public GameObject totalMoney;
    public string itemInfo;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("aaaaa");
        if(collision.GameObject().tag == "Player")
        {
            print("�΋H��");
            StartCoroutine(SummonSaleItems("Brick", new Vector3(85, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
            StartCoroutine(SummonSaleItems("Brick2", new Vector3(90, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            StartCoroutine(SummonSaleItems("Brick2", new Vector3(95, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            StartCoroutine(SummonSaleItems("Brick", new Vector3(100, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
        }
    }

    // prefab�� ���� �̸�, ��ȯ�� ��ǥ, ����, ����
    public IEnumerator SummonSaleItems(string Prefabname, Vector3 position, Quaternion rotation, int price, float radius)
    {
        // ������ ���� ä�� Ÿ�̹��� �����ؼ� �ϴ� ���� 5�� �� ����

        bool isSaled = false;
        GameObject SaleItem = PhotonNetwork.Instantiate("Prefab/Game/" + Prefabname, position, rotation);
        SaleItem.transform.GetComponent<Block>().isAbleGrabed = false;
        SaleItem.transform.GetComponent<Rigidbody2D>().isKinematic = true;
        // ������ y��ǥ�� 1 ������ ��ȯ
        GameObject ItemInfo = PhotonNetwork.Instantiate("Prefab/Game/ItemInfo", new Vector3 (position.x,position.y + 1,position.z), rotation);
        
        string itemName = PrefabsNameToRealName(Prefabname) + ' ' + price.ToString() + '$';
        ItemInfo.transform.GetComponent<TMP_Text>().text = itemName;

        // ������ ��ȯ�� ��ǥ�κ��� 1f �̳��� ��� �ݶ��̴� ����, �װ� Player��� ���� ���� ��Ŭ���� ������ ���� ����
        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.tag == "Player")
                {
                    print("����!");
                    ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + itemInfo;

                    if (Input.GetMouseButtonDown(1))
                    {
                        if (totalMoney.GetComponent<MoneyManager>().totalMoney < price)
                        {
                            print("�� ����");
                            ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + "���� �����մϴ�!";
                        }
                        else
                        {
                            print("���� ��");
                            ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + "���� �Ϸ�!";
                            StartCoroutine(DestoryInfo(1f, ItemInfo));
                            totalMoney.GetComponent<MoneyManager>().moneyChange(-price);
                            SaleItem.transform.GetComponent<Block>().isAbleGrabed = true;
                            SaleItem.transform.GetComponent<Rigidbody2D>().isKinematic = false;
                            isSaled = true;
                            break;
                        }
                    }
                }
                else
                {
                    ItemInfo.transform.GetComponent<TMP_Text>().text = itemName;
                }
            }
            if (isSaled) break;
            yield return null;
        }
    }

    string PrefabsNameToRealName(string name)
    {
        if (name == "Brick") return "������";
        else if (name == "Brick2") return "��Ʈ������";
        else return "?????";
    }

    IEnumerator DestoryInfo(float delay, GameObject ItemInfo)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(ItemInfo);
    }
}
