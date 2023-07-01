using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class DialogueControlBehaviour : PlayableBehaviour
{
    public string speaker;
    public string text;
    public Sprite sprite;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var data = playerData as DialogueUI;
        var process = ((float)playable.GetTime()) / ((float)playable.GetDuration());
        
        data.Speaker.text = speaker;

        if(!string.IsNullOrEmpty(text))
            data.Text.text = text.Substring(0, Mathf.CeilToInt(process * text.Length));

        if(sprite == null) data.Image.color = new Color(1,1,1,0);
        else 
        {
            data.Image.color = new Color(1,1,1,1);
            data.Image.sprite = sprite;
        }
    }
}
