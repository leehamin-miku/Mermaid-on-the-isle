using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceManager : MonoBehaviour
{

    public GameObject AnswerButtonGroup;
    private TMP_Text[] Answers;

    public IEnumerator MakeChoice(int n, string[] Questions)
    {

        Transform parentTransform = AnswerButtonGroup.transform;

        Answers = new TMP_Text[n];
        Answers = parentTransform.GetComponentsInChildren<TMP_Text>();

        // ¼±Áö Fade in
        for (int i = 0; i < n; i++)
        {
            Answers[i].text = Questions[i];
            StartCoroutine(AnswersFadeIn(Answers[i]));
            Answers[i].GetComponent<BoxCollider2D>().enabled = true; 
        }



        yield return null;
    }

    IEnumerator AnswersFadeIn(TMP_Text Answer)
    {
        while(Answer.color.a < 1.0f)
        {
            Answer.color = new Color(Answer.color.r, Answer.color.g, Answer.color.b, Answer.color.a + (Time.deltaTime / .5f));
            yield return null;
        }
        yield return null;
    }
}
