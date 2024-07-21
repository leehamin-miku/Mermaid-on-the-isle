using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoToTitle : MonoBehaviour
{
    // Start is called before the first frame update
    bool isTigger = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<PlayerController>().PV.IsMine)
        {
            GetComponent<TextMeshPro>().text = "Ÿ��Ʋ��\n���ư���\n��Ŭ��";
            isTigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0 && collision.GetComponent<PlayerController>().PV.IsMine)
        {
            GetComponent<TextMeshPro>().text = "Ÿ��Ʋ��\n���ư���";
            isTigger = false;
        }
    }

    private void Update()
    {
        if (isTigger)
        {
            if (Input.GetMouseButtonDown(1))
            {
                PhotonNetwork.LeaveRoom();
            }
        }
    }
}
