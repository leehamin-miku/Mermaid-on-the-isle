using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class VNManager : MonoBehaviourPunCallbacks
{
    //�����Ͱ� ����ϴ� ����
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

    bool isSkip;
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

    // �������� ���� �ش��ϴ� ���ο� ��Ʒα� ���� / OutputValue�� ���������� �ٸ� (����� 2�� Correct, 1,3~5�� Incorrect
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
        
        // Resources ���� ���� �ִ� dialogue ������ List ���·� �ҷ���(CSV Reader �̿�)
 
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
                // SpriteChange[0] = �̹��� ��ũ, [1] = x��ǥ, [2] = y��ǥ, [3] = Size
                string[] SpriteChange = Parameter.Split('`');

                Vector3 spawnPosition = new Vector3(float.Parse(SpriteChange[1]), float.Parse(SpriteChange[2]), 0);
                Vector3 Scale = new Vector3(float.Parse(SpriteChange[3]), float.Parse(SpriteChange[3]), float.Parse(SpriteChange[3]));
                StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + SpriteChange[0]);
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.localScale = Scale;
                StandingGroup.transform.GetChild(int.Parse(Target)).transform.position = spawnPosition;
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
                //Island,,�������̿÷α� �̸�
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
                    //����, ����
                    string[] a = Parameter.Split('`');
                    //a[0] = ����
                    //a[1] = �������� ���� (�ѹ��� ���� ��ȯ�ϱ� ����)
                    //a[2] =  ���� Tidal ������ �ð�

                    //����=0�϶� �Ϲ� ������
                    //����=1�϶� ���ξ�����
                    //����=2�϶� ��� ũ����

                    //����ð�, Tidal������ �ð�

                    if (int.Parse(a[0]) == 0)
                    {
                        Vector2 vec = Random.insideUnitCircle.normalized * 30;
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
                //Island,�ð�(��),�������̿÷α� �̸�
                //���� �ʱ�ȭ
                //Ÿ�̸� ����
                //���̿÷α� �̸��� ���ؼ� �����Ϳ��Ը� ����
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
    /// narrator : ��� �̸� | narration : ���
    /// </summary>
    /// <param name="narrator"></param>
    /// <param name="narration"></param>
    /// <returns></returns>
    /// 
    

    
    //�����͸� �̰� �����ϵ��� �ϼ���.
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



    // ��ݽ� sprite�� �ٲ۴�.
    public SpriteRenderer HalfStandingSpriteRenderer;       // ���� �̹���

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

            // Ÿ���� ȿ��
            for (int i = 0; i < narration.Length; i++)
            {
                writerText += narration[i];
                ChatText.text = writerText;

                // delay(0.1��)���� �� ���� ���, �߰��� space or ��Ŭ�� ������ ���� ���
                // ���� Ÿ���� �ӵ� ��ȭ�� �ʿ��ϴٸ�, delay�� ���� �޾Ƽ� ���� �� ��
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
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/VN/" + Parameter); ;
            StandingGroup.transform.GetChild(int.Parse(Target)).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            StandingGroup.transform.GetChild(int.Parse(Target)).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
    }


}
