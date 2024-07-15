using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class MiningDrill : Block
{
    public int waitingLen = 0; //마스터만 관리
    float a; // 이전 프레임의 각도
    float b = 0;
    public override void Awake()
    {
        base.Awake();
        a = transform.rotation.eulerAngles.z;
    }
    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Debug.Log(b);
            b += Mathf.Abs(a - transform.rotation.eulerAngles.z);
            a = transform.rotation.eulerAngles.z;
            if (b > 360f)
            {
                Vector2 a = Random.insideUnitCircle.normalized*2;
                Vector3 c = new Vector3(a.x, a.y, 0);
                b -= 360f;
                Debug.Log("돌 소환");
                PhotonNetwork.Instantiate("Prefab/Game/Rock", transform.position + c, transform.rotation);
            }
        }
        
    }
}
