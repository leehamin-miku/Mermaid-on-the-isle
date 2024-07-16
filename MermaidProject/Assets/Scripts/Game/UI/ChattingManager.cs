using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChattingManager : MonoBehaviour
{

    public PlayerController PlayerController;
    [SerializeField] public PhotonView PV;
    [SerializeField] private TMP_Text chatDisplay;
    [SerializeField] private TMP_InputField chatInput;

    [SerializeField] public bool isFocused = false;


    private void Start()
    {
        chatInput.characterLimit = 20;
    }
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
                    SendChat(chatInput.text, PlayerController.colorNumber);
                    chatInput.text = ""; // 입력 필드를 비웁니다.
                }
                EventSystem.current.SetSelectedGameObject(null);
                isFocused = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetMouseButtonDown(0)) ||(Input.GetMouseButtonDown(1)) || (Input.GetMouseButtonDown(2)))
        {
            EventSystem.current.SetSelectedGameObject(null);
            isFocused = false;
        }
    }
    // Start is called before the first frame update

    
    
    int MaxChatLogLine(string context)
    {
        int cnt = 0;

        for(int i = 0;i< context.Length; i++)
        {
            if (context[i] == '\n') cnt++;
        }
        return cnt;
    }

    [PunRPC]
    void Chatting(string context, int a)
    {
        string colorCodeStart = null;
        string[] temp = new string[2];
        string playerName = PhotonNetwork.NickName + " : ";

        switch (a)
        {
            case 0:
                colorCodeStart = "<color=red>";
                break;
            case 1:
                colorCodeStart = "<color=yellow>";
                break;
            case 2:
                colorCodeStart = "<color=green>";
                break;
            case 3:
                colorCodeStart = "<color=blue>";
                break;
        }
        print(MaxChatLogLine(chatDisplay.text));
        if(MaxChatLogLine(chatDisplay.text) >= 7)
        {
            chatDisplay.text = chatDisplay.text.Substring(chatDisplay.text.IndexOf('\n') + 1);
        }
        chatDisplay.text += colorCodeStart + playerName + "</color>" + context + '\n';
    }

    void SendChat(string context, int a)
    {
        PV.RPC("Chatting", RpcTarget.All, context, a);
    }
    [PunRPC]
    public void SystemChatting(string context)
    {

        print(MaxChatLogLine(chatDisplay.text));
        if (MaxChatLogLine(chatDisplay.text) >= 7)
        {
            chatDisplay.text = chatDisplay.text.Substring(chatDisplay.text.IndexOf('\n') + 1);
        }
        chatDisplay.text += context + '\n';
    }
}
