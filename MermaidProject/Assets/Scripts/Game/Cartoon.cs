using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Cartoon : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] private Slider timeBar;
    // Start is called before the first frame update
    float a = 0;
    public float b {
        set; private get;
    }
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
        }
        a += Time.deltaTime;
        timeBar.value = a / b;
        transform.rotation = Camera.main.transform.rotation;
    }
}
