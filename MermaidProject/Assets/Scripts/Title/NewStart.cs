using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;

public class NewStart : MonoBehaviour
{
    [SerializeField] GameObject newStartWindow;
    [SerializeField] GameObject joinWindow;
    [SerializeField] GameObject noticeWindow;
    [SerializeField] GameObject loadWindow;
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
        joinWindow.SetActive(false);
        noticeWindow.SetActive(false);
        newStartWindow.SetActive(true);
        loadWindow.SetActive(false);
    }
}
