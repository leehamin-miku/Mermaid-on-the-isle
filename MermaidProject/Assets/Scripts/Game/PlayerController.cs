using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
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
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, 0, -10), Time.deltaTime * 4);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 2);
    }
    //[PunRPC]
    //public void ChangeColor(int a)
    //{
        
    //}
    
}
