using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;

public class NewStart : MonoBehaviour
{
    static bool isFirst = true;
    //TitleScene의 새로시작 버튼의 컴포넌트 스크립트

    private void OnMouseEnter()
    {
        GetComponent<TextMeshPro>().color = Color.gray;
    }
    private void OnMouseExit()
    {
        GetComponent<TextMeshPro>().color = Color.white;
    }
    private void OnMouseDown()
    {
        TransitionManager.Instance().Transition("LobbyScene", GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0f);
    }
}
