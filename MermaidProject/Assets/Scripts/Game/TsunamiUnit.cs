using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiUnit : Block
{
    [SerializeField] int HP;
    Coroutine coroutine;
    // Start is called before the first frame update
    public virtual void StartTunamiUnit()
    {
        
    }


    //마스터한테서 호출되는 함수
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
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(Attacked());
    }

    public IEnumerator Attacked()
    {
        float a = 0;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
        while (a < 0.3)
        {
            a += Time.deltaTime;
            yield return null;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        coroutine = null;
    }

    IEnumerator DestroyDelay(float a)
    {
        float b = 0;
        while (b <= a)
        {
            b+= Time.deltaTime; yield return null;
        }
        PhotonNetwork.Destroy(gameObject);

    }

    //닿으면 어쩌구 등등
}
