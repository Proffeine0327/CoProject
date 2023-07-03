using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnounceUI : MonoBehaviour
{
    private static AnnounceUI instance;

    public static void DisplayUI(string title, string explain, float fadeIn, float wait, float fadeOut)
    {
        instance.title.text = title;
        instance.explain.text = explain;

        instance.StartCoroutine(instance.Animation(fadeIn, wait, fadeOut));
    }

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI explain;
    [SerializeField] private Image bg;

    private float startbgalpha;

    private void Awake()
    {
        instance = this;
        startbgalpha = bg.color.a;

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
    }

    private IEnumerator Animation(float _in, float wait, float _out)
    {
        bg.gameObject.SetActive(true);
        title.gameObject.SetActive(true);
        explain.gameObject.SetActive(true);
        
        for (float t = 0; t < _in; t += Time.deltaTime)
        {
            bg.color = new Color(0,0,0, (t / _in) * startbgalpha);
            title.color = new Color(1,1,1, t / _in);
            explain.color = new Color(1,1,1, t / _in);
            yield return null;
        }

        yield return new WaitForSeconds(wait);

        for (float t = _out; t > 0; t -= Time.deltaTime)
        {
            bg.color = new Color(0,0,0, (t / _out) * startbgalpha);
            title.color = new Color(1,1,1, t / _out);
            explain.color = new Color(1,1,1, t / _out);
            yield return null;
        }

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
    }
}
