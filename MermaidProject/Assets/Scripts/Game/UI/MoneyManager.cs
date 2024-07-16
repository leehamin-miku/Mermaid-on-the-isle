using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public int money;
    private void Start()
    {
        moneyDisplay.text = totalMoney.ToString();
    }
    public void MoneyNote(int amount)
    {
        totalMoney += amount;
        moneyDisplay.text = totalMoney.ToString();
    }

    [PunRPC]
    public bool MoneyChange(int a)
    {
        //������ Ŭ���̾�Ʈ���� ����Ǵ� �Լ�
        if (money >= a)
        {
            money += a;
            return true;
        } else
        {
            return false;
        }
    }

    [PunRPC]
    public void MoneyMark()
    {
        moneyDisplay.text = totalMoney.ToString();
    }

}
