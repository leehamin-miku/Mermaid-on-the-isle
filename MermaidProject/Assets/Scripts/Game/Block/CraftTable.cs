using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static Data;
using static Unity.Burst.Intrinsics.X86;


public class CraftTable : Block
{
    //�����͸� ����
    [SerializeField] Color colorBlock3;
    [SerializeField] Color colorBlock4;
    [SerializeField] Color colorBlock5;
    [SerializeField] Color colorBlock6;
    [SerializeField] Color colorBlock7;
    [SerializeField] Color colorBlock19;

    bool isCool = false;

    public List<int> inputList = new List<int>();
    public float a = 0;
    [SerializeField] List<int> SwordGoldRecipe = new List<int>() {5, 5, 19};
    [SerializeField] List<int> SwordRubyRecipe = new List<int>() {6, 6, 19};
    [SerializeField] List<int> RepairKitRecipe = new List<int>() { 3, 3, 7 };
    [SerializeField] List<int> IngotRecipe = new List<int>() {4, 4, 4};
    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&collision.collider.GetComponent<Block>().isGrabed&&inputList.Count < 3&&isCool==false)
        {
            if(collision.collider.GetComponent<Block>().BlockCode == 7 || 
                collision.collider.GetComponent<Block>().BlockCode == 19 || 
                collision.collider.GetComponent<Block>().BlockCode == 3||
                collision.collider.GetComponent<Block>().BlockCode == 4||
                collision.collider.GetComponent<Block>().BlockCode == 5||
                collision.collider.GetComponent<Block>().BlockCode == 6)
            {
                inputList.Add(collision.collider.GetComponent<Block>().BlockCode);
                inputList.Sort();
                StartCoroutine(CoolDown(0.5f));
                PV.RPC("IngredientMarkChange", RpcTarget.AllBuffered, inputList.Count - 1, collision.collider.GetComponent<Block>().BlockCode);
                collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
            }
        }
    }

    public override IEnumerator RunningCoroutine()
    {
        while (true)
        {
            
            if (inputList.Count >= 3)
            {
                a += Time.deltaTime;
                
                transform.GetChild(5).Rotate(new Vector3(0, 0, Time.deltaTime * 200));
                if (a >= 20f)
                {
                    a = 0f;
                    if (inputList.SequenceEqual(SwordGoldRecipe))
                    {
                        PhotonNetwork.Instantiate("Prefab/Game/SwordGold", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                    } else if (inputList.SequenceEqual(SwordRubyRecipe))
                    {
                        PhotonNetwork.Instantiate("Prefab/Game/SwordRuby", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                    }
                    else if (inputList.SequenceEqual(RepairKitRecipe))
                    {
                        PhotonNetwork.Instantiate("Prefab/Game/RepairKit", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                    }
                    else if (inputList.SequenceEqual(IngotRecipe))
                    {
                        PhotonNetwork.Instantiate("Prefab/Game/Ingot", transform.position - transform.up, transform.rotation).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                    }
                    else
                    {
                        Debug.Log("�����ǿ� ���� ���۹�");
                    }
                    PV.RPC("InitIngredientMark", RpcTarget.AllBuffered);
                    inputList.Clear();
                }
            }
            yield return null;
        }
        
    }

    //��ü����
    [PunRPC]
    void InitIngredientMark()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.white;
    }

    //��ü���� ����
    [PunRPC]
    void IngredientMarkChange(int a, int b)
    {
        switch (b)
        {
            case 3:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock3;
                break;
            case 4:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock4;
                break;
            case 5:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock5;
                break;
            case 6:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock6;
                break;
            case 7:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock7;
                break;
            case 19:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock19;
                break;
        }
    }

    public void LoadFuc()
    {
        for(int i=0; i<inputList.Count; i++)
        {
            PV.RPC("IngredientMarkChange", RpcTarget.AllBuffered, i, inputList[i]);
        }
    }

    public override SaveBlockStruct DeepCopySub(SaveBlockStruct block)
    {
        if (inputList.Count == 0)
        {
            block.w1 = 0;
            block.w2 = 0;
            block.w3 = 0;
        }
        else if (inputList.Count == 1)
        {
            block.w1 = inputList[0];
            block.w2 = 0;
            block.w3 = 0;
        }
        else if (inputList.Count == 2)
        {
            block.w1 = inputList[0];
            block.w2 = inputList[1];
            block.w3 = 0;
        }
        else if (inputList.Count == 3)
        {
            block.w1 = inputList[0];
            block.w2 = inputList[1];
            block.w3 = inputList[2];
        }
        return block;
    }
    public IEnumerator CoolDown(float a)
    {
        isCool = true;
        float b = 0;
        while (b <= a)
        {
            b += Time.deltaTime;
            yield return null;
        }
        isCool = false;
    }
}
