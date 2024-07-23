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
            LoadObjects(0);
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
        GameObject.Find("VN").GetComponent<VNManager>().;



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
                    PhotonNetwork.Instantiate("Prefab/Game/"+blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 2:
                    blockName = "Brick2";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 3:
                    blockName = "Shell";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 4:
                    blockName = "ShellBronze";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 5:
                    blockName = "ShellGold";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 6:
                    blockName = "ShellRuby";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 7:
                    blockName = "Rock";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 8:
                    blockName = "BM1";
                    BM1 bm1 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<BM1>();
                    bm1.strength = block.block.strength;
                    bm1.a = (block.block as BM1).a;
                    bm1.waitingLen = (block.block as BM1).waitingLen;
                    break;
                case 9:
                    blockName = "BM1Blueprint";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 10:
                    blockName = "BM2";
                    BM2 bm2 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<BM2>();
                    bm2.strength = block.block.strength;
                    bm2.a = (block.block as BM2).a;
                    bm2.waitingLen = (block.block as BM2).waitingLen;
                    break;
                case 11:
                    blockName = "BM2Blueprint";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 12:
                    blockName = "PiggyBank";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 13:
                    blockName = "PiggyBankBlueprint";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 14:
                    blockName = "CraftTable";
                    CraftTable craftTable = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<CraftTable>();
                    craftTable.strength = block.block.strength;
                    craftTable.inputList = (block.block as CraftTable).inputList.ToList();
                    craftTable.LoadFuc();
                    break;
                case 15:
                    blockName = "CraftTableBlueprint";
                    PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<Block>().strength = block.block.strength;
                    break;
                case 16:
                    blockName = "Hammer";
                    break;
                case 17:
                    blockName = "Sword";
                    break;
                case 18:
                    blockName = "RepairKit";
                    RepairKit repairKit = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation).GetComponent<RepairKit>();
                    repairKit.a = (block.block as RepairKit).a;
                    break;
                default:
                    blockName = null;
                    break;
            }
        }
    }
    void CopyComponentValues(Block source, Block destination)
    {
        if (source == null || destination == null)
        {
            Debug.LogError("Source or destination is null");
            return;
        }

        // Reflection을 사용하여 모든 필드를 복사
        FieldInfo[] fields = typeof(Block).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            field.SetValue(destination, field.GetValue(source));
        }

        // 필요시, 모든 프로퍼티를 복사
        PropertyInfo[] properties = typeof(Block).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            if (property.CanRead && property.CanWrite)
            {
                property.SetValue(destination, property.GetValue(source, null), null);
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
