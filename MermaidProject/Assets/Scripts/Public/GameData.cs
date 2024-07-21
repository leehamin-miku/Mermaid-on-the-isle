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

        //saveObject에 있는 모든 오브젝트를 저장 << 가능?
        //타입, 
        public List<Block> saveBlockList;
        
    }

    public SaveStruct[] saveFile;
}