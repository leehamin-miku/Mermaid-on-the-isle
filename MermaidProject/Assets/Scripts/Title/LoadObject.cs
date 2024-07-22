using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class LoadObject : MonoBehaviour
{
    [SerializeField] GameObject gameObject;
    [SerializeField] int a;//0, 1, 2
    [SerializeField] InputField roomName;
    bool isSelected= false;
    bool isNull = false;
    void SetIsSelected(bool value)
    {
        gameObject.GetComponent<LobbyManager>().a = a;
        for (int i = 1; i < 4; i++)
        {
            transform.parent.GetChild(i).GetComponent<LoadObject>().isSelected = false;
            transform.parent.GetChild(i).GetComponent<LoadObject>().Mark();
        }
        isSelected = value;
        Mark();
    }

    public void DataMark()
    {
        DataManager.Instance.LoadGameData();
        if (DataManager.Instance.data.saveFile[a].roomName == null)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "비어있음";
            isNull = true;
        } else
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DataManager.Instance.data.saveFile[a].roomName + "\nD-" + DataManager.Instance.data.saveFile[a].progressStatus;
            isNull = false;
        }
        
    }

    private void OnMouseDown()
    {
        if (isNull==false)
        {
            if (isSelected)
            {
                SetIsSelected(false);
            }
            else
            {
                SetIsSelected(true);
                roomName.text = DataManager.Instance.data.saveFile[a].roomName;
            }
        }
    }

    void Mark()
    {
        if (isSelected)
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        } else
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }


}
