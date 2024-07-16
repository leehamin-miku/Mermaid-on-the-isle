using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public PhotonView PV;
    public int money = 0; //모든 플레이어가 money를 가지지만 마스터에 것만 사용함
    public void Start()
    {
        PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
    }

    [PunRPC]
    public bool MoneyChange(int a)
    {
        //마스터 클라이언트에서 실행되는 함수
        if (money +a >=0)
        {
            money += a;
            PV.RPC("MoneyMark", RpcTarget.All, money);
            return true;
        } else
        {
            return false;
        }
    }

    [PunRPC]
    public void MoneyMark(int money)
    {
        //마스터클라이언트에서 다른 PV로 넘기는 함수
        moneyDisplay.text = money.ToString();
    }

    [PunRPC]
    public void MoneyMarkRequest()
    {
        //마스터클라이언트에게 요청하는 함수 RPCtaget.master 어쩌구
        if (PhotonNetwork.IsMasterClient) //2차 확인
        {
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }
}
