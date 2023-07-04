using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;
    [SerializeField] private Situation situation;

    private BoxCollider2D boxCollider2D;

    private void Start() 
    {
        boxCollider2D = GetComponent<BoxCollider2D>();   
    }

    public void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), !isOpen ? "Open" : "Close");
    }

    public void Interact()
    {
        if(situation != null)
            SequenceManager.StartSequence(situation);
        isOpen = !isOpen;
        boxCollider2D.isTrigger = isOpen;
    }
}
