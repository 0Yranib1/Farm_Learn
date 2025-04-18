using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour
{
    [Header("音乐数据")]
    public SoundDetailsList_SO soundDetailsList;
    public SceneSoundList_SO sceneSoundList;
    [Header("Audio Source")]
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;

    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound= sceneSoundList.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
        {
            return;
        }
        SoundDetails ambient= soundDetailsList.GetSoundDetails(sceneSound.ambient);
        SoundDetails music= soundDetailsList.GetSoundDetails(sceneSound.music);
        
        if(soundRoutine!=null)
            StopCoroutine(soundRoutine);
        else
        {
            soundRoutine= StartCoroutine(PlaySoundRoutine(music, ambient));
        }
    }

    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
    {
        if (music != null && ambient != null)
        {
            PlayAmbientClip(ambient);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music);
        }
    }
    
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails)
    {
        gameSource.clip= soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled)
        {
            gameSource.Play();
        }
    }
    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails)
    {
        ambientSource.clip= soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();
        }
    }
}
