using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color red;
    [SerializeField] Color yellow;
    [SerializeField] Color green;
    [SerializeField] Color blue;


    public PhotonView PV;
    public SpriteRenderer SR;
    void Start()
    {
        GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            float h1 = Input.GetAxis("Horizontal");
            float h2 = Input.GetAxis("Vertical");
            GetComponent<Rigidbody2D>().AddTorque(-h1 * Time.deltaTime * 2, ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().AddForce(transform.up * h2 * Time.deltaTime * 15, ForceMode2D.Impulse);
        }
    }
    private void FixedUpdate()
    {
        if (PV.IsMine) {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, 0, -10), Time.deltaTime * 4);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 2);
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
    }

}
