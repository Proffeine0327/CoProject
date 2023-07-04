using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractUI : MonoBehaviour
{
    private static InteractUI instance;

    public static void DisplayUI(bool active, Vector2 worldPos, string explain)
    {
        instance.group.gameObject.SetActive(active);
        instance.group.position = Camera.main.WorldToScreenPoint(worldPos);
        instance.explain.text = explain;
    }

    [SerializeField] private TextMeshProUGUI explain;
    [SerializeField] private RectTransform group;           

    private void Awake() 
    {
        instance = this;
    }
}
