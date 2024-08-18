using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable] // 직렬화

public class Data
{
    public struct SaveStruct
    {
        public int money;
        public string nextDialogue;
        public List<string> shopItemList;
        public List<string> textbookList;
        public List<SaveBlockStruct> saveBlockList;
        public string roomName;
        public int progressStatus;
        public bool shopOpened;
    }

    public struct SaveBlockStruct
    {
        public SaveBlockStruct(int blockCode, float[] b, Quaternion q, int strength, int w1, int w2, int w3, float a)
        {
            this.blockCode = blockCode;
            this.strength = strength;
            this.q = q;
            this.w1 = w1;
            this.w2 = w2;
            this.w3 = w3;
            this.a = a;
            this.b = b;
        }
        public int blockCode;
        public int strength;
        public int w1;
        public int w2;
        public int w3;
        public float a;
        public Quaternion q;
        public float[] b;
    }

    public SaveStruct[] saveFile = new SaveStruct[3]; //세브데이터 아마 3개 들어갈듯?
    public float MV = 0f;
    public float SV = 0f;
    public float BV = 0f;
    public float MS = 4f;
}