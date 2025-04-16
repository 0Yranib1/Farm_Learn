using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluePrintDataList_SO", menuName = "Inventory/BluePrintData")]
public class BluePrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;
    
    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrintDataList.Find(x => x.ID == itemID);
    }
}
[Serializable]
public class BluePrintDetails
{
    public int ID;
    public InventoryItem[] resourceItem = new InventoryItem[4];
    public GameObject buildPrefab;
}