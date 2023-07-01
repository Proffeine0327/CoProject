using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;

    public TextMeshProUGUI Speaker => speaker;
    public TextMeshProUGUI Text => text;
    public Image Image => image;
}
