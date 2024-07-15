using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public int totalMoney;

    private void Start()
    {
        moneyDisplay.text = totalMoney.ToString();
    }
    public void moneyChange(int amount)
    {
        totalMoney += amount;
        moneyDisplay.text = totalMoney.ToString();
    }

}
