using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiUnit : Block
{
    [SerializeField] int HP;
    // Start is called before the first frame update
    public virtual void StartTunamiUnit()
    {
        
    }


    //마스터한테서 호출되는 함수
    [PunRPC]
    public void ChangeHp(int a)
    {
        HP += a;
        if (HP <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    [PunRPC]
    public void AddRB(Vector2 vec)
    {
        rb.AddForce(vec);
    }


    //닿으면 어쩌구 등등
}
