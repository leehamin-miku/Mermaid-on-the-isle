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
            print("부딫힘");
            StartCoroutine(SummonSaleItems("Brick", new Vector3(85, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
            StartCoroutine(SummonSaleItems("Brick2", new Vector3(90, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            StartCoroutine(SummonSaleItems("Brick2", new Vector3(95, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            StartCoroutine(SummonSaleItems("Brick", new Vector3(100, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
        }
    }

    // prefab의 파일 이름, 소환할 좌표, 방향, 가격
    public IEnumerator SummonSaleItems(string Prefabname, Vector3 position, Quaternion rotation, int price, float radius)
    {
        // 상점에 물건 채울 타이밍을 못정해서 일단 시작 5초 후 생성

        bool isSaled = false;
        GameObject SaleItem = PhotonNetwork.Instantiate("Prefab/Game/" + Prefabname, position, rotation);
        SaleItem.transform.GetComponent<Block>().isAbleGrabed = false;
        SaleItem.transform.GetComponent<Rigidbody2D>().isKinematic = true;
        // 설명은 y좌표만 1 높여서 소환
        GameObject ItemInfo = PhotonNetwork.Instantiate("Prefab/Game/ItemInfo", new Vector3 (position.x,position.y + 1,position.z), rotation);
        
        string itemName = PrefabsNameToRealName(Prefabname) + ' ' + price.ToString() + '$';
        ItemInfo.transform.GetComponent<TMP_Text>().text = itemName;

        // 물건이 소환된 좌표로부터 1f 이내에 모든 콜라이더 감지, 그게 Player라면 설명 띄우고 우클릭을 누르면 구매 판정
        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.tag == "Player")
                {
                    print("접촉!");
                    ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + itemInfo;

                    if (Input.GetMouseButtonDown(1))
                    {
                        if (totalMoney.GetComponent<MoneyManager>().totalMoney < price)
                        {
                            print("돈 부족");
                            ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + "돈이 부족합니다!";
                        }
                        else
                        {
                            print("구매 완");
                            ItemInfo.transform.GetComponent<TMP_Text>().text = itemName + '\n' + "구매 완료!";
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
        if (name == "Brick") return "방파제";
        else if (name == "Brick2") return "테트라포드";
        else return "?????";
    }

    IEnumerator DestoryInfo(float delay, GameObject ItemInfo)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(ItemInfo);
    }
}
