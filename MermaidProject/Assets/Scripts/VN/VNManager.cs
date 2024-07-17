using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using EasyTransition;
using static UnityEngine.GraphicsBuffer;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Photon.Realtime;

public class VNManager : MonoBehaviourPunCallbacks
{
    //마스터가 사용하는 변수
    public string nextDialogue = "dialogue";
    
    
    
    public GameObject StandingGroup;
    public GameObject AnswerGroup;

    public PlayerController PlayerController;
    public PhotonView PV;

    private ChoiceManager ChoiceManager;

    public TMP_Text ChatText;
    public TMP_Text CharacterName;

    public string writerText = "";

    int page = 0;

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
        PlayerController.PV.RPC("VNStart", RpcTarget.All);
        VNRunning = true;
        Dialogue = CSVReader.Read("VN_DB/" + DialogueName);
        StartCoroutine(VNScripts());
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MasterController());
        }
    }
    [PunRPC]
    public void StartNextDialogue()
    {
        VNRunning = true;
        PlayerController.PV.RPC("VNStart", RpcTarget.All);
        Dialogue = CSVReader.Read("VN_DB/" + nextDialogue);
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
            else if (ActionName == "SmoothSpriteChange")
            {
                print("SmooothSpriteChange");
                StartCoroutine(StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<StandingController>().SmoothSpriteChange(Parameter));

            }
            else if (ActionName == "Lobby")
            {
                //Island,,다음다이올로그 이름
                PlayerController.transform.position = new Vector2(-85, -121);
                foreach (Transform ts in GameObject.Find("SignGroup").transform)
                {
                    Destroy(ts.gameObject);
                }
                nextDialogue = Parameter;
                print("로비로");
                VNRunning = false;

                break;
            }
            else if (ActionName == "TidalAdd")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //구분, 갯수
                    string[] a = Parameter.Split('`');
                    //a[0] = 구분
                    //a[1] = 쓰나미의 갯수 (한번에 많이 소환하기 위한)
                    //a[2] =  다음 Tidal 까지의 시간

                    //예상시간, Tidal사이의 시간

                    if (int.Parse(a[0]) == 0)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 30;
                        for (int w = 0; w < int.Parse(a[1]); w++)
                        {
                            //이 오브젝트들은 마스터에게만 존재함
                            GameObject tsunami = new GameObject();
                            tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                            tsunami.AddComponent<TsunamiObject>();
                            tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(new Vector3(vec.x, vec.y), 10, float.Parse(a[2]));
                            PV.RPC("AddSign", RpcTarget.All, vec+ new Vector2(GameObject.Find("IslandSquare").transform.position.x, GameObject.Find("IslandSquare").transform.position.y));
                        }
                    }
                }



            }
            else if (ActionName == "Island")
            {
                //Island,시간(초),다음다이올로그 이름
                //상점 초기화
                //타이머 시작
                //다이올로그 이름에 대해선 마스터에게만 적용
                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject.Find("Shop").GetComponent<Shop>().InitializeShop();
                    GameObject.Find("TideTimer").GetComponent<TideTimer>().TimeSetAndStart(int.Parse(Target));
                    nextDialogue = Parameter;
                }
                PlayerController.transform.position += GameObject.Find("IslandSquare").transform.position - GameObject.Find("LobbySquare").transform.position;
                print("섬 이야기 시작");
                VNRunning = false;
                break;
            }
            else
            {
                HalfStandingChange(Parameter);
                yield return StartCoroutine(NormalChat(ActionName, Target));
            }
            i++;

        }

        if (VNRunning == false)
        {
            PlayerController.PV.RPC("VNEndSub", RpcTarget.All);
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



        while (isSkip == false)
        {
            yield return null;
        }
        isSkip = false;

    }

    IEnumerator MasterController()
    {
        while (VNRunning)
        {
            if (PV.IsMine && (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && !GameObject.Find("GameManager").GetComponent<ChattingManager>().isFocused)
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
    [PunRPC]
    public void AddSign(Vector2 vec)
    {
        GameObject go = Instantiate(Resources.Load("Prefab/Game/Sign") as GameObject);
        go.transform.parent = GameObject.Find("SignGroup").transform;
        go.GetComponent<Sign>().target = vec;
        go.transform.localScale = new Vector3(20, 20, 1);
    }



    // 상반신 sprite를 바꾼다.
    public SpriteRenderer HalfStandingSpriteRenderer;       // 기존 이미지

    public void HalfStandingChange(string after_img)
    {
        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        HalfStandingSpriteRenderer.sprite = newSprite;
    }


}
