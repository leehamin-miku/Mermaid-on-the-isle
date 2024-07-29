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
    public void ChangeHP(int a)
    {
        HP += a;
        PV.RPC("AttackedSub", RpcTarget.All);
        if (HP <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    [PunRPC]
    public void AddRB(Vector2 vec)
    {
        rb.AddForce(vec, ForceMode2D.Impulse);
    }

    [PunRPC]
    public void AttackedSub()
    {
        StartCoroutine(Attacked());
    }

    public IEnumerator Attacked()
    {
        float a = 0;
        while (a < 0.3)
        {
            a += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    //������ ��¼�� ���
}
