using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class VNManager : MonoBehaviour
{
    private GameObject Noah;
    private GameObject Morgana;
    public GameObject NoahPrefab;
    public GameObject MorganaPrefab;

    public GameObject AnswerGroup;

    private StandingController NoahController;
    private StandingController MorganaController;
    private ChoiceManager ChoiceManager;

    public TMP_Text ChatText;
    public TMP_Text CharacterName;

    public string writerText = "";

    
    // Start is called before the first frame update
    void Start()
    {
        ChoiceManager = AnswerGroup.GetComponent<ChoiceManager>();
        print("a");
        StartVN("dialogue");


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("��Ŭ");
            

        }
    }


    List<Dictionary<string, object>> Dialogue;

    private void OnEnable()
    {
        ObjectClickHandler.OnObjectClicked += HandleObjectClicked;
    }

    private void OnDisable()
    {
        ObjectClickHandler.OnObjectClicked -= HandleObjectClicked;
    }

    // �������� ���� �ش��ϴ� ���ο� ��Ʒα� ���� / OutputValue�� ���������� �ٸ� (����� 2�� Correct, 1,3~5�� Incorrect
    private void HandleObjectClicked(string OutputValue)
    {
        StartVN("dialogue" + OutputValue);
    }

    void StartVN(string DialogueName)
    {
        Dialogue = CSVReader.Read("VN_DB/" + DialogueName);
        StartCoroutine(VNScripts());
    }

    IEnumerator VNScripts()
    {
        // Resources ���� ���� �ִ� dialogue ������ List ���·� �ҷ���(CSV Reader �̿�)
        int i = 0, j = 0;
        string Narrator = null;
        string Narration = null;
        string HalfStandingSprite = null;
        string[] Questions = new string[5];

        while (true)
        {
            Narrator = Dialogue[i]["Narrator"].ToString();
            Narration = Dialogue[i]["Narration"].ToString();
            HalfStandingSprite = Dialogue[i]["Sprite"].ToString();

            
            if (Narrator == "Break")
            {
                print("Break!!");
                yield return null;
                break;
            }
            else if (Narrator == "Effect_Noah")
            {
                // ~ Narration ���� ���� �ش� ȿ�� �߻���Ű�� �Լ� �����

                print("ȿ���߻�! Noah");
                yield return StartCoroutine(NoahController.Effect(Narration));
            }
            else if (Narrator == "Effect_Morgana")
            {
                print("ȿ���߻�! Morgana");
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if (Narrator == "Effect_All")
            {
                print("ȿ���߻�! All");
                StartCoroutine(NoahController.Effect(Narration));
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if (Narrator == "Question")
            {
                j = i;
                while (Narrator != "QuestionEnd")
                {
                    Questions[j - i] = Narration;
                    j++;
                    Narrator = Dialogue[j]["Narrator"].ToString();
                    Narration = Dialogue[j]["Narration"].ToString();
                    print(Narrator);
                    yield return null;
                }
                StartCoroutine(ChoiceManager.MakeChoice(j - i, Questions));
                i = j;

                break;
            }
            else if (Narrator == "Appear")
            {
                // Appear[0] = �̸�, [1] = x��ǥ, [2] = y��ǥ
                string[] Appear = Narration.Split('`');
                if (Appear[0] == "Noah")
                {
                    Vector3 spawnPosition = new Vector3(float.Parse(Appear[1]), float.Parse(Appear[2]), 0);
                    Noah = Instantiate(NoahPrefab, spawnPosition, Quaternion.identity);
                    NoahController = Noah.GetComponent<StandingController>();
                    yield return null;

                }
                else if (Appear[0] == "Morgana")
                {
                    Vector3 spawnPosition = new Vector3(float.Parse(Appear[1]), float.Parse(Appear[2]), 0);
                    Morgana = Instantiate(MorganaPrefab, spawnPosition, Quaternion.identity);
                    MorganaController = Morgana.GetComponent<StandingController>();
                    yield return null;
                }
            }
            else if (Narrator == "Exit")
            {
                if (Narration == "Noah")
                {
                    Destroy(Noah);
                    yield return null;

                }
                else if (Narration == "Morgana")
                {
                    Destroy(Morgana);
                    yield return null;
                }
            }
            else
            {
                HalfStandingChange(HalfStandingSprite);
                yield return StartCoroutine(NormalChat(Narrator, Narration));
            }
            i++;

        }
    }

    /// <summary>
    /// narrator : ��� �̸� | narration : ���
    /// </summary>
    /// <param name="narrator"></param>
    /// <param name="narration"></param>
    /// <returns></returns>
    IEnumerator NormalChat(string narrator, string narration)
    {

        CharacterName.text = narrator;
        writerText = "";
        float timer = 0;
        float delay = 0.05f;
        bool isSkip = false;

        // Ÿ���� ȿ��
        for (int i = 0; i < narration.Length; i++)
        {
            writerText += narration[i];
            ChatText.text = writerText;

            // delay(0.05��)���� �� ���� ���, �߰��� space or ��Ŭ�� ������ ���� ���
            // ���� Ÿ���� �ӵ� ��ȭ�� �ʿ��ϴٸ�, delay�� ���� �޾Ƽ� ���� �� ��
            while (timer < delay)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
                {
                    isSkip = true;
                    yield return null;
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
            if (isSkip)
            {
                isSkip = false;
                ChatText.text = narration;
                yield return null;
                break;
            }
            yield return null;
        }



        while (true)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
            {
                yield return null;
                break;
            }
            yield return null;
        }
    }


    // ��ݽ� sprite�� �ٲ۴�.
    public SpriteRenderer HalfStandingSpriteRenderer;       // ���� �̹���

    public void HalfStandingChange(string after_img)
    {

        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        HalfStandingSpriteRenderer.sprite = newSprite;
    }
}
