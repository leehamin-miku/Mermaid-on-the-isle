using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class CraftTable : Block
{
    //마스터만 관리
    [SerializeField] Color colorBlock3;
    [SerializeField] Color colorBlock4;
    [SerializeField] Color colorBlock5;
    [SerializeField] Color colorBlock6;
    [SerializeField] Color colorBlock7;
    [SerializeField] Color colorBlock19;


    public List<int> inputList = new List<int>();
    public float a = 0;
    [SerializeField] List<int> SwordRecipe = new List<int>() {1, 2, 2};
    [SerializeField] List<int> RepairKitRecipe = new List<int>() { 3, 3, 7 };
    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&collision.collider.GetComponent<Block>().isGrabed&&inputList.Count < 3)
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
                    a = 0;
                    if (inputList.SequenceEqual(SwordRecipe))
                    {
                        Debug.Log("칼 제작 완료");
                        PhotonNetwork.Instantiate("Prefab/Game/Sword", transform.position - transform.up, transform.rotation);
                    } else if (inputList.SequenceEqual(RepairKitRecipe))
                    {
                        Debug.Log("칼 제작 완료");
                        PhotonNetwork.Instantiate("Prefab/Game/RepairKit", transform.position - transform.up, transform.rotation);
                    }
                    else
                    {
                        Debug.Log("레시피에 없는 제작법");
                    }
                    a = 0;
                    PV.RPC("InitIngredientMark", RpcTarget.AllBuffered);
                    inputList.Clear();
                }
            }
            yield return null;
        }
        
    }

    //전체실행
    [PunRPC]
    void InitIngredientMark()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.white;
    }

    //전체한테 실행
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

    public override Block DeepCopySub(Block block)
    {
        (block as CraftTable).inputList = inputList.ToList();
        return block;
    }
}
