using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MoneyManager : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public PhotonView PV;
    //������
    public int money = 0; //��� �÷��̾ ���� money�� ������ �����͸� �������� ����
    
    public void Start()
    {
        PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void MoneyChange(int a)
    {
        //������ Ŭ���̾�Ʈ���� ����Ǵ� �Լ�
        if (PhotonNetwork.IsMasterClient)
        {
            money += a;
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }

    [PunRPC]
    public void MoneyMark(int money)
    {
        //���� �����ϰ�, mark�� ǥ����
        this.money = money;
        moneyDisplay.text = "�����ݾ�"+money.ToString()+"$";
    }


    [PunRPC]
    public void MoneyMarkRequest()
    {
        //������Ŭ���̾�Ʈ���� ��û�ϴ� �Լ� RPCtaget.master ��¼��
        //����̰� ������ money���� ������ �ִ°��� ������, Ư�� �������� ���� ����
        if (PhotonNetwork.IsMasterClient) //2�� Ȯ��
        {
            PV.RPC("MoneyMark", RpcTarget.All, money);
        }
    }
}
