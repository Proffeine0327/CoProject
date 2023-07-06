using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : Interactable
{
    [SerializeField] private UnityEvent interactEvent;

    public override void Interact()
    {
        base.Interact();
        interactEvent.Invoke();
        gameObject.SetActive(false);
    }
}
