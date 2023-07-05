using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

public class Light2DControlBehaviour : PlayableBehaviour
{
    public Color color;
    public float intensity;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var light = playerData as Light2D;

        if(light != null)
        {
            light.color = color;
            light.intensity = intensity;
        }
    }
}
