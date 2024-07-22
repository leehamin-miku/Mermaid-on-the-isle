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
        Pl.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle*2,ForceMode2D.Impulse);
        if (GameObject.Find("LoadData0"))
        {

        }
    }

    void LoadObjects(int a)
    {

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

        DataManager.Instance.LoadGameData();
        Data.SaveStruct ss = DataManager.Instance.data.saveFile[a];
        GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money = ss.money;
        GameObject.Find("VN").GetComponent<VNManager>().nextDialogue = ss.nextDialogue;
        GameObject.Find("Shop").GetComponent<Shop>().shopItemList = ss.shopItemList.ToList();
        GameObject.Find("LobbySquare").GetComponent<GameStart>().progressStatus = ss.progressStatus;




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
                    break;
                case 2:
                    blockName = "Brick2";
                    break;
                case 3:
                    blockName = "Shell";
                    break;
                case 4:
                    blockName = "ShellBronze";
                    break;
                case 5:
                    blockName = "ShellGold";
                    break;
                case 6:
                    blockName = "ShellRuby";
                    break;
                case 7:
                    blockName = "Rock";
                    break;
                case 8:
                    blockName = "BM1";
                    break;
                case 9:
                    blockName = "BM1Blueprint";
                    break;
                case 10:
                    blockName = "BM2";
                    break;
                case 11:
                    blockName = "BM2Blueprint";
                    break;
                case 12:
                    blockName = "PiggyBank";
                    break;
                case 13:
                    blockName = "PiggyBankBlueprint";
                    break;
                case 14:
                    blockName = "CraftTable";
                    break;
                case 15:
                    blockName = "CraftTableBlueprint";
                    break;
                case 16:
                    blockName = "Hammer";
                    break;
                case 17:
                    blockName = "Sword";
                    break;
                case 18:
                    blockName = "RepairKit";
                    break;
                default:
                    blockName = null;
                    break;
            }
            GameObject go = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, block.block.transform.position, block.block.transform.rotation);
            Destroy(go.GetComponent<Block>());
            go.AddComponent<Block>()     = block.block as block.block.GetType();
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
