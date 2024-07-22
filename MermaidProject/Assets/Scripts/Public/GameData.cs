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
        public List<SaveBlockStruct> saveBlockList;
        public string roomName;
        public int progressStatus;
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
}