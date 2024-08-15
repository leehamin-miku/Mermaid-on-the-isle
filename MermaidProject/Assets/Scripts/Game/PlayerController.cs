using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using EasyTransition;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Block
{
    public int colorNumber = 0;
    public float mouseSensitivity = 4;
	public bool isAbleMove = true;
    public bool isFocusOnChattingInputField = false;
    bool VNRunning = false;
    bool isMatser;
    // Start is called before the first frame update
    [SerializeField] Color red;
    [SerializeField] Color yellow;
    [SerializeField] Color green;
    [SerializeField] Color blue;
    Sprite[] spriteArr = new Sprite[8];

    public SpriteRenderer SR;

    public SpriteRenderer body;
    public SpriteRenderer body2;
    public SpriteRenderer star;



    int j = 0;
    float a = 0;
    public override void Awake()
    {
        base.Awake();
        for (int i = 0; i < 5; i++)
        {
            spriteArr[i] = Resources.Load<Sprite>("Image/Game/≥Îæ∆≈æ∫‰" + i);
        }
        spriteArr[5] = Resources.Load<Sprite>("Image/Game/≥Îæ∆≈æ∫‰3");
        spriteArr[6] = Resources.Load<Sprite>("Image/Game/≥Îæ∆≈æ∫‰2");
        spriteArr[7] = Resources.Load<Sprite>("Image/Game/≥Îæ∆≈æ∫‰1");
        body = transform.GetChild(0).GetComponent<SpriteRenderer>();
        body2 = transform.GetChild(2).GetComponent<SpriteRenderer>();
        star = transform.GetChild(3).GetComponent<SpriteRenderer>();
        ChangeColor(colorNumber);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

		

		if (PV.IsMine)
        {
            GameObject.Find("TotalMoney").GetComponent<MoneyManager>().PV.RPC("MoneyMarkRequest", RpcTarget.MasterClient);
            GameObject.Find("VN").GetComponent<VNManager>().PlayerController = this;
            GameObject.Find("GameManager").GetComponent<ChattingManager>().PlayerController = this;
            GameObject.Find("GameManager").GetComponent<ChattingManager>().PV.RPC("SystemChatting", RpcTarget.All, "<color=yellow>" + PhotonNetwork.NickName + "¥‘¿Ã ¿‘¿Â«ﬂΩ¿¥œ¥Ÿ</color>");
        }

        GameObject Datamanager = GameObject.Find("DataManager");
        mouseSensitivity = DataManager.Instance.data.MS;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine&&VNRunning==false)
        {
            if (isAbleMove)
            {
                float h1 = 0, h2 = 0, r1 = 0, r2 = 0;
                if (!GameObject.Find("GameManager").GetComponent<ChattingManager>().isFocused)
                {
                    h1 = Input.GetAxis("Horizontal");
                    h2 = Input.GetAxis("Vertical");
                    r1 = Input.GetAxis("Mouse X");
                }

                if (h2 > 0) h2 = 1.5f;

                rb.AddTorque(-r1 * Time.deltaTime * mouseSensitivity, ForceMode2D.Impulse);
                GetComponent<Rigidbody2D>().AddRelativeForce(15 * new Vector2(h1, h2) * Time.deltaTime, ForceMode2D.Impulse);
                if (Input.GetMouseButtonDown(0))
                {
                    //PV.RPC("ToggleAction", RpcTarget.AllBuffered);
                    ToggleAction();
                }
            }



            if (GetComponent<FixedJoint2D>().enabled)
            {
                if (Input.GetMouseButtonUp(1) && GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().isInUse)
                {
                    GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().UseUpAction();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().UseDownAction();
                }
            }
        }
        if (rb.velocity.magnitude > 0.3f)
        {
            a += Time.deltaTime;
        }

        if (a > 0.3f)
        {
            a -= 0.3f;
            j++;
            j %= 8;
            body.sprite = spriteArr[j];
            body2.sprite = spriteArr[j];
        }
    }
    private void FixedUpdate()
    {
        if (PV.IsMine&&VNRunning==false)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.GetChild(1).position + new Vector3(0, 0, -10), Time.deltaTime * 3);
            //Camera.main.transform.position = transform.GetChild(1).position + new Vector3(0, 0, -10);
            //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 2);
            Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);
            Camera.main.transform.rotation = transform.rotation;
        }

    }



    [PunRPC]
    public void ChangeColor(int a)
    {
        //0=r 1=y 2=g 3=b
        switch (a)
        {
            case 0:
                star.color = red;
                break;
            case 1:
                star.color = yellow;
                break;
            case 2:
                star.color = green;
                break;
            case 3:
                star.color = blue;
                break;
        }
        colorNumber = a;
    }

    [PunRPC]
    public void Teleport(Vector3 position)
    {
        Vector3 pos = Vector3.zero;
        if (GetComponent<FixedJoint2D>().enabled)
        {
            pos = GetComponent<FixedJoint2D>().GetComponent<FixedJoint2D>().connectedBody.transform.position;
            pos -= transform.position;
        }
        transform.position = position;
        if (GetComponent<FixedJoint2D>().enabled)
        {
            GetComponent<FixedJoint2D>().connectedBody.transform.position = position+pos;
        }

    }
    //[PunRPC]

    //±◊∑¶æ◊º« -> fixedjoint --- ±◊∑¶æ◊º« -> fixedjoint off
    public void ToggleAction()
    {
        if (GetComponent<FixedJoint2D>().enabled)
        {
            GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().Grabed(this);
            GetComponent<FixedJoint2D>().connectedBody = null;
            GetComponent<FixedJoint2D>().enabled = false;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Block"));
            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Tool"));
            }
            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Blueprint"));
            }
            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Building"));
            }
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Block>().isAbleGrabed && hit.collider.GetComponent<Block>().isGrabed == false)
                {
                    hit.collider.GetComponent<Block>().Grabed(this);
                    GetComponent<FixedJoint2D>().enabled = true;
                    GetComponent<FixedJoint2D>().connectedBody = hit.collider.GetComponent<Rigidbody2D>();
                }
            }
        }
    }

    [PunRPC]
    public void VNStart()
    {
        if (PV.IsMine)
        {
            VNRunning = true;
            TransitionManager.Instance().onTransitionCutPointReached += VNStartSub;
            TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0f);
        }
    }

    [PunRPC]
    public void VNStartSub()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.transform.rotation = Quaternion.identity;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        TransitionManager.Instance().onTransitionCutPointReached -= VNStartSub;
    }

    [PunRPC]
    public void VNEndSub()
    {
        if (PV.IsMine)
        {
            Debug.Log("VN≥°");
            Camera.main.transform.position = transform.position;
            VNRunning = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            TransitionManager.Instance().onTransitionCutPointReached -= VNEndSubSub;
        }
        //if (PV.IsMine)
        //{
        //    TransitionManager.Instance().onTransitionCutPointReached += VNEndSubSub;
        //    TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 0.2f);
        //}
    }

    void VNEndSubSub()
    {
        Debug.Log("VN≥°");
        Camera.main.transform.position = transform.position;
        VNRunning = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TransitionManager.Instance().onTransitionCutPointReached -= VNEndSubSub;
    }

}