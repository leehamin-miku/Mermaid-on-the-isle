using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShopRefill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            print("����");
            // Ư�� ���Ǹ��� ���ʵǴ� �Ŷ�� �� ������ ������ �ִ� ��� ���� �ʱ�ȭ ���� �ʿ��� ��?
            //StartCoroutine(GameObject.Find("summonItems").GetComponent<Shop>().SummonSaleItems("Brick", new Vector3(85, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
            //StartCoroutine(GameObject.Find("summonItems").GetComponent<Shop>().SummonSaleItems("Brick2", new Vector3(90, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            //StartCoroutine(GameObject.Find("summonItems").GetComponent<Shop>().SummonSaleItems("Brick2", new Vector3(95, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1.5f));
            //StartCoroutine(GameObject.Find("summonItems").GetComponent<Shop>().SummonSaleItems("Brick", new Vector3(100, -142, -1), new Quaternion(0, 0, 0, 1), 5, 1f));
        }
    }
}
