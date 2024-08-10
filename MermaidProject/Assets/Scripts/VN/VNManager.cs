using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Pun.Demo.Procedural;
using EasyTransition;
using System.Data.Common;

public class VNManager : MonoBehaviourPunCallbacks
{
    //�����Ͱ� ����ϴ� ����
    //���̺� ���
    public string nextDialogue = "4n1";
    public string nextSubDialogue = "";
    public List<CoroutineAbs> coList = new List<CoroutineAbs>();

    //���̺� ���
    public List<string> textbookList = new List<string>(); //���̰� 5���� Ŀ�� �� ����

    public static GameObject StandingGroup;
    public GameObject AnswerGroup;

    public PlayerController PlayerController;
    public PhotonView PV;

    private ChoiceManager ChoiceManager;

    public static TMP_Text ChatText;
    public static TMP_Text CharacterName;
    public static SpriteRenderer ChatWindowSR;
    public static SpriteRenderer HalfStandingSpriteRenderer;

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
        HalfStandingSpriteRenderer = GameObject.Find("HalfStanding").GetComponent<SpriteRenderer>();
        ChatWindowSR = GameObject.Find("VN_TextWindowSample").GetComponent<SpriteRenderer>();
    }

    int i = 0;

    public Coroutine masterControlCoroutine;


    List<Dictionary<string, object>> Dialogue;

    private new void OnEnable()
    {
        ObjectClickHandler.OnObjectClicked += HandleObjectClicked;
    }

    private new void OnDisable()
    {
        ObjectClickHandler.OnObjectClicked -= HandleObjectClicked;
    }

    // �������� ���� �ش��ϴ� ���ο� ��Ʒα� ���� / OutputValue�� ���������� �ٸ� (����� 2�� Correct, 1,3~5�� Incorrect
    private void HandleObjectClicked(string OutputValue)
    {
        Debug.Log(OutputValue);
        nextSubDialogue = OutputValue;
        VNNextScript();
        SetController();
    }

    [PunRPC]
    public void StartVN(string DialogueName)
    {
        if (PlayerController.GetComponent<FixedJoint2D>().enabled)
        {
            PlayerController.ToggleAction();
        }
        VNRunning = true;
        PlayerController.PV.RPC("VNStart", RpcTarget.All);
        Dialogue = CSVReader.Read("VN_DB/" + DialogueName);
        i = 0;
        VNNextScript();
    }

    public void SetController()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            masterControlCoroutine = StartCoroutine(MasterController());
        }
    }


    public void StartNextDialogue()
    {
        
        PV.RPC("StartVN", RpcTarget.All, nextDialogue);
        masterControlCoroutine = StartCoroutine(MasterController());
    }

    public void VNNextScript()
    {

        // Resources ���� ���� �ִ� dialogue ������ List ���·� �ҷ���(CSV Reader �̿�)

        string ActionName = null;
        string Target = null;
        string Parameter = null;
        string[] Questions = {"", "", "", "", ""};
        while (true)
        {
            Debug.Log(i);
            ActionName = Dialogue[i]["ActionName"].ToString();
            Target = Dialogue[i]["Target"].ToString();
            Parameter = Dialogue[i]["Parameter"].ToString();

            if (ActionName == "Break")
            {
                i++;
                break;
            }//�ʿ��Ѱ� ������ �ϴ� �ִ� �ѹ� ���� �ִ� �׼�
            else if (ActionName == "Question")
            {
                int j = i;

                if (PhotonNetwork.IsMasterClient)
                {
                    StopCoroutine(masterControlCoroutine);
                }

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
                i++;
                Debug.Log(i);
                break;
            }//������ �׼�-questionEnd�׼Ƕ��� �ʿ�
            else if (ActionName == "AddTextbook")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    textbookList.Add(Parameter);
                }

                i++;
            }//������ �߰��ϴ� �׼�
            else if (ActionName == "RemoveTextbook")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log(textbookList.Remove(Parameter));
                }

                i++;
            }//������ �����ϴ� ����
            else if (ActionName == "ReadTextbook")
            {

                if (PhotonNetwork.IsMasterClient)
                {
                    StopCoroutine(masterControlCoroutine);
                    for (int j = 0; j < textbookList.Count; j++)
                    {
                        Questions[j] = (j + 1) + ". " + textbookList[j];
                        Debug.Log(Questions[j]);
                    }
                    PV.RPC("StartQuestionCoroutine", RpcTarget.All, (string[])Questions);
                }
                i++;
                break;
            }//���� ������ ����Ʈ���� ������ �������� ����
            else if (ActionName == "SpriteChange")
            {
                // Target = �ٲ� ������Ʈ
                // SpriteChange[0] = �̹��� ��ũ, [1] = x��ǥ, [2] = y��ǥ, [3] = Size
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
                // Target = �̵��� ������Ʈ
                // Parameter[0] = 1ȸ�� �̵��Ÿ�, Parameter[1] = Ƚ��, Parameter[2] = ����
                CoroutineAbs temp = new Move(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "Jump")
            {
                // Target = ������ ������Ʈ
                // Parameter = ���� ��
                CoroutineAbs temp = new Jump(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "Emotion")
            {
                // Target = �̸�� ���� ������Ʈ
                // Parameter[0] = �̸�� �� ������Ʈ, Parameter[1] = ����� ��������Ʈ
                CoroutineAbs temp = new Emotion(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "LetterBox") {
				// Target[0] = �� LetterBox, Target[1] = �Ʒ� LetterBox
				// Parameter == On -> LetterBox On, == Off -> LetterBox Off
				CoroutineAbs temp = new LetterBox(Target, Parameter);
				temp.co = StartCoroutine(temp.Action());
				coList.Add(temp);
				i++;
			}
            else if(ActionName == "ChatWindow")
            {
                CoroutineAbs temp = new ChatWindow(Parameter);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);
                i++;
            } //ê������ ��Ÿ���� �ϰ� ������� �ϴ� �׼� on/off
            else if(ActionName == "Thinking") //�����ڽ� + ê������ �׼�
            {
                CoroutineAbs temp = new Thinking(Target);
                temp.co = StartCoroutine(temp.Action());
                coList.Add(temp);

                CoroutineAbs temp2 = new NormalChat("", "...");
                temp2.co = StartCoroutine(temp2.Action());
                coList.Add(temp2);
                i++;
                break;
                
            }
            else if (ActionName == "Exit")
            {
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = null;

            }
            else if (ActionName == "SmoothSpriteChange")
            {
                // Target = �ٲ�� ������Ʈ
                // Parameter = �ٲ� �̹����� �̸�
                CoroutineAbs temp = new SmoothSpriteChange(Target, Parameter);
                temp.co = StartCoroutine(temp.Action());
                Debug.Log(temp.co);
                coList.Add(temp);
                i++;
            }
            else if (ActionName == "Sound") {
                // Target = ���� �̸�
                // Parameter == 0 -> BGM ���, == 1 -> SE ���, == �� �� -> ��� Sound ����
                VNSoundManager.instance.PlaySound(Target, int.Parse(Parameter));
                i++;
            }
            else if (ActionName == "Lobby")
            {
                //Island,,�������̿÷α� �̸�
                PlayerController.transform.position = new Vector2(-85, -121);
                foreach (Transform ts in GameObject.Find("SignGroup").transform)
                {
                    Destroy(ts.gameObject);
                }
                nextDialogue = Parameter;
                VNRunning = false;
                i++;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }
                break;
            }
            else if (ActionName == "AddTide")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //����, ����
                    string[] a = Parameter.Split('`');
                    //a[0] = ����
                    //a[1] = �������� ���� (�ѹ��� ���� ��ȯ�ϱ� ����)
                    //a[2] = ���� Tidal ������ �ð�

                    //����=0�϶� ���� ������
                    //����=1�϶� �Ϲ� ������
                    //����=2�϶� ����� ������
                    //����=3�϶� ��� ũ����
                    //����=4�϶� ���� ũ����
                    //����=5�϶� �հ� ũ����
                    //����ð�, Tidal������ �ð�


                    if (int.Parse(a[0]) == 0)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(Vector3.zero, 0, float.Parse(a[2]));
                    }
                    else if (int.Parse(a[0]) == 1)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        for (int w = 0; w < int.Parse(a[1]); w++)
                        {
                            //�� ������Ʈ���� �����Ϳ��Ը� ������
                            GameObject tsunami = new GameObject();
                            tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                            tsunami.AddComponent<TsunamiObject>();
                            tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(new Vector3(vec.x, vec.y), 10, float.Parse(a[2]));
                        }
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                    else if (int.Parse(a[0]) == 2)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        Vector2 vec2 = -vec;
                        for (int w = 0; w < int.Parse(a[1]); w++)
                        {
                            //�� ������Ʈ���� �����Ϳ��Ը� ������
                            GameObject tsunami = new GameObject();
                            tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                            tsunami.AddComponent<TsunamiObject>();
                            tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(new Vector3(vec.x, vec.y), 10, float.Parse(a[2]));
                            tsunami.GetComponent<TsunamiObject>().SummonFirstTsunami(new Vector3(vec2.x, vec2.y), 10, float.Parse(a[2]));
                        }
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                        PV.RPC("AddSign", RpcTarget.All, vec2 + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                    else if (int.Parse(a[0]) == 3)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonCreature("Shark", new Vector3(vec.x, vec.y), int.Parse(a[1]), float.Parse(a[2]));
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                    else if (int.Parse(a[0]) == 4)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonCreature("Orca", new Vector3(vec.x, vec.y), int.Parse(a[1]), float.Parse(a[2]));
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                    else if (int.Parse(a[0]) == 5)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 40;
                        GameObject tsunami = new GameObject();
                        tsunami.transform.parent = GameObject.Find("TsunamiGroup").transform;
                        tsunami.AddComponent<TsunamiObject>();
                        tsunami.GetComponent<TsunamiObject>().SummonCreature("BlueWhale", new Vector3(vec.x, vec.y), int.Parse(a[1]), float.Parse(a[2]));
                        PV.RPC("AddSign", RpcTarget.All, vec + new Vector2(GameObject.Find("Island").transform.position.x, GameObject.Find("Island").transform.position.y));
                    }
                }
                i++;
            }
            else if (ActionName == "Island")
            {
                //Island,�ð�(��),�������̿÷α� �̸�
                //���� �ʱ�ȭ
                //Ÿ�̸� ����
                //���̿÷α� �̸��� ���ؼ� �����Ϳ��Ը� ����
                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject.Find("Shop").GetComponent<Shop>().InitializeShop();
                    GameObject.Find("TideTimer").GetComponent<TideTimer>().TimeSetAndStart(int.Parse(Target));
                    if (Parameter != "")
                    {
                        nextDialogue = Parameter;
                    }
                    

                    GameObject[] temp = FindObjectsOfType<GameObject>();
                    foreach (GameObject go in temp)
                    {
                        if (go.GetComponent<Block>() != null)
                        {
                            go.GetComponent<Block>().StartObject();
                        }
                    }
                }
                GameObject.Find("TideTimer").GetComponent<TideTimer>().totalTime = int.Parse(Target);
                PlayerController.transform.position += GameObject.Find("IslandSquare").transform.position - GameObject.Find("LobbySquare").transform.position;
                VNRunning = false;
                i++;
                break;
            }//�Ķ���Ͱ� ���ٸ� �׳� �״�� �������ϴ�
            else if (ActionName == "AddShopItem")
            {
                //AddShopItem,,(������ �̸�)
                GameObject.Find("Shop").GetComponent<Shop>().shopItemList.Add(Parameter);
                i++;
            }
            else if (ActionName == "GetItem")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate("Prefab/Game/" + Parameter, GameObject.Find("IslandSquare").transform.position, Quaternion.identity).transform.SetParent(GameObject.Find("SaveObjectGroup").transform);
                }
                i++;

            }
            else if (ActionName == "GetMoney") {
				if (PhotonNetwork.IsMasterClient) {
					GameObject.Find("TotalMoney").GetComponent<MoneyManager>().PV.RPC("MoneyChange", RpcTarget.MasterClient, int.Parse(Parameter));
				}
                i++;
			}
            else if (ActionName == "ConnectDialogue")
            {
                Dialogue = CSVReader.Read("VN_DB/" + Parameter);
                i = 0;
            }
            else if (ActionName == "ConnectSubDialogue")
            {
                Dialogue = CSVReader.Read("VN_DB/" + nextSubDialogue);
                i = 0;
            }//���� ��Ʒα״� ���� �ȵ˴ϴ�... �������ʿ䵵 ���� �������ϱ���
            else if (ActionName == "ConnectNextDialogue")
            {
                Dialogue = CSVReader.Read("VN_DB/" + nextDialogue);
                i = 0;
            }
            else if (ActionName == "SetSubDialogue")
            {
                nextSubDialogue = Parameter;
                i++;
            }
            else if (ActionName == "SetNextDialogue")
            {
                nextDialogue = Parameter;
                i++;
            }
            else if (ActionName == "Transition")
            {
                void TransitionSub()
                {
                    VNNextScript();
                    if (PhotonNetwork.IsMasterClient)
                    {
                        masterControlCoroutine = StartCoroutine(MasterController());
                    }
                    TransitionManager.Instance().onTransitionCutPointReached -= TransitionSub;
                }

                TransitionManager.Instance().onTransitionCutPointReached += TransitionSub;
                if (PhotonNetwork.IsMasterClient)
                {
                    StopCoroutine(masterControlCoroutine);
                }
                if (Parameter == "fade")
                {
                    TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0f);
                } else if(Parameter == "rectangle")
                {
                    TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().rectangleGrid, 0f);
                }
                else if (Parameter == "circle")
                {
                    TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().circleWipe, 0f);
                } else
                {
                    TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0f);
                }

                i++;
                break;
            }//fade, rectangle, circle
            else if (ActionName == "ShopOpen")
            {
                GameObject.Find("ShopParent").transform.GetChild(0).gameObject.SetActive(true);
                i++;
            }
            else
            {
                HalfStandingChange(Parameter);
                Debug.Log(Target);
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
            StartCoroutine(VNSoundManager.instance.StopSound());
        }
    }

    /// <summary>
    /// narrator : ��� �̸� | narration : ���
    /// </summary>
    /// <param name="narrator"></param>
    /// <param name="narration"></param>
    /// <returns></returns>
    /// 
    //������ ���� ����
    [PunRPC]
    void StartQuestionCoroutine(string[] Questions)
    {
        int a = 0;
        for(int i=0; i<5; i++)
        {
            if (Questions[i] != "")
            {
                a++;
            } else
            {
                break;
            }
        }
        Debug.Log(a);
        StartCoroutine(ChoiceManager.MakeChoice(a, Questions));
    }

    //�����͸� �̰� �����ϵ��� �ϼ���.
    IEnumerator MasterController()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    PV.RPC("StopAllCoAndNextScript", RpcTarget.All);
                    yield return new WaitForSeconds(0.5f);
                } else if (Input.GetMouseButton(2))
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
        go.transform.SetParent(GameObject.Find("SignGroup").transform);
        go.GetComponent<Sign>().target = vec;
        go.transform.localScale = new Vector3(20, 20, 1);
    }



    // ��ݽ� sprite�� �ٲ۴�.
          // ���� �̹���

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

            // Ÿ���� ȿ��
            for (int i = 0; i < narration.Length; i++)
            {
                if (narration[i] == '<') {
                    while (narration[i] != '>') {
						writerText += narration[i];
                        i++;
					}
                    ChatText.text = writerText;
                }
                writerText += narration[i];
                ChatText.text = writerText;

                yield return new WaitForSecondsRealtime(0.03f);
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
            float a = 0;
            Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + Parameter);
            Transform standing = StandingGroup.transform.GetChild(int.Parse(Target));
            standing.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = newSprite;
            standing.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            standing.GetChild(0).position = standing.position;
            standing.GetChild(0).localScale = new Vector3(1, 1, 1);

            while (a < 0.3f)
            {
                a += Time.deltaTime;
                standing.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1-a*2);
                standing.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f+a*2);
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
            // move[0] = 1ȸ�� �̵��Ÿ�, move[1] = Ƚ��, move[2] = ����
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
                    // ���� �������� ���鼭 StartPosition + 1ȸ �̵��Ÿ����� Ŀ�� �� or ���� �������� ���鼭 StartPosition - 1ȸ �̵��Ÿ����� �۾��� �� break
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
        // parameter = ������ ���ӵ� = ���̸� ������
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
            // Bubble[0] = ǥ���ϴ� ���ĵ� ��ȣ, Bubble[1] = ����� ��������Ʈ
            Bubble = Parameter.Split(',');
        }

        public override IEnumerator Action()
        {
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + Bubble[1]);
            StandingGroup.transform.GetChild(int.Parse(Target)).position = StandingGroup.transform.GetChild(int.Parse(Bubble[0])).position + new Vector3(-1, 1, 0);
            StandingGroup.transform.GetChild(int.Parse(Target)).rotation = Quaternion.Euler(new Vector3(0, 0, 25));
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            while (StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color.a <= 1)
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

    public class LetterBox : CoroutineAbs {

        string Target;
        string Parameter;
        string[] temp;
        int TopLetterBox, BottomLetterBox;
        public LetterBox(string Target, string Parameter) {
            this.Target = Target;
            this.Parameter = Parameter;
			temp = Target.Split('`');
            TopLetterBox = int.Parse(temp[0]);
            BottomLetterBox = int.Parse(temp[1]);
        }

		public override IEnumerator Action() {
            float timer = 0;
            if(Parameter == "On") {
				StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
				StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
				StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 6.5f, 0f);
				StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -6.5f, 0f);
				StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
				StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);

                timer = 0;
                while (timer <= 1) {
                    timer += Time.deltaTime;
					StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(6.5f,4.81f,timer), 0f);
					StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(-6.5f, -4.81f, timer), 0f);
					yield return null;
                }
			}
            else if(Parameter == "Off") {
                timer = 1;
                while (timer >= 0) {
                    timer -= Time.deltaTime;
                    StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(6.5f, 4.81f, timer), 0f);
                    StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(-6.5f, -4.81f, timer), 0f);
                    yield return null;
                }

				StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = null;
                StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = null;
			}
		}

		override public void EndAction() {
			if (Parameter == "On") {
				StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
				StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
				StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 4.81f, 0f);
				StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -4.81f, 0f);
				StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
				StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
			}
			else if (Parameter == "Off") {
				StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = null;
				StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = null;
			}
		}
	}
    public class ChatWindow : CoroutineAbs
    {
        float alpha = 0.7f;
        string Parameter;
        public ChatWindow(string Parameter)
        {
            this.Parameter = Parameter;
        }

        public override IEnumerator Action()
        {
            float timer = 0;
            if (Parameter == "On")
            {
                while (timer <= 0.5f)
                {
                    timer += Time.deltaTime;
                    ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, timer * 2*alpha);
                    HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, timer * 2);
                    ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, timer*2);
                    CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, timer * 2);
                    yield return null;
                }
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
            }
            else if (Parameter == "Off")
            {
                while (timer <= 0.5f)
                {
                    timer += Time.deltaTime;
                    ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha*(1- timer * 2));
                    HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1 - timer * 2);
                    ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1-timer * 2);
                    CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1-timer * 2);
                    yield return null;
                }
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, 0);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 0);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 0);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 0);
            }
        }

        override public void EndAction()
        {
            if (Parameter == "On")
            {
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
            }
            else if (Parameter == "Off")
            {
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, 0);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 0);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 0);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 0);
            }
        }
    }

    public class Thinking : CoroutineAbs
    {
        float alpha = 0.5f;
        string Target;
        static bool isOnAction = false; //������ �ݴ°� ��� ��������
        int TopLetterBox, BottomLetterBox;
        public Thinking(string Target)
        {
            
                this.Target = Target;
                string[] temp;
                temp = Target.Split('`');
                TopLetterBox = int.Parse(temp[0]);
                BottomLetterBox = int.Parse(temp[1]);
        }

        public override IEnumerator Action()
        {
            isOnAction ^= true;
            float timer = 0;
            if (isOnAction)
            {
                StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
                StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
                StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 6.5f, 0f);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -6.5f, 0f);
                StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, 0);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 0);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 0);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 0);
                while (timer <= 1)
                {
                    timer += Time.deltaTime;
                    StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(6.5f, 4.81f, timer), 0f);
                    StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(-6.5f, -4.81f, timer), 0f);
                    yield return null;
                }
                StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 4.81f, 0f);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -4.81f, 0f);
                StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
            }
            else
            {
                StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
                StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/LetterBox");
                StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 6.5f, 0f);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -6.5f, 0f);
                StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, 0);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 0);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 0);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 0);
                while (timer <= 1)
                {
                    timer += Time.deltaTime;
                    StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(6.5f, 4.81f, 1 - timer), 0f);
                    StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, Mathf.Lerp(-6.5f, -4.81f, 1 - timer), 0f);
                    yield return null;
                }
                StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 4.81f, 0f);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -4.81f, 0f);
                StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
                StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = null;
                StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = null;

            }

        }
        override public void EndAction()
        {
            if (isOnAction) {
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
                StandingGroup.transform.GetChild(TopLetterBox).transform.position = new Vector3(0f, 4.81f, 0f);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.position = new Vector3(0f, -4.81f, 0f);
                StandingGroup.transform.GetChild(TopLetterBox).transform.localScale = new Vector3(1, 1, 1);
                StandingGroup.transform.GetChild(BottomLetterBox).transform.localScale = new Vector3(1, 1, 1);
            }
            else {
                ChatWindowSR.color = new Color(ChatWindowSR.color.r, ChatWindowSR.color.g, ChatWindowSR.color.b, alpha);
                HalfStandingSpriteRenderer.color = new Color(HalfStandingSpriteRenderer.color.r, HalfStandingSpriteRenderer.color.g, HalfStandingSpriteRenderer.color.b, 1);
                ChatText.color = new Color(ChatText.color.r, ChatText.color.g, ChatText.color.b, 1);
                CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
                StandingGroup.transform.GetChild(TopLetterBox).GetComponent<SpriteRenderer>().sprite = null;
                StandingGroup.transform.GetChild(BottomLetterBox).GetComponent<SpriteRenderer>().sprite = null;
        }
        }
    }
}
