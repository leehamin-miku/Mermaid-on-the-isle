using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public void Shot()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * 20, ForceMode2D.Impulse);
        StartCoroutine(DestroyDelay(5f));
    }

    IEnumerator DestroyDelay(float a)
    {
        float b = 0;
        while (b < a)
        {
            b+= Time.deltaTime;
            yield return null;
        }
        PhotonNetwork.Destroy(this.gameObject);
    }
}
