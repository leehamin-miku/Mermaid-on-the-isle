using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    public GameObject checker;
    // Start is called before the first frame update
    void OnMouseDown()
    {
        if (GameObject.Find("SaveSpace").GetComponent<SaveObject>().index == -1)
        {
            GameObject.Find("SaveSpace").GetComponent<SaveObject>().index = int.Parse(name);
            Instantiate(checker);
        }
    }
}
