using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChoiceManager : MonoBehaviour
{

    public GameObject AnswerButtonGroup;
    private TMP_Text[] Answers;

    private void OnEnable()
    {
        ObjectClickHandler.OnObjectClicked += HandleObjectClicked;
    }

    private void OnDisable()
    {
        ObjectClickHandler.OnObjectClicked -= HandleObjectClicked;
    }

    // ���� ������ �޾Ƽ� �Ѿ �� ���
    private void HandleObjectClicked(string OutputValue)
    {

        for (int i = 0; i < Answers.Length; i++)
        {
            StartCoroutine(AnswersFadeOut(Answers[i]));
            Answers[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public IEnumerator MakeChoice(int n, string[] Questions)
    {

        Answers = transform.GetComponentsInChildren<TextMeshPro>();
        ObjectClickHandler[] objectClickHandler = transform.GetComponentsInChildren<ObjectClickHandler>();

        // ���� Fade in
        for (int i = 0; i < n; i++)
        {
            // s[0] = ���� ����, s[1] = ������ OutValue = �� üũ
            Debug.Log(i);
            string[] s = Questions[i].Split('`');
            objectClickHandler[i].OutputValue = s[1];
            Answers[i].text = s[0];
            StartCoroutine(AnswersFadeIn(Answers[i]));
        }
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < n; i++) Answers[i].GetComponent<BoxCollider2D>().enabled = true;



        yield return null;
    }

    IEnumerator AnswersFadeIn(TMP_Text Answer)
    {
        while (Answer.color.a < 1.0f)
        {
            Answer.color = new Color(Answer.color.r, Answer.color.g, Answer.color.b, Answer.color.a + (Time.deltaTime / .5f));
            Answer.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, Answer.color.a/2);
            yield return null;
        }
        yield return null;
    }

    IEnumerator AnswersFadeOut(TMP_Text Answer)
    {
        while (Answer.color.a > 0)
        {
            Answer.color = new Color(Answer.color.r, Answer.color.g, Answer.color.b, Answer.color.a - (Time.deltaTime / .5f));
            Answer.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, (Answer.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a -  (Time.deltaTime / .5f))/2);
            yield return null;
        }
        yield return null;
    }
}