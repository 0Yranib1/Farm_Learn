using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    public interface ISaveable
    {
        
        string GUID{ get;}
        
        GameSaveData generateSaveData();

        public void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);   
        }
        
        void RestoreData(GameSaveData saveData);
    }
}

