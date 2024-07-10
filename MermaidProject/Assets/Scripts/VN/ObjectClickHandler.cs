using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    public TMP_Text AnswerText;
    public string OutputValue;
    [SerializeField] PhotonView PV;

    public static event Action<string> OnObjectClicked;


    public void ChangeOutputValue(string outputvalue)
    {
        OutputValue = outputvalue;
    }

    private void OnMouseDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("DownAction", RpcTarget.All);
        }
    }



    private void ExecuteOnClick()
    {
        Debug.Log(OutputValue);
        OnObjectClicked?.Invoke(OutputValue);
    }

    private void OnMouseEnter()
    {
        AnswerText.color = Color.red;
    }

    private void OnMouseExit()
    {
        AnswerText.color = Color.white;

    }

    [PunRPC]
    public void DownAction()
    {
        ExecuteOnClick();
    }
    
}
