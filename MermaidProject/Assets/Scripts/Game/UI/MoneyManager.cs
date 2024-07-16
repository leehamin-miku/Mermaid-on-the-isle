using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public PhotonView PV;
    public int money = 0; //��� �÷��̾ money�� �������� �����Ϳ� �͸� �����
    public void Start()
    {
        PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
    }

    [PunRPC]
    public bool MoneyChange(int a)
    {
        //������ Ŭ���̾�Ʈ���� ����Ǵ� �Լ�
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
        //������Ŭ���̾�Ʈ���� �ٸ� PV�� �ѱ�� �Լ�
        moneyDisplay.text = money.ToString();
    }

    [PunRPC]
    public void MoneyMarkRequest()
    {
        //������Ŭ���̾�Ʈ���� ��û�ϴ� �Լ� RPCtaget.master ��¼��
        if (PhotonNetwork.IsMasterClient) //2�� Ȯ��
        {
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }
}
