using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class Temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TransitionManager.Instance().Transition("TitleScene", GameObject.Find("TransitionManager").GetComponent<TransitionSetArchive>().fade, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
