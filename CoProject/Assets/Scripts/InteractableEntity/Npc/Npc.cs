using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private Situation situation;
    [SerializeField] private string explain;

    public void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), explain);
    }

    public void Interact()
    {
        SequenceManager.StartSequence(situation);
    }
}
