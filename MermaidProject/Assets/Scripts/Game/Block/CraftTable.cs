using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CraftTable : Block
{
    //�����͸� ����
    [SerializeField] Color colorBlock1;
    [SerializeField] Color colorBlock2;
    [SerializeField] Color colorBlock3;
    [SerializeField] Color colorBlock4;
    [SerializeField] Color colorBlock5;


    [SerializeField] List<int> inputList = new List<int>();
    bool isMaking = false;
    float a = 0;
    [SerializeField] List<int> SwordRecipe = new List<int>() {1, 2, 2};
    public override void CollisionEnterAction(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient&&collision.collider.GetComponent<Block>().isGrabed&&inputList.Count < 3)
        {
            if(collision.collider.GetComponent<Block>().BlockCode == 1 || 
                collision.collider.GetComponent<Block>().BlockCode == 2 || 
                collision.collider.GetComponent<Block>().BlockCode == 3)
            {
                inputList.Add(collision.collider.GetComponent<Block>().BlockCode);
                PV.RPC("IngredientMarkChange", RpcTarget.All, inputList.Count - 1, collision.collider.GetComponent<Block>().BlockCode);
                collision.gameObject.GetComponent<Block>().PV.RPC("DestroyFuc", RpcTarget.All);
                if (inputList.Count == 3)
                {
                    StartCoroutine(MakeItem());
                }
            }
        }
    }

    IEnumerator MakeItem()
    {
        inputList.Sort();
        while (a < 10f&&isRunning){
            transform.GetChild(5).Rotate(new Vector3(0, 0, Time.deltaTime * 200));
            a += Time.deltaTime;
            yield return null;
        }

        if (inputList.SequenceEqual(SwordRecipe))
        {
            Debug.Log("Į ���� �Ϸ�");
            PhotonNetwork.Instantiate("Prefab/Game/Sword", transform.position - transform.up, transform.rotation);
        }
        else
        {
            Debug.Log("�����ǿ� ���� ���۹�");
        }
        a = 0;
        PV.RPC("InitIngredientMark", RpcTarget.All);
        inputList.Clear();
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
            case 1:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock1;
                break;
            case 2:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock2;
                break;
            case 3:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock3;
                break;
            case 4:
                transform.GetChild(a).GetComponent<SpriteRenderer>().color = colorBlock4;
                break;
        }
    }
}
