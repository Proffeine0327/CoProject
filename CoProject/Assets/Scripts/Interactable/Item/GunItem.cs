using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunItem : Interactable
{
    [SerializeField] private WeaponSelection getWeaponType;

    public override void Interact()
    {
        base.Interact();
        Player.Instance.GetWeapon(getWeaponType);
        gameObject.SetActive(false);
    }
}
