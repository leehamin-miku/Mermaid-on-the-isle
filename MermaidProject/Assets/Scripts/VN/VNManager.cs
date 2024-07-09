using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class VNManager : MonoBehaviour
{
    public GameObject Noah;
    public GameObject Morgana;
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
        NoahController = Noah.GetComponent<StandingController>();
        MorganaController = Morgana.GetComponent<StandingController>();

        ChoiceManager = AnswerGroup.GetComponent<ChoiceManager>();
        StartCoroutine(VNScripts());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("좌클");
            

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
            while (timer<delay)
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

    
    IEnumerator VNScripts()
    {
        // Resources 폴더 내에 있는 dialogue 파일을 List 형태로 불러옴(CSV Reader 이용)
        List<Dictionary<string, object>> Dialogue = CSVReader.Read("VN_DB/dialogue");
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

            if (Narrator == "EndOfFile")
            {
                print("a");
                yield return null;
                break;
            }
            else if(Narrator == "Effect_Sensei"){
                // ~ Narration 값에 따라 해당 효과 발생시키는 함수 만들기

                print("효과발생! sensei");
               yield return StartCoroutine(NoahController.Effect(Narration));
            }
            else if(Narrator == "Effect_Arona")
            {
                print("효과발생! arona");
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if(Narrator == "Effect_All")
            {
                print("효과발생! All");
                StartCoroutine(NoahController.Effect(Narration));
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if(Narrator == "Question")
            {
                j = i;
                while(Narrator != "QuestionEnd")
                {
                    Questions[j - i] = Narration;
                    j++;
                    Narrator = Dialogue[j]["Narrator"].ToString();
                    Narration = Dialogue[j]["Narration"].ToString();
                    print(Narrator);
                    yield return null;
                }
                StartCoroutine(ChoiceManager.MakeChoice(j-i, Questions));
                i = j;

                // 여기서 퀘스천 처리?
            }
            else
            {
                HalfStandingChange(HalfStandingSprite);
                yield return StartCoroutine(NormalChat(Narrator, Narration));
            }
            i++;

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
