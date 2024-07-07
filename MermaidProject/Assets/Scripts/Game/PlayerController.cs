using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Block
{
    int b = 0;
    // Start is called before the first frame update
    [SerializeField] Color red;
    [SerializeField] Color yellow;
    [SerializeField] Color green;
    [SerializeField] Color blue;
    Block grabBlock;

    public SpriteRenderer SR;
    public override void Awake()
    {
        base.Awake();
        ChangeColor(b);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            float h1 = Input.GetAxis("Horizontal");
            float h2 = Input.GetAxis("Vertical");
            float r1 = Input.GetAxis("Mouse X");
            float r2 = Input.GetAxis("Mouse Y");



            GetComponent<Rigidbody2D>().AddTorque(-r1 * Time.deltaTime * 4, ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().AddRelativeForce(15 * new Vector2(h1, h2) * Time.deltaTime, ForceMode2D.Impulse);



            transform.GetChild(1).transform.localPosition = Vector2.Lerp(transform.GetChild(1).transform.localPosition, new Vector2(h1, h2).normalized * 4, Time.deltaTime * 3);

            if (Input.GetMouseButtonDown(0))
            {
                //PV.RPC("ToggleAction", RpcTarget.AllBuffered);
                ToggleAction();
            }
            if (Input.GetMouseButtonDown(1) && GetComponent<FixedJoint2D>().enabled)
            {
                UseAction();
            }
        }
    }
    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.GetChild(1).position + new Vector3(0, 0, -10), Time.deltaTime * 3);
            //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 2);
            Camera.main.transform.rotation = transform.rotation;
        }

    }
    public void UseAction()
    {
        if (GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().isReady)
        {
            GetComponent<FixedJoint2D>().connectedBody.GetComponent<Block>().UseItem();
        }
    }


    [PunRPC]
    public void ChangeColor(int a)
    {
        //0=r 1=y 2=g 3=b
        switch (a)
        {
            case 0:
                GetComponent<SpriteRenderer>().color = red;
                break;
            case 1:
                GetComponent<SpriteRenderer>().color = yellow;
                break;
            case 2:
                GetComponent<SpriteRenderer>().color = green;
                break;
            case 3:
                GetComponent<SpriteRenderer>().color = blue;
                break;
        }
        b = a;
    }

    //[PunRPC]

    //±×·¦¾×¼Ç -> fixedjoint --- ±×·¦¾×¼Ç -> fixedjoint off
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
                hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("BluePrint"));
            }
            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.position, transform.up, 1.5f, LayerMask.GetMask("Building"));
            }
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Block>().isAbleGrabed && hit.collider.GetComponent<Block>().p1 == null)
                {
                    hit.collider.GetComponent<Block>().Grabed(this);
                    GetComponent<FixedJoint2D>().enabled = true;
                    GetComponent<FixedJoint2D>().connectedBody = hit.collider.GetComponent<Rigidbody2D>();
                }
            }
        }
    }

}
