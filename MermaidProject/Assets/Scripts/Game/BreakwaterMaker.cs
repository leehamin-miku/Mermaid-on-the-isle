using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BreakwaterMaker : Block
{
    [SerializeField] bool isOn;
    [SerializeField] float power;
    [SerializeField] GameObject NextBuoy;
    [SerializeField] Color OnLIGHT;
    [SerializeField] Color OffLIGHT;



    public override void Awake()
    {
        base.Awake();
        if (isOn)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = OnLIGHT;
            transform.GetChild(2).GetComponent<Light2D>().intensity = 0.5f;
        }
        else
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = OffLIGHT;
            transform.GetChild(2).GetComponent<Light2D>().intensity = 0.1f;
        }
    }
    public override void CollisionEnterAction(Collision2D collision)
    {
        base.CollisionEnterAction(collision);
        if(collision.collider.GetComponent<Block>().BlockCode == 0)
        {
            if (isOn)
            {
                //충돌자에게 점수주기
                PV.RPC("LightOff", RpcTarget.All);
                NextBuoy.GetComponent<Buoy>().PV.RPC("LightOn", RpcTarget.All);
            }
        }
        collision.collider.GetComponent<Rigidbody2D>().AddForce((collision.collider.transform.position - transform.position).normalized * power, ForceMode2D.Impulse);

    }

    [PunRPC]
    public void LightOn()
    {
        isOn = true;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = OnLIGHT;
        transform.GetChild(2).GetComponent<Light2D>().intensity = 0.5f;
    }
    [PunRPC]
    public void LightOff()
    {
        isOn = true;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = OffLIGHT;
        transform.GetChild(2).GetComponent<Light2D>().intensity = 0.1f;
    }


}
