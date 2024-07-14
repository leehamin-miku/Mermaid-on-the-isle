using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChattingManager : MonoBehaviour
{
    [SerializeField] private PhotonView PV;
    [SerializeField] private TMP_Text chatDisplay;
    [SerializeField] private TMP_InputField chatInput;

    public bool isFocused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isFocused)
            {
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject, null);
                chatInput.OnPointerClick(new PointerEventData(EventSystem.current));
                isFocused = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(chatInput.text))
                {
                    SendMessage(chatInput.text);
                    chatInput.text = ""; // 입력 필드를 비웁니다.\
                }
                EventSystem.current.SetSelectedGameObject(null);
                isFocused = false;
            }
        }
    }
    // Start is called before the first frame update

    
    

    [PunRPC]
    void Chatting(string context)
    {
        print("a");
        chatDisplay.text += context + '\n';
    }

    void SendMessage(string context)
    {
        print("a");
        PV.RPC("Chatting", RpcTarget.All, context);
    }
}
