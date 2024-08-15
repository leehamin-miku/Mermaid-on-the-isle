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
        public SaveBlockStruct(int blockCode, Block block)
        {
            this.blockCode = blockCode;
            this.block = block;
        }
        public int blockCode;
        public Block block;
    }

    public SaveStruct[] saveFile = new SaveStruct[3]; //세브데이터 아마 3개 들어갈듯?
    public float MV = 100f;
    public float SV = 100f;
    public float BV = 100f;
    public float MS = 4f;

}