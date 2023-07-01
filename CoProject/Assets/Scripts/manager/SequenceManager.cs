using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SequenceManager : MonoBehaviour
{
    public SequenceManager instance { get; private set; }

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        var pd = GetComponent<PlayableDirector>();
        pd.time = 5;
    }
}
