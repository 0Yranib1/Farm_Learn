using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    /// <summary>
    /// 进度条 string 是guid
    /// </summary>
    public class DataSlot 
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();
    }
}

