using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class GraShoes : Block
{
    [SerializeField] float shoesForce;
    [SerializeField] Color color;
    //플레이어의 공기저항과 관련되어 있으니 꼭 확인해야함
    public void Update()
    {
        if (isInUse&&p1!=null)
        {
            p1.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * shoesForce);
        }   
    }

    public override void UseDownAction()
    {
        base.UseDownAction();
        p1.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * shoesForce, ForceMode2D.Impulse);
    }

    public override void GrabedAction()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            GetComponent<Collider2D>().enabled = false;

            p1.GetComponent<FixedJoint2D>().enabled = false;

            rb.rotation = p1.rb.rotation;
            transform.position = p1.transform.position;

            p1.GetComponent<FixedJoint2D>().enabled = true;

            p1.GetComponent<Rigidbody2D>().drag = 3;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            p1.GetComponent<FixedJoint2D>().connectedBody = null;
            transform.position = p1.transform.position;
            p1.GetComponent<FixedJoint2D>().connectedBody = rb;
            p1.GetComponent<Rigidbody2D>().drag = 5;
        }
    }

}
