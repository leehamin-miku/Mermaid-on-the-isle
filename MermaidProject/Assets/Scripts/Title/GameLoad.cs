using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    [SerializeField] GameObject newStartWindow;
    [SerializeField] GameObject joinWindow;
    [SerializeField] GameObject noticeWindow;
    [SerializeField] GameObject loadWindow;
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
        newStartWindow.SetActive(false);
        noticeWindow.SetActive(false);
        loadWindow.SetActive(true);
    }
}
