using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Save : MonoBehaviour
{
    [SerializeField] int a; //0, 1, 2
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadData();
        } else
        {
            gameObject.SetActive(false);
        }
        
    }
    private void LoadData()
    {
        DataManager.Instance.LoadGameData();
        Data.SaveStruct ss = DataManager.Instance.data.saveFile[a];
        if (ss.roomName == null)
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = "비어있음";
            transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
        } else
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = ss.roomName + "\nD-" + ss.progressStatus;
            transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
        }
        
    }

    //마스터만 보이고 실행가능
    public void SavePresentData()
    {
        DataManager.Instance.LoadGameData();
        Data.SaveStruct ss = new Data.SaveStruct();
        ss.money = GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money;
        ss.nextDialogue = GameObject.Find("VN").GetComponent<VNManager>().nextDialogue;
        ss.shopItemList = GameObject.Find("Shop").GetComponent<Shop>().shopItemList.ToList();
        ss.roomName = PhotonNetwork.CurrentRoom.Name;
        ss.progressStatus = GameObject.Find("LobbySquare").GetComponent<GameStart>().progressStatus;
        ss.textbookList = GameObject.Find("VN").GetComponent<VNManager>().textbookList.ToList();
        List<Data.SaveBlockStruct> saveBlockList = new List<Data.SaveBlockStruct>();
        Transform saveGroup = GameObject.Find("SaveObjectGroup").transform;
        foreach (Transform child in saveGroup)
        {

            saveBlockList.Add(new Data.SaveBlockStruct(child.GetComponent<Block>().BlockCode, child.GetComponent<Block>().DeepCopy()));

        }
        ss.saveBlockList = saveBlockList.ToList();
        Data data = DataManager.Instance.data;
        data.saveFile[a] = ss;
        DataManager.Instance.SaveGameData(data);
    }

    bool isTigger = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<PlayerController>().PV.IsMine)
        {
            transform.GetChild(1).GetComponent<TextMeshPro>().text = "덮어쓰기 우클릭";
            isTigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<PlayerController>().PV.IsMine)
        {
            transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
            isTigger = false;
        }
    }

    private void Update()
    {
        if (isTigger)
        {
            if (Input.GetMouseButtonDown(1))
            {
                SavePresentData();
                LoadData();
            }
        }
    }
}
