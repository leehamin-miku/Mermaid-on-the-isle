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
            print("��Ŭ");
            

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
        // Resources ���� ���� �ִ� dialogue ������ List ���·� �ҷ���(CSV Reader �̿�)
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
                // ~ Narration ���� ���� �ش� ȿ�� �߻���Ű�� �Լ� �����

                print("ȿ���߻�! sensei");
               yield return StartCoroutine(NoahController.Effect(Narration));
            }
            else if(Narrator == "Effect_Arona")
            {
                print("ȿ���߻�! arona");
                yield return StartCoroutine(MorganaController.Effect(Narration));
            }
            else if(Narrator == "Effect_All")
            {
                print("ȿ���߻�! All");
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

                // ���⼭ ����õ ó��?
            }
            else
            {
                HalfStandingChange(HalfStandingSprite);
                yield return StartCoroutine(NormalChat(Narrator, Narration));
            }
            i++;

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
