using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValFish.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 载入json文件对应的类
**************************************/

namespace tpgm
{
    [Serializable]
    public class ValFish : BaseVal
    {
        public int type;
        public string icon = "";
        public string name = "";
        public string hp = "";
        public float swimm;
        public int star;
        public int body;
        public int golds;
        public int score;
        public int hardLev;
        // 难度
        public int userLevExp;
        // 玩家等级经验
        public int gunLevExp;
        // 火炮等级经验
    }
}
