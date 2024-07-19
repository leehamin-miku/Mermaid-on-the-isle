using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;

public class VNManager : MonoBehaviourPunCallbacks
{
    //마스터가 사용하는 변수
    public string nextDialogue = "dialogue";

    public List<CoroutineAbs> coList = new List<CoroutineAbs>();

    public static GameObject StandingGroup;
    public GameObject AnswerGroup;

    public PlayerController PlayerController;
    public PhotonView PV;

    private ChoiceManager ChoiceManager;

    public static TMP_Text ChatText;
    public static TMP_Text CharacterName;

    public static string writerText = "";

    int page = 0;

    public bool isEnd = false;
    public bool VNRunning;

    int Skip;
    // Start is called before the first frame update
    void Start()
    {
        ChatText = GameObject.Find("ChatText").GetComponent<TextMeshPro>();
        CharacterName = GameObject.Find("CharacterName").GetComponent<TextMeshPro>();
        ChoiceManager = AnswerGroup.GetComponent<ChoiceManager>();
        StandingGroup = GameObject.Find("StandingGroup");
    }

    int i = 0;

    Coroutine masterControlCoroutine;


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

    [PunRPC]
    public void StartVN(string DialogueName)
    {
        VNRunning = true;
        PlayerController.PV.RPC("VNStart", RpcTarget.All);
        Dialogue = CSVReader.Read("VN_DB/" + nextDialogue);
        i = 0;
        VNNextScript();
        if (PhotonNetwork.IsMasterClient)
        {
            masterControlCoroutine = StartCoroutine(MasterController());
        }
    }

    public void StartNextDialogue()
    {
        PV.RPC("StartVN", RpcTarget.All, nextDialogue);
    }

    public void VNNextScript()
    {

        // Resources 폴더 내에 있는 dialogue 파일을 List 형태로 불러옴(CSV Reader 이용)

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
                i++;
            }
            else if (ActionName == "Question")
            {
                int j = i;
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
                i++;
            }
            
            else if (ActionName == "Move")
            {
                // move[0] = 1회당 이동거리, move[1] = 횟수, move[2] = 방향
                CoroutineAbs temp = new Move(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "Jump")
            {
                CoroutineAbs temp = new Jump(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "Emotion")
            {
                CoroutineAbs temp = new Emotion(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp );
                i++;
            }
            else if (ActionName == "Exit")
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = null;

            }
            else if (ActionName == "SmoothSpriteChange")
            {
                CoroutineAbs temp = new SmoothSpriteChange(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
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
                VNRunning = false;
                i++;
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

                    //구분=0일때 일반 쓰나미
                    //구분=1일때 제로쓰나미
                    //구분=2일때 상어 크리쳐

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
                        }
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                    else if (int.Parse(a[0]) == 1)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 30;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(Vector3.zero, 0, float.Parse(a[2]));
                    }
                    else if (int.Parse(a[0]) == 2)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 30;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonCreature(new Vector3(vec.x, vec.y), 5, float.Parse(a[2]));
                    }
                }
                i++;



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
                GameObject.Find("TideTimer").GetComponent<TideTimer>().totalTime = int.Parse(Target);
                PlayerController.transform.position += GameObject.Find("IslandSquare").transform.position - GameObject.Find("LobbySquare").transform.position;
                VNRunning = false;
                i++;
                break;
            }
            else
            {
                HalfStandingChange(Parameter);
                CoroutineAbs temp = new NormalChat(ActionName, Target);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
                break;
            }

        }

        if (VNRunning == false)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StopCoroutine(masterControlCoroutine);
            }
            PlayerController.PV.RPC("VNEndSub", RpcTarget.All);
        }
    }

    /// <summary>
    /// narrator : 사람 이름 | narration : 대사
    /// </summary>
    /// <param name="narrator"></param>
    /// <param name="narration"></param>
    /// <returns></returns>
    /// 



    //마스터만 이걸 실행하도록 하세요.
    IEnumerator MasterController()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    PV.RPC("StopAllCoAndNextScript", RpcTarget.All);
                }
            }
            yield return null;
        }
    }



    [PunRPC]
    void StopAllCoAndNextScript()
    {

        foreach (CoroutineAbs co in coList)
        {
            StopCoroutine(co.co);
            co.EndAction();
        }
        coList.Clear();
        VNNextScript();
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
    public abstract class CoroutineAbs
    {
        public Coroutine co;
        public abstract IEnumerator Action();
        public virtual void EndAction()
        {

        }

    }
    public class NormalChat : CoroutineAbs
    {
        string narrator;
        string narration;

        public NormalChat(string narrator, string narration)
        {
            this.narrator = narrator;
            this.narration = narration;
        }
        public override IEnumerator Action()
        {
            CharacterName.text = narrator;
            writerText = "";
            float timer = 0;
            float delay = 0.1f;

            // 타이핑 효과
            for (int i = 0; i < narration.Length; i++)
            {
                writerText += narration[i];
                ChatText.text = writerText;

                // delay(0.1초)마다 한 글자 출력, 중간에 space or 좌클릭 들어오면 전부 출력
                // 만약 타이핑 속도 변화가 필요하다면, delay를 따로 받아서 쓰면 될 듯
                while (timer < delay)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

            }
            ChatText.text = narration;
        }
        public override void EndAction()
        {
            base.EndAction();
            ChatText.text = narration;
        }
    }
    public class SmoothSpriteChange : CoroutineAbs
    {
        string Parameter;
        string Target;
        public SmoothSpriteChange(string Target, string Parameter)
        {
            this.Target = Target;
            this.Parameter = Parameter;
        }

        public override IEnumerator Action()
        {
            Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + Parameter);
            Transform standing = StandingGroup.transform.GetChild(int.Parse(Target));
            standing.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = newSprite;
            standing.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            standing.GetChild(0).position = standing.position;
            standing.GetChild(0).localScale = new Vector3(1, 1, 1);

            while (standing.GetComponent<SpriteRenderer>().color.a > 0)
            {
                standing.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, standing.GetComponent<SpriteRenderer>().color.a - Time.deltaTime / .5f);
                standing.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, standing.GetChild(0).GetComponent<SpriteRenderer>().color.a + Time.deltaTime / .5f);
                yield return null;
            }

            standing.GetComponent<SpriteRenderer>().sprite = newSprite;
            standing.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            standing.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
        override public void EndAction()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + Parameter);
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            StandingGroup.transform.GetChild(int.Parse(Target)).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public class Move : CoroutineAbs
    {
        public bool isEnd = false;
        string Parameter;
        string Target;
        string[] move;
        Vector3 startPosition;
        Vector3 EndPosition;

        public Move(string Target, string Parameter)
        {
            // move[0] = 1회당 이동거리, move[1] = 횟수, move[2] = 방향
            this.Target = Target;
            this.Parameter = Parameter;
            move = Parameter.Split('`');
            startPosition = StandingGroup.transform.GetChild(int.Parse(Target)).transform.position;
            EndPosition = startPosition + new Vector3(int.Parse(move[0]) * int.Parse(move[1]) * int.Parse(move[2]), 0, 0);
        }

        public override IEnumerator Action()
        {
            float acceleration = 0.05f, velocity = 0;
            for (int i = 1; i <= int.Parse(move[1]); i++)
            {
                while (true)
                {
                    // 양의 방향으로 가면서 StartPosition + 1회 이동거리보다 커질 때 or 음의 방향으로 가면서 StartPosition - 1회 이동거리보다 작아질 때 break
                    if ((StandingGroup.transform.GetChild(int.Parse(Target)).transform.position.x >= startPosition.x + float.Parse(move[0]) * float.Parse(move[2]) / 2) && move[2] == "1") break;
                    else if ((StandingGroup.transform.GetChild(int.Parse(Target)).transform.position.x <= startPosition.x + float.Parse(move[0]) * float.Parse(move[2]) / 2) && move[2] == "-1") break;
                    else velocity += Time.deltaTime * acceleration;
                    StandingGroup.transform.GetChild(int.Parse(Target)).transform.position += velocity * new Vector3(int.Parse(move[2]), 0, 0);
                    yield return null;
                }
                while (true)
                {
                    if (StandingGroup.transform.GetChild(int.Parse(Target)).transform.position.x >= startPosition.x + float.Parse(move[0]) * float.Parse(move[2]) && move[2] == "1") break;
                    else if (StandingGroup.transform.GetChild(int.Parse(Target)).transform.position.x <= startPosition.x + float.Parse(move[0]) * float.Parse(move[2]) && move[2] == "-1") break;
                    else if (velocity <= 0) break;
                    else velocity -= Time.deltaTime * acceleration;
                    StandingGroup.transform.GetChild(int.Parse(Target)).transform.position += velocity * new Vector3(int.Parse(move[2]), 0, 0);
                    yield return null;
                }
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.position = startPosition + new Vector3(int.Parse(move[0]) * int.Parse(move[2]), 0, 0);
                startPosition = StandingGroup.transform.GetChild(int.Parse(Target)).transform.position;
                velocity = 0;
                yield return new WaitForSeconds(0.3f);
            }
        }

        override public void EndAction()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).transform.position = EndPosition;
        }
    }

    public class Jump : CoroutineAbs
    {
        // parameter = 점프시 가속도 = 높이를 조절함
        string Parameter;
        string Target;
        Vector3 startPosition;

        public Jump(string Target, string Parameter)
        {
            this.Target = Target;
            this.Parameter = Parameter;
            startPosition = StandingGroup.transform.GetChild(int.Parse(Target)).transform.position;
        }

        public override IEnumerator Action()
        {
            float acceleration = float.Parse(Parameter);
            float gravityAcceleration = 9.8f;
            while (acceleration > 0)
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.position += new Vector3(0, acceleration * Time.deltaTime, 0);
                acceleration -= gravityAcceleration * Time.deltaTime;
                yield return null;
            }
            while (StandingGroup.transform.GetChild(int.Parse(Target)).transform.position.y >= startPosition.y)
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.position -= new Vector3(0, acceleration * Time.deltaTime, 0);
                acceleration += gravityAcceleration * Time.deltaTime;
                yield return null;
            }
        }

        override public void EndAction()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).transform.position = startPosition;
        }

    }

    public class Emotion : CoroutineAbs
    {
        string Parameter;
        string Target;
        string[] Bubble;
        public Emotion(string Target, string Parameter)
        {
            this.Target = Target;
            this.Parameter = Parameter;
            // Bubble[0] = 표현하는 스탠딩 번호, Bubble[1] = 사용할 스프라이트
            Bubble = Parameter.Split(',');
        }

        public override IEnumerator Action()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + Bubble[1]);
            StandingGroup.transform.GetChild(int.Parse(Target)).position = StandingGroup.transform.GetChild(int.Parse(Bubble[0])).position + new Vector3(-1, 1, 0);
            StandingGroup.transform.GetChild(int.Parse(Target)).rotation = Quaternion.Euler(new Vector3(0, 0, 25));
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            while (StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color.a <=1)
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color =
                    new Color(1, 1, 1, StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color.a + Time.deltaTime / .1f);
                yield return null;
            }
            yield return new WaitForSeconds(2f);
            while (StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color.a > 0)
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color =
                    new Color(1, 1, 1, StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color.a - Time.deltaTime / .3f);
                yield return null;
            }
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = null;
        }

        override public void EndAction()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
