using EasyTransition;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviourPunCallbacks
{
    public AudioMixer audioMixer;
    void Start()
    {
        GameObject Pl = PhotonNetwork.Instantiate("Prefab/Game/Player", new Vector3(-85, -121), Quaternion.identity);
        Pl.GetComponent<Rigidbody2D>().AddForce(UnityEngine.Random.insideUnitCircle*2,ForceMode2D.Impulse);
        if (DataManager.Instance.data.MV == -40f) audioMixer.SetFloat("Master", -80);
        else
        {
            audioMixer.SetFloat("Master", DataManager.Instance.data.MV);
        }
        if (DataManager.Instance.data.BV == -40f) audioMixer.SetFloat("BGM", -80);
        else
        {
            audioMixer.SetFloat("BGM", DataManager.Instance.data.BV);
        }
        if (DataManager.Instance.data.SV == -40f) audioMixer.SetFloat("SFX", -80);
        else
        {
            audioMixer.SetFloat("SFX", DataManager.Instance.data.SV);
        }

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



        DataManager.Instance.LoadGameData();
        Data.SaveStruct ss = DataManager.Instance.data.saveFile[a];
        GameObject.Find("TotalMoney").GetComponent<MoneyManager>().money = ss.money;
        GameObject.Find("TotalMoney").GetComponent<MoneyManager>().PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
        GameObject.Find("VN").GetComponent<VNManager>().nextDialogue = ss.nextDialogue;
        GameObject.Find("Shop").GetComponent<Shop>().shopItemList = ss.shopItemList.ToList();
        GameObject.Find("LobbySquare").GetComponent<GameStart>().progressStatus = ss.progressStatus;
        GameObject.Find("VN").GetComponent<VNManager>().textbookList = ss.textbookList.ToList();
        GameObject.Find("ShopParent").transform.GetChild(0).gameObject.SetActive(ss.shopOpened);

        
        GameObject sog = GameObject.Find("SaveObjectGroup");
        List<Data.SaveBlockStruct> blockList = ss.saveBlockList.ToList();
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
                    Block brick1 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    brick1.strength = block.strength;
                    brick1.transform.SetParent(sog.transform);
                    break;
                case 2:
                    blockName = "Brick2";
                    Block brick2 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    brick2.strength = block.strength;
                    brick2.transform.SetParent(sog.transform);
                    break;
                case 3:
                    blockName = "Shell";
                    Block shell = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    shell.strength = block.strength;
                    shell.transform.SetParent(sog.transform);
                    break;
                case 4:
                    blockName = "ShellBronze";
                    Block shellBronze = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    shellBronze.strength = block.strength;
                    shellBronze.transform.SetParent(sog.transform);
                    break;
                case 5:
                    blockName = "ShellGold";
                    Block shellGold = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    shellGold.strength = block.strength;
                    shellGold.transform.SetParent(sog.transform);
                    break;
                case 6:
                    blockName = "ShellRuby";
                    Block shellRuby = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    shellRuby.strength = block.strength;
                    shellRuby.transform.SetParent(sog.transform);
                    break;
                case 7:
                    blockName = "Rock";
                    Block rock = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    rock.strength = block.strength;
                    rock.transform.SetParent(sog.transform);
                    break;
                case 8:
                    blockName = "BM1";
                    BM1 bm1 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<BM1>();
                    bm1.strength = block.strength;
                    bm1.a = block.a;
                    bm1.waitingLen1 = block.w1;
                    bm1.waitingLen2 = block.w2;
                    bm1.transform.SetParent(sog.transform);
                    break;
                case 9:
                    blockName = "BM1Blueprint";
                    Block BM1Blueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    BM1Blueprint.strength = block.strength;
                    BM1Blueprint.transform.SetParent(sog.transform);
                    break;
                case 10:
                    blockName = "BM2";
                    BM2 bm2 = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<BM2>();
                    bm2.strength = block.strength;
                    bm2.a = block.a;
                    bm2.waitingLen1 = block.w1;
                    bm2.waitingLen2 = block.w2;
                    bm2.transform.SetParent(sog.transform);
                    break;
                case 11:
                    blockName = "BM2Blueprint";
                    Block BM2Blueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    BM2Blueprint.strength = block.strength;
                    BM2Blueprint.transform.SetParent(sog.transform);
                    break;
                case 12:
                    blockName = "PiggyBank";
                    Block piggyBank = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    piggyBank.strength = block.strength;
                    piggyBank.transform.SetParent(sog.transform);
                    break;
                case 13:
                    blockName = "PiggyBankBlueprint";
                    Block piggyBankBlueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<Block>();
                    piggyBankBlueprint.strength = block.strength;
                    piggyBankBlueprint.transform.SetParent(sog.transform);
                    break;
                case 14:
                    blockName = "CraftTable";
                    CraftTable craftTable = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[2]), block.q).GetComponent<CraftTable>();
                    craftTable.strength = block.strength;
                    List<int> list = new List<int>();
                    if (block.w1 != 0)
                    {
                        list.Add(block.w1);
                    }
                    if (block.w2 != 0)
                    {
                        list.Add(block.w2);
                    }
                    if (block.w3 != 0)
                    {
                        list.Add(block.w3);
                    }
                    craftTable.inputList = list.ToList();
                    craftTable.LoadFuc();
                    break;
                case 15:
                    blockName = "CraftTableBlueprint";
                    Block craftTableBlueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Block>();
                    craftTableBlueprint.strength = block.strength;
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
                    RepairKit repairKit = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<RepairKit>();
                    repairKit.strength = block.strength;
                    repairKit.a = block.w1;
                    repairKit.transform.SetParent(sog.transform);
                    break;
                case 19:
                    blockName = "Wood";
                    Block wood = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Block>();
                    wood.strength = block.strength;
                    wood.transform.SetParent(sog.transform);
                    break;
                case 20:
                    blockName = "SwordGold";
                    Sword sg = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Sword>();
                    sg.strength = block.strength;
                    sg.Durability = block.w1;
                    sg.transform.SetParent(sog.transform);
                    break;
                case 21:
                    blockName = "SwordRuby";
                    Sword sr = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Sword>();
                    sr.strength = block.strength;
                    sr.Durability = block.w1;
                    sr.transform.SetParent(sog.transform);
                    break;
                case 22:
                    blockName = "Ingot";
                    Block ingot = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Block>();
                    ingot.strength = block.strength;
                    ingot.transform.SetParent(sog.transform);
                    break;
                case 23:
                    blockName = "Cannon";
                    Cannon cannon = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Cannon>();
                    cannon.strength = block.strength;
                    cannon.transform.SetParent(sog.transform);
                    break;
                case 24:
                    blockName = "CannonBlueprint";
                    Block cannonBlueprint = PhotonNetwork.Instantiate("Prefab/Game/" + blockName, new Vector3(block.b[0], block.b[1], block.b[3]), block.q).GetComponent<Block>();
                    cannonBlueprint.strength = block.strength;
                    cannonBlueprint.transform.SetParent(sog.transform);
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
