using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattenList_SO", menuName = "Light/LightPatten")]
public class LightPattenList_SO : ScriptableObject
{
    public List<LightDetails> lightPattenList;

    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        return lightPattenList.Find(x => x.season == season && x.lightShift == lightShift);
    }
    
}

[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}
