using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orca : TsunamiUnit
{
    private GameObject ClosestPlayer;
    private Vector3 OrcaPosition;
    private Vector3 FlowerPosition;
    public override void StartTunamiUnit()
    {
        base.StartTunamiUnit();
        StartCoroutine(OrcaAi());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Block>() != null&& collision.collider.GetComponent<Block>().BlockCode!=0&& collision.collider.GetComponent<TsunamiUnit>() == null)
        {
            collision.collider.GetComponent<Block>().PV.RPC("ChangeStrength", Photon.Pun.RpcTarget.All, -4);
            ChangeHP(-1);
        } else if(collision.collider.GetComponent<CannonBall>() != null)
        {
            ChangeHP(-30);
        }
    }

    AudioSource soundEffect;
    private IEnumerator OrcaAi()
    {
        soundEffect = GetComponent<AudioSource>();
        Vector3 ClosestPlayerPosition;
        FlowerPosition = GameObject.Find("Island").transform.position;
        float DetectDistance = 2f;
        float Power = 45f;
        while (true)
        {
            StartCoroutine(FindClosestPlayerPosition());
            ClosestPlayerPosition = ClosestPlayer.transform.position;
            if (Vector3.Distance(ClosestPlayerPosition, OrcaPosition) < DetectDistance)
            {
                rb.AddForce((ClosestPlayerPosition - OrcaPosition).normalized * Power, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce((FlowerPosition - OrcaPosition).normalized * Power, ForceMode2D.Impulse);
            }
            soundEffect.Play();
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }        
    }

    private IEnumerator FindClosestPlayerPosition()
    {
        OrcaPosition = transform.position;
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        float ClosestDistance = 99999f;

        foreach (GameObject p in Players)
        {
            Vector3 PlayerPosition = p.transform.position;
            float distance = Vector3.Distance(OrcaPosition, PlayerPosition);
            if (distance < ClosestDistance)
            {
                ClosestPlayer = p;
                ClosestDistance = distance;
            }
        }
        yield return null;
    }
}
