using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private Queue<GameObject> soundQueue= new Queue<GameObject>(); 
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSoundEffect += InitSoundEffect;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }

    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                ()=> Instantiate(item, parent),
                e=>{e.gameObject.SetActive(true);},
                e=>{e.gameObject.SetActive(false);},
                e=>{Destroy(e.gameObject);},true,10,20
                ); 
            poolEffectList.Add(newPool);
        }

    }
    
    public void OnParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        //根据特效补全
        ObjectPool<GameObject> objPool=effectType switch
        {
            ParticaleEffectType.LeavesFalling01 => poolEffectList[0],
            ParticaleEffectType.LeavesFalling02 => poolEffectList[1],
            ParticaleEffectType.Rock => poolEffectList[2],
            ParticaleEffectType.ReapableScenery => poolEffectList[3],
            _=> null,
        };
        
        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool,obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool , GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }

    // private void InitSoundEffect(SoundDetails soundDetails)
    // {
    //     ObjectPool<GameObject> pool= poolEffectList[4];
    //     var obj = pool.Get();
    //     obj.GetComponent<Sound>().SetSound(soundDetails);
    //     StartCoroutine(DisableSound(pool,obj,soundDetails));
    // }
    //
    // private IEnumerator DisableSound(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDetails)
    // {
    //     yield return new WaitForSeconds(soundDetails.soundClip.length);
    //     pool.Release(obj);
    // }

    private void CreateSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }
    
    private GameObject GetPoolObject()
    {
        if(soundQueue.Count<2)
            CreateSoundPool();
        return soundQueue.Dequeue();
    }

    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }

    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
}
