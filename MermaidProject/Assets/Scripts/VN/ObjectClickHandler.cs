using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    public TMP_Text AnswerText;
    public string OutputValue;

    public static event Action<string> OnObjectClicked;

    private void OnMouseDown()
    {
        ExecuteOnClick();
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

    
}
