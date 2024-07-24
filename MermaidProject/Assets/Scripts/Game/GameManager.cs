using EasyTransition;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(-85, -121), Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(UnityEngine.Random.insideUnitCircle*2,ForceMode2D.Impulse);
        if (GameObject.Find("LoadData0"))
        {
            Destroy(GameObject.Find("LoadData0"));
            LoadObjects(0);
        } else if (GameObject.Find("LoadData1"))
        {
            Destroy(GameObject.Find("LoadData1"));
            LoadObjects(1);
        } else if (GameObject.Find("LoadData2"))
        {
            Destroy(GameObject.Find("LoadData2"));
            LoadObjects(2);
        }
    }

    void LoadObjects(int a)
    {

        List<Data.SaveBlockStruct> saveBlockList = new List<Data.SaveBlockStruct>();
        Transform saveGroup = GameObject.Find("SaveObjectGroup").transform;
        foreach (Transform child in saveGroup)
        {
            saveBlockList.Add(new Data.SaveBlockStruct(child.GetComponent<Block>().BlockCode, child.GetComponent<Block>()));

        }

        DataManager.Instance.LoadGameData();
        Data.SaveStruct ss = DataManager.Instance.data.saveFile[a];
        GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money = ss.money;
        GameObject.Find("VN").GetComponent<VNManager>().nextDialogue = ss.nextDialogue;
        GameObject.Find("Shop").GetComponent<Shop>().shopItemList = ss.shopItemList.ToList();
        GameObject.Find("LobbySquare").GetComponent<GameStart>().progressStatus = ss.progressStatus;
        GameObject.Find("VN").GetComponent<VNManager>().textbookList = ss.textbookList.ToList();



        GameObject sog = GameObject.Find("SaveObjectGroup");
        List<Data.SaveBlockStruct> blockList = ss.saveBlockList.ToList();
        //노가다 ㄱㄱ 밥먹고옴
        foreach (Data.SaveBlockStruct block in blockList)
        {
            string blockName = "";
            switch (block.blockCode)
            {

                case 0:
                    //플레이어인데 이게 왜 저장되는거야
                    break;
                case 1:
                    blockName = "Brick1";
                    Block brick1 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    brick1.strength = block.block.strength;
                    brick1.transform.SetParent(sog.transform);
                    break;
                case 2:
                    blockName = "Brick2";
                    Block brick2 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    brick2.strength = block.block.strength;
                    brick2.transform.SetParent(sog.transform);
                    break;
                case 3:
                    blockName = "Shell";
                    Block shell = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    shell.strength = block.block.strength;
                    shell.transform.SetParent(sog.transform);
                    break;
                case 4:
                    blockName = "ShellBronze";
                    Block shellBronze = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    shellBronze.strength = block.block.strength;
                    shellBronze.transform.SetParent(sog.transform);
                    break;
                case 5:
                    blockName = "ShellGold";
                    Block shellGold = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    shellGold.strength = block.block.strength;
                    shellGold.transform.SetParent(sog.transform);
                    break;
                case 6:
                    blockName = "ShellRuby";
                    Block shellRuby = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    shellRuby.strength = block.block.strength;
                    shellRuby.transform.SetParent(sog.transform);
                    break;
                case 7:
                    blockName = "Rock";
                    Block rock = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    rock.strength = block.block.strength;
                    rock.transform.SetParent(sog.transform);
                    break;
                case 8:
                    blockName = "BM1";
                    BM1 bm1 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<BM1>();
                    bm1.strength = block.block.strength;
                    bm1.a = (block.block as BM1).a;
                    bm1.waitingLen = (block.block as BM1).waitingLen;
                    bm1.transform.SetParent(sog.transform);
                    break;
                case 9:
                    blockName = "BM1Blueprint";
                    Block BM1Blueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    BM1Blueprint.strength = block.block.strength;
                    BM1Blueprint.transform.SetParent(sog.transform);
                    break;
                case 10:
                    blockName = "BM2";
                    BM2 bm2 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<BM2>();
                    bm2.strength = block.block.strength;
                    bm2.a = (block.block as BM2).a;
                    bm2.waitingLen = (block.block as BM2).waitingLen;
                    bm2.transform.SetParent(sog.transform);
                    break;
                case 11:
                    blockName = "BM2Blueprint";
                    Block BM2Blueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    BM2Blueprint.strength = block.block.strength;
                    BM2Blueprint.transform.SetParent(sog.transform);
                    break;
                case 12:
                    blockName = "PiggyBank";
                    Block piggyBank = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    piggyBank.strength = block.block.strength;
                    piggyBank.transform.SetParent(sog.transform);
                    break;
                case 13:
                    blockName = "PiggyBankBlueprint";
                    Block piggyBankBlueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    piggyBankBlueprint.strength = block.block.strength;
                    piggyBankBlueprint.transform.SetParent(sog.transform);
                    break;
                case 14:
                    blockName = "CraftTable";
                    CraftTable craftTable = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<CraftTable>();
                    craftTable.strength = block.block.strength;
                    craftTable.inputList = (block.block as CraftTable).inputList.ToList();
                    craftTable.LoadFuc();
                    break;
                case 15:
                    blockName = "CraftTableBlueprint";
                    Block craftTableBlueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    craftTableBlueprint.strength = block.block.strength;
                    craftTableBlueprint.transform.SetParent(sog.transform);
                    break;
                case 16:
                    blockName = "Hammer";
                    break;
                case 17:
                    blockName = "Sword";
                    break;
                case 18:
                    blockName = "RepairKit";
                    RepairKit repairKit = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<RepairKit>();
                    repairKit.strength = block.block.strength;
                    repairKit.a = (block.block as RepairKit).a;
                    repairKit.transform.SetParent(sog.transform);
                    break;
                case 19:
                    blockName = "Wood";
                    Block wood = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.savePosition, block.block.saveRotation).GetComponent<Block>();
                    wood.strength = block.block.strength;
                    wood.transform.SetParent(sog.transform);
                    break;
                default:
                    blockName = null;
                    break;
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        GameObject go = new GameObject();
        go.name = "RoomExplode";
        DontDestroyOnLoad(go);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        GameObject.Find("GameManager").GetComponent<ChattingManager>().SystemChatting("<color=yellow>" + otherPlayer.NickName + "님이 떠났습니다</color>");

        base.OnPlayerLeftRoom(otherPlayer);
        
    }

    public override void OnLeftRoom()
    {
        // This will be called when the client leaves the room
        // Optionally, load another scene or handle post-room-leave logic
        if (PhotonNetwork.IsMasterClient)
        {

        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }
}
