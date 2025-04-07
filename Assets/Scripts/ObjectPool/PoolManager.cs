using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();


    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
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
}
