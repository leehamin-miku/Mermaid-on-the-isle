using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class VNManager : MonoBehaviour
{
    public GameObject Noah;
    public GameObject Morgana;
    public GameObject Button;

    private NoahController NoahController;
    private MorganaController MorganaController;
    private ChoiceManager ChoiceManager;

    public TMP_Text ChatText;
    public TMP_Text CharacterName;

    public string writerText = "";

    // Start is called before the first frame update
    void Start()
    {
        NoahController = Noah.GetComponent<NoahController>();
        MorganaController = Morgana.GetComponent<MorganaController>();

        ChoiceManager = Button.GetComponent<ChoiceManager>();
        StartCoroutine(VNScripts());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("��Ŭ");
            //StartCoroutine(ChoiceManager.MakeChoice(4));

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
               yield return StartCoroutine(NoahController.NoahEffect(Narration));
            }
            else if(Narrator == "Effect_Arona")
            {
                print("ȿ���߻�! arona");
                yield return StartCoroutine(MorganaController.MorganaEffect(Narration));
            }
            else if(Narrator == "Effect_All")
            {
                print("ȿ���߻�! All");
                StartCoroutine(NoahController.NoahEffect(Narration));
                yield return StartCoroutine(MorganaController.MorganaEffect(Narration));
            }
            else if(Narrator == "Question")
            {
                j = i + 1;
                while(Narrator != "QuestionEnd")
                {
                    print("b");
                    Narrator = Dialogue[j++]["Narrator"].ToString();
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

    // ��ݽ� sprite�� �ٲ۴�.
    public SpriteRenderer HalfStandingSpriteRenderer;       // ���� �̹���

    public void HalfStandingChange(string after_img)
    {

        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        HalfStandingSpriteRenderer.sprite = newSprite;
    }
}
