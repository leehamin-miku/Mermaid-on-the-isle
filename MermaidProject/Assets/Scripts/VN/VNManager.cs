using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using EasyTransition;
using static UnityEngine.GraphicsBuffer;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class VNManager : MonoBehaviourPunCallbacks
{

    public GameObject StandingGroup;
    public GameObject AnswerGroup;

    public PlayerController PlayerController;
    public PhotonView PV;

    private ChoiceManager ChoiceManager;

    public TMP_Text ChatText;
    public TMP_Text CharacterName;

    public string writerText = "";

    bool isSkip;
    public bool VNRunning;
    
    // Start is called before the first frame update
    void Start()
    {
        ChoiceManager = AnswerGroup.GetComponent<ChoiceManager>();
    }



    List<Dictionary<string, object>> Dialogue;

    private new void OnEnable()
    {
        ObjectClickHandler.OnObjectClicked += HandleObjectClicked;
    }

    private new void OnDisable()
    {
        ObjectClickHandler.OnObjectClicked -= HandleObjectClicked;
    }

    // 선택지에 따라 해당하는 새로운 디아로그 시작 / OutputValue는 선택지마다 다름 (현재는 2번 Correct, 1,3~5번 Incorrect
    private void HandleObjectClicked(string OutputValue)
    {
        StartVN("dialogue" + OutputValue);
    }

    
    public void StartVN(string DialogueName)
    {
        VNRunning = true;
        Debug.Log("VN시작");
        Dialogue = CSVReader.Read("VN_DB/" + DialogueName);
        StartCoroutine(VNScripts());
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MasterController());
        }
    }

    IEnumerator VNScripts()
    {
        
        // Resources 폴더 내에 있는 dialogue 파일을 List 형태로 불러옴(CSV Reader 이용)
        int i = 0, j = 0;
        string ActionName = null;
        string Target = null;
        string Parameter = null;
        string[] Questions = new string[5];

        while (true)
        {
            ActionName = Dialogue[i]["ActionName"].ToString();
            Target = Dialogue[i]["Target"].ToString();
            Parameter = Dialogue[i]["Parameter"].ToString();

            if (ActionName == "Break")
            {
                    print("Break!!");
                    VNRunning = false;
                    
                    break;
            }
            else if (ActionName == "Effect")
            {
                 print("효과발생! standing0");
                 yield return StartCoroutine(StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<StandingController>().Effect(Parameter));
                  

            }
            else if (ActionName == "Question")
            {
                    j = i;
                    while (ActionName != "QuestionEnd")
                    {
                        Questions[j - i] = Target;
                        j++;
                        ActionName = Dialogue[j]["ActionName"].ToString();
                        Target = Dialogue[j]["Target"].ToString();
                        print(ActionName);
                        
                    }
                    StartCoroutine(ChoiceManager.MakeChoice(j - i, Questions));
                    i = j;

                    break;
            }
            else if (ActionName == "SpriteChange")
            {
                // SpriteChange[0] = 이미지 링크, [1] = x좌표, [2] = y좌표, [3] = Size
                string[] SpriteChange = Parameter.Split('`');

                Vector3 spawnPosition = new Vector3(float.Parse(SpriteChange[1]), float.Parse(SpriteChange[2]), 0);
                Vector3 Scale = new Vector3(float.Parse(SpriteChange[3]), float.Parse(SpriteChange[3]), float.Parse(SpriteChange[3]));
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + SpriteChange[0]);
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.localScale = Scale;
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.position = spawnPosition;

                
            }
            else if (ActionName == "Exit")
            {
                 StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = null;
                
            }
            else if(ActionName == "SmoothSpriteChange")
            {
                print("SmooothSpriteChange");
                StartCoroutine(StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<StandingController>().SmoothSpriteChange(Parameter));
                
            }
            else 
            {
                HalfStandingChange(Parameter);
                yield return StartCoroutine(NormalChat(ActionName, Target));
            }
            i++;

        }

        if (VNRunning==false)
        {
            PlayerController.PV.RPC("VNEndSub",RpcTarget.All);
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

        // 타이핑 효과
        for (int i = 0; i < narration.Length; i++)
        {
            writerText += narration[i];
            ChatText.text = writerText;

            // delay(0.05초)마다 한 글자 출력, 중간에 space or 좌클릭 들어오면 전부 출력
            // 만약 타이핑 속도 변화가 필요하다면, delay를 따로 받아서 쓰면 될 듯
            while (timer < delay)
            {
                if (isSkip)
                {
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



        while (isSkip==false)
        {
            yield return null;
        }
        isSkip = false;
        
    }

    IEnumerator MasterController()
    {
        while (VNRunning)
        {
            if (PV.IsMine && (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")))
            {
                PV.RPC("SkipOn", RpcTarget.All);
            }
            yield return null;
        }
        
    }

    [PunRPC]
    public void SkipOn()
    {
        isSkip = true;
    }



    // 상반신 sprite를 바꾼다.
    public SpriteRenderer HalfStandingSpriteRenderer;       // 기존 이미지

    public void HalfStandingChange(string after_img)
    {
        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        HalfStandingSpriteRenderer.sprite = newSprite;
    }

    
}
