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
            print("좌클");
            

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

    // 선택지에 따라 해당하는 새로운 디아로그 시작 / OutputValue는 선택지마다 다름 (현재는 2번 Correct, 1,3~5번 Incorrect
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
        // Resources 폴더 내에 있는 dialogue 파일을 List 형태로 불러옴(CSV Reader 이용)
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
                // ~ Narration 값에 따라 해당 효과 발생시키는 함수 만들기

                print("효과발생! Noah");
                yield return StartCoroutine(NoahController.Effect(Narration));
            }
            else if (Narrator == "Effect_Morgana")
            {
                print("효과발생! Morgana");
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if (Narrator == "Effect_All")
            {
                print("효과발생! All");
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
                // Appear[0] = 이름, [1] = x좌표, [2] = y좌표
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
    /// narrator : 사람 이름 | narration : 대사
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

        // 타이핑 효과
        for (int i = 0; i < narration.Length; i++)
        {
            writerText += narration[i];
            ChatText.text = writerText;

            // delay(0.05초)마다 한 글자 출력, 중간에 space or 좌클릭 들어오면 전부 출력
            // 만약 타이핑 속도 변화가 필요하다면, delay를 따로 받아서 쓰면 될 듯
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


    // 상반신 sprite를 바꾼다.
    public SpriteRenderer HalfStandingSpriteRenderer;       // 기존 이미지

    public void HalfStandingChange(string after_img)
    {

        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        HalfStandingSpriteRenderer.sprite = newSprite;
    }
}
