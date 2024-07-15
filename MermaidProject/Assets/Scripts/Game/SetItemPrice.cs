using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class SetItemPrice : MonoBehaviour
{
    public GameObject totalMoney;
    private TMP_Text displayItemInfo;
    public string itemInfo;
    string itemName;
    public int price;

    private void Start()
    {
        displayItemInfo = transform.GetChild(0).GetComponent<TMP_Text>();
        displayItemInfo.text += ' ' + price;
        itemName = displayItemInfo.text;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("Player가 들어옴");
            displayItemInfo.text += '\n' + itemInfo;
            StartCoroutine(buyItem());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("Player가 나감");
            displayItemInfo.text = itemName;
        }
    }

    IEnumerator delayTime(float n)
    {
        yield return new WaitForSeconds(n);
    }

    IEnumerator buyItem()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (totalMoney.GetComponent<MoneyManager>().totalMoney < price)
                {
                    print("돈 부족");
                    displayItemInfo.text = itemName + '\n' + "돈이 부족합니다!";
                    StartCoroutine(delayTime(1));
                    displayItemInfo.text = itemName + '\n' + itemInfo;
                }
                else
                {
                    print("구매 완");
                    displayItemInfo.text = itemName + '\n' + "구매 완료!";
                    totalMoney.GetComponent<MoneyManager>().moneyChange(-price);
                }
                break;
            }
            yield return null;
        }
    }
    
}
