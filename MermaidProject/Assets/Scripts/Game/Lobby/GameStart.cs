using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    List<PlayerController> list = new List<PlayerController>();

    public int progressStatus = 1;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&collision.GetComponent<Block>().BlockCode==0)
        {
            list.Add(collision.GetComponent<PlayerController>());
            if (PhotonNetwork.CurrentRoom.PlayerCount == list.Count && DuplicationCheck(list) == false)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                progressStatus++;
                GameObject.Find("VN").GetComponent<VNManager>().StartNextDialogue();
                GameObject.Find("TotalMoney").GetPhotonView().RPC("MoneyChange", RpcTarget.MasterClient, 0);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.GetComponent<Block>().BlockCode == 0)
        {
            list.Remove(collision.GetComponent<PlayerController>());
        }
    }

    private bool DuplicationCheck(List<PlayerController> list)
    {
        for(int i=0; i<list.Count; i++)
        {
            for(int j = i+1; j<list.Count; j++)
            {
                if (list[j].colorNumber == list[i].colorNumber)
                {
                    return true;
                }
            }
            
        }
        return false;
    }
    //public void ExecuteTidalGame()
    //{
    //    StartCoroutine(TidalGame());
    //}

    //IEnumerator TidalGame()
    //{
    //    for (int i = 0; i < defaultTime; i++)
    //    {
    //        GameObject.Find("Timer").GetComponent<TextMeshProUGUI>().text = "0" + (defaultTime - i) / 60 + ":" + (defaultTime - i) % 60;
    //        yield return new WaitForSeconds(1f);
    //    }
    //    GetComponent<TsunamiController>().StartTsunami();
    //}
}
