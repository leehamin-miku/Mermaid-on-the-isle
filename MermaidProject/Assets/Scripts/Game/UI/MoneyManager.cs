using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public PhotonView PV;
    //저장대상
    public int money = 0; //모든 플레이어가 같은 money를 가지고 마스터를 기준으로 갱신
    
    public void Start()
    {
        PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void MoneyChange(int a)
    {
        //마스터 클라이언트에서 실행되는 함수
        if (PhotonNetwork.IsMasterClient)
        {
            money += a;
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }

    [PunRPC]
    public void MoneyMark(int money)
    {
        //돈을 갱신하고, mark에 표시함
        this.money = money;
        moneyDisplay.text = "소유금액"+money.ToString()+"$";
    }


    [PunRPC]
    public void MoneyMarkRequest()
    {
        //마스터클라이언트에게 요청하는 함수 RPCtaget.master 어쩌구
        //모든이가 동일한 money값을 가지고 있는것은 맞지만, 특히 마스터의 것을 참고
        if (PhotonNetwork.IsMasterClient) //2차 확인
        {
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }
}
