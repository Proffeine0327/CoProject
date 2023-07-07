using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponDisplayUI : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI ammo;

    private void Update()
    {
        var curweapon = Player.Instance.CurrentGun;

        if (curweapon == null)
        {
            image.color = new Color(1, 1, 1, 0);
            type.text = "None";
            ammo.text = "";
            return;
        }

        image.sprite = curweapon.UIDisplaySprite;
        image.color = new Color(1, 1, 1, 0.5f);
        type.text = curweapon.UIDisplayType;

        if (curweapon.MaxAmmo > 0) ammo.text = $"{curweapon.CurAmmo}/{curweapon.MaxAmmo}";
        else ammo.text = "";
    }
}
