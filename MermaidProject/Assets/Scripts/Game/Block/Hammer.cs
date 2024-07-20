using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Hammer : Block
{
    public bool isBuilding;
    public Blueprint blueprint;
    [SerializeField] GameObject cartoonPrefab;
    GameObject cartoon;
    //�÷��̾��� �������װ� ���õǾ� ������ �� Ȯ���ؾ���


    public override void UseUpAction()
    {
        base.UseUpAction();
        

    }
    public override void GrabedAction()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            GetComponent<Collider2D>().enabled = false;

            p1.GetComponent<FixedJoint2D>().enabled = false;

            rb.rotation = p1.rb.rotation;
            transform.position = p1.transform.position+p1.transform.up;

            p1.GetComponent<FixedJoint2D>().enabled = true;

        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            p1.GetComponent<FixedJoint2D>().connectedBody = null;
            transform.position = p1.transform.position;
            p1.GetComponent<FixedJoint2D>().connectedBody = rb;
        }
    }

}
