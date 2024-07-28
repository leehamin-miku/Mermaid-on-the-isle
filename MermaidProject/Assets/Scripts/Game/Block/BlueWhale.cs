using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWhale : TsunamiUnit
{
    private Vector3 BlueWhalePosition;
    private Vector3 FlowerPosition;
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        StartCoroutine(BlueWhaleAi());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Block>() != null&& collision.collider.GetComponent<Block>().BlockCode!=0&& collision.collider.GetComponent<TsunamiUnit>() == null)
        {
            collision.collider.GetComponent<Block>().PV.RPC("ChangeStrength", Photon.Pun.RpcTarget.All, -2);
        }
    }

    private IEnumerator BlueWhaleAi()
    {
        BlueWhalePosition = transform.position;
        FlowerPosition = GameObject.Find("Island").transform.position;
        float Power = 500f;
        while (true)
        {
            BlueWhalePosition = transform.position;
            rb.AddForce((FlowerPosition - BlueWhalePosition).normalized * Power, ForceMode2D.Impulse);
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }        
    }
}
