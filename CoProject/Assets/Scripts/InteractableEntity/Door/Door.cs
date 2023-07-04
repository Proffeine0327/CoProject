using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected Situation situation;

    protected BoxCollider2D boxCollider2D;
    protected bool isOpen;

    protected virtual void Start() 
    {
        boxCollider2D = GetComponent<BoxCollider2D>(); 
    }

    public virtual void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), !isOpen ? "Open" : "Close");
    }

    public virtual void Interact()
    {
        if(situation != null)
            SequenceManager.StartSequence(situation);
        isOpen = !isOpen;
        boxCollider2D.isTrigger = isOpen;
    }
}
