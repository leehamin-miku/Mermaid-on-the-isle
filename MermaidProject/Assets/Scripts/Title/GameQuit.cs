using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;

public class GameQuit : MonoBehaviour
{
    // Start is called before the first frame update
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
        TransitionManager.Instance().onTransitionCutPointReached += GameQuitFuc; 
        TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0f);
    }
    private void GameQuitFuc()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
