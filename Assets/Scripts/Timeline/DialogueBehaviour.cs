using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Dialogue;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            if (dialoguePiece.hasToPause)
            {
                //暂停timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }
}
