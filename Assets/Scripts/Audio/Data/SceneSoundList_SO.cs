using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneSoundList_SO", menuName = "Sound/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> SceneSoundList;

    public SceneSoundItem GetSceneSoundItem(string name)
    {
        return SceneSoundList.Find(x => x.sceneName == name);
    }
}

[System.Serializable]
public class SceneSoundItem
{
     public string sceneName;
    public SoundName ambient;
    public SoundName music;
}
