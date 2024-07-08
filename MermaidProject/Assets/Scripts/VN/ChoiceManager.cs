using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{

    public GameObject OptionButtonGroup;
    private GameObject[] Button;

    public IEnumerator MakeChoice(int n)
    {
      
        float SpawnLocate = (n / 2) - 0.5f;
        Transform parentTransform = OptionButtonGroup.transform;

        Button = new GameObject[n];

        for (int i = 0; i < n; i++)
        {

            Button[i] = parentTransform.GetChild(i).gameObject;
            Button[i].SetActive(true);

        }

        for (int i = 0; i < n; i++)
        {
            
        }

        // ¼±ÅÃ


        if (Input.GetKeyDown("space"))
        {
            yield return null; 
        }
        yield return null;
    }
}
