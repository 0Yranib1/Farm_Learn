using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector starDirector;
    private PlayableDirector currentDirector;

    private bool isDone;
    public bool IsDone
    {
        set => isDone = value;
    }

    private void OnEnable()
    {
        // currentDirector.played += TimelinePlayed;
        // currentDirector.stopped += TimelineStoped;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null)
        {
            currentDirector.Play();
        }
    }


    // private void TimelinePlayed(PlayableDirector director)
    // {
    //     if (director != null)
    //     {
    //         EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    //     }
    // }
    //
    // private void TimelineStoped(PlayableDirector director)
    // {
    //     if (director != null)
    //     {
    //         EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    //         director.gameObject.SetActive(false);
    //     }
    // }
    private bool isPause;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = starDirector;
    }

    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }
}
