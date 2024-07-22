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
        
        List<Data.SaveBlockStruct> saveBlockList = new List<Data.SaveBlockStruct>();
        Transform saveGroup = GameObject.Find("SaveObjectGroup").transform;
        foreach (Transform child in saveGroup)
        {
            saveBlockList.Add(new Data.SaveBlockStruct(child.GetComponent<Block>().BlockCode, child.GetComponent<Block>()));
        //    int a = block.BlockCode;
        //    string blockName;
        //    //            0 player
        //    //1 brick1
        //    //2 brick2
        //    //3 Shell
        //    //4 ShellBronze
        //    //5ShellGold
        //    //6 ShellRuby
        //    //7 Rock
        //    //8 BM1
        //    //9 BM1Blueprint
        //    //10 BM2
        //    //11 BM2Blueprint
        //    //12 PiggyBank
        //    //13 PiggyBankBlueprint
        //    //14 CraftTable
        //    //15 CraftTableBlueprint
        //    //16 Hammer
        //    //17 Sword
        //    //18 RepairKit
        //    switch (a)
        //    {

        //        case 0:
        //            //플레이어인데 이게 왜 저장되는거야
        //            break;
        //        case 1:
        //            blockName = "Brick1";
        //            break;
        //        case 2:
        //            blockName = "Brick2";
        //            break;
        //        case 3:
        //            blockName = "Shell";
        //            break;
        //        case 4:
        //            blockName = "ShellBronze";
        //            break;
        //        case 5:
        //            blockName = "ShellGold";
        //            break;
        //        case 6:
        //            blockName = "ShellRuby";
        //            break;
        //        case 7:
        //            blockName = "Rock";
        //            break;
        //        case 8:
        //            blockName = "BM1";
        //            break;
        //        case 9:
        //            blockName = "BM1Blueprint";
        //            break;
        //        case 10:
        //            blockName = "BM2";
        //            break;
        //        case 11:
        //            blockName = "BM2Blueprint";
        //            break;
        //        case 12:
        //            blockName = "PiggyBank";
        //            break;
        //        case 13:
        //            blockName = "PiggyBankBlueprint";
        //            break;
        //        case 14:
        //            blockName = "CraftTable";
        //            break;
        //        case 15:
        //            blockName = "CraftTableBlueprint";
        //            break;
        //        case 16:
        //            blockName = "Hammer";
        //            break;
        //        case 17:
        //            blockName = "Sword";
        //            break;
        //        case 18:
        //            blockName = "RepairKit";
        //            break;
        //        default:
        //            blockName = null;
        //            break;
        //    }

        }
        ss.saveBlockList = saveBlockList.ToList();
        DataManager.Instance.data.saveFile[a] = ss;
        DataManager.Instance.SaveGameData();
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
