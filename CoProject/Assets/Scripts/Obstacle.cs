using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Obstacle : MonoBehaviour, IInteractable
{
    [SerializeField] private Situation interactSituation;
    [SerializeField] private string explain;

    public void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), explain);
    }

    public void Interact()
    {
        SequenceManager.StartSequence(interactSituation);
    }
}
