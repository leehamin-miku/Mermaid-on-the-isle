using EasyTransition;
using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleporter : Block
{
    [SerializeField] Vector3 targetPosition;
    Collider2D collision;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>().BlockCode == 0)
        {
            this.collision = collision;
            if (collision.GetComponent<Block>().PV.IsMine)
            {
                TransitionManager.Instance().onTransitionCutPointReached += TransitionTeleportFuc;
                TransitionManager.Instance().Transition(GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().rectangleGrid, 0f);
            }
        }
    }

    private void TransitionTeleportFuc()
    {
        collision.GetComponent<Block>().PV.RPC("Teleport", RpcTarget.All, targetPosition);
        TransitionManager.Instance().onTransitionCutPointReached -= TransitionTeleportFuc;
    }
}
