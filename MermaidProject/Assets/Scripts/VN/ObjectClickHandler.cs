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
        // Ŭ�� ����
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
