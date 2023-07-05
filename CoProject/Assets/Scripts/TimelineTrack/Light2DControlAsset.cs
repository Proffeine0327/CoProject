using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Light2DControlAsset : PlayableAsset
{
    public Color color = Color.white;
    public float intensity = 1f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<Light2DControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.color = color;
        behaviour.intensity = intensity;
        
        return playable;
    }
}
