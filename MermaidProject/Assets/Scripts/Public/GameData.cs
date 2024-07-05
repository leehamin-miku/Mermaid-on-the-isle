using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[Serializable] // 직렬화

public class Data
{
    public struct SaveStruct
    {
        public Sprite img;
        public string text;
        public DateTime date;
        public int num;//진행상황을 int로 조절 img = null이면 noData
    }

    public SaveStruct[] saveArr = new SaveStruct[60];
}