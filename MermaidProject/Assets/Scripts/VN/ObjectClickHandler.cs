using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    public TMP_Text AnswerText;

    private void OnMouseDown()
    {
        ExecuteOnClick();
    }

    private void ExecuteOnClick()
    {
        Debug.Log("Object clicked!");
        // 클릭 실행
    }

    private void OnMouseEnter()
    {
        AnswerText.color = Color.red;
    }

    private void OnMouseExit()
    {
        AnswerText.color = Color.white;

    }
}
