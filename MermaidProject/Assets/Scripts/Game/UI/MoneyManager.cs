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
        //마스터 클라이언트에서 실행되는 함수
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
