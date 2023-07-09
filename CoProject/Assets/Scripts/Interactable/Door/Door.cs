using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Door : Interactable
{
    protected BoxCollider2D boxCollider2D;
    protected SpriteRenderer spriteRenderer;
    protected bool isOpen;

    protected override void Start() 
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void DisplayUI()
    {
        InteractUI.DisplayUI(true, transform.position + new Vector3(-0.75f, 0.5f), !isOpen ? "Open" : "Close");
    }

    public override void Interact()
    {
        base.Interact();
        isOpen = !isOpen;
        boxCollider2D.isTrigger = isOpen;
        spriteRenderer.enabled = !isOpen;
    }
}
