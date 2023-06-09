using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected Situation situation;
    [SerializeField] protected string explain;

    public bool canTalk => situation.canContinue;

    protected virtual void Start()
    {
        situation.Initialize();
    }

    public virtual void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), explain);
    }

    public virtual void Interact()
    {
        if(situation != null)
            SequenceManager.StartSequence(situation);
    }
}
