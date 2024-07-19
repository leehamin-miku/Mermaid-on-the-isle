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


    //���������׼� ȣ��Ǵ� �Լ�
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


    //������ ��¼�� ���
}
