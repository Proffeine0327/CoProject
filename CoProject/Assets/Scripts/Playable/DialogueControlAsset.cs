using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueControlAsset : PlayableAsset
{
    public string speaker;
    public string text;
    public Sprite sprite;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueControlBehaviour>.Create(graph);
        var dialogueControlBehaviour = playable.GetBehaviour();

        dialogueControlBehaviour.speaker = speaker;
        dialogueControlBehaviour.text = text;
        dialogueControlBehaviour.sprite = sprite;
        
        return playable;
    }
}
