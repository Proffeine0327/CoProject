using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Obstacle : MonoBehaviour, IInteractable
{
    [SerializeField] protected Situation situation;
    [SerializeField] protected string explain;

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
