using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnounceUI : MonoBehaviour
{
    private static AnnounceUI instance;

    public static void DisplayUI(string title, string explain, float fadeIn, float fadeOut, Sprite sprite)
    {
        instance.title.text = title;
        instance.explain.text = explain;
        instance.img.sprite = sprite;

        instance.StartCoroutine(instance.Animation(fadeIn, fadeOut));
    }

    public static bool isShowingAnnounce => instance._isShowingAnnounce;

    [Header("Setting")]
    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI explain;
    [SerializeField] private Image img;
    [Header("Display")]

    private float startbgalpha;
    private bool _isShowingAnnounce;

    private void Awake()
    {
        instance = this;
        startbgalpha = bg.color.a;

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
    }

    private IEnumerator Animation(float _in, float _out)
    {
        _isShowingAnnounce = true;

        bg.gameObject.SetActive(true);
        title.gameObject.SetActive(true);
        explain.gameObject.SetActive(true);
        img.gameObject.SetActive(true);
        
        for (float t = 0; t < _in; t += Time.deltaTime)
        {
            bg.color = new Color(0,0,0, (t / _in) * startbgalpha);
            title.color = new Color(1,1,1, t / _in);
            explain.color = new Color(1,1,1, t / _in);
            img.color = new Color(1,1,1, t / _in);
            yield return null;
        }

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        _isShowingAnnounce = false;

        for (float t = _out; t > 0; t -= Time.deltaTime)
        {
            bg.color = new Color(0,0,0, (t / _out) * startbgalpha);
            title.color = new Color(1,1,1, t / _out);
            explain.color = new Color(1,1,1, t / _out);
            img.color = new Color(1,1,1, t / _out);
            yield return null;
        }

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
    }
}
