using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnounceUI : MonoBehaviour
{
    private static AnnounceUI instance;

    public static void DisplayUI(int displayIndex, float fadeIn, float fadeOut)
    {
        instance.displayIndex = displayIndex;
        instance.StartCoroutine(instance.Animation(fadeIn, fadeOut));
    }

    public static bool isShowingAnnounce => instance._isShowingAnnounce;

    [Header("Setting")]
    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI explain;
    [SerializeField] private TextMeshProUGUI leftMark;
    [SerializeField] private TextMeshProUGUI rightMark;
    [SerializeField] private TextMeshProUGUI zMark;
    [Header("Display")]
    [SerializeField] private List<DisplayInfo> displayInfos = new List<DisplayInfo>();

    private float startbgalpha;
    private bool _isShowingAnnounce;
    private int displayIndex;

    private void Awake()
    {
        instance = this;
        startbgalpha = bg.color.a;

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
    }

    private IEnumerator Animation(float _in, float _out)
    {
        _isShowingAnnounce = true;

        title.text = displayInfos[displayIndex].Layers[0].Title;
        explain.text = displayInfos[displayIndex].Layers[0].Explain;

        bg.gameObject.SetActive(true);
        title.gameObject.SetActive(true);
        explain.gameObject.SetActive(true);

        if(displayInfos[displayIndex].Layers.Count != 1)
            rightMark.gameObject.SetActive(true);
        
        zMark.gameObject.SetActive(true);

        for (float t = 0; t < _in; t += Time.deltaTime)
        {
            bg.color = new Color(0, 0, 0, (t / _in) * startbgalpha);
            title.color = new Color(1, 1, 1, t / _in);
            explain.color = new Color(1, 1, 1, t / _in);
            leftMark.color = new Color(1, 1, 1, t / _in);
            rightMark.color = new Color(1, 1, 1, t / _in);
            zMark.color = new Color(1, 1, 1, t / _in);
            yield return null;
        }

        var curIndex = 0;
        while (true)
        {
            curIndex = Mathf.Clamp(curIndex, 0, displayInfos[displayIndex].Layers.Count - 1);
            var curlayer = displayInfos[displayIndex].Layers[curIndex];

            leftMark.gameObject.SetActive(true);
            rightMark.gameObject.SetActive(true);
            if(curIndex <= 0) leftMark.gameObject.SetActive(false);
            if(curIndex >= displayInfos[displayIndex].Layers.Count - 1) rightMark.gameObject.SetActive(false);
            

            title.text = curlayer.Title;
            explain.text = curlayer.Explain;

            if (Input.GetKeyDown(KeyCode.LeftArrow)) curIndex--;
            if (Input.GetKeyDown(KeyCode.RightArrow)) curIndex++;
            if (Input.GetKeyDown(KeyCode.Z)) break;

            yield return null;
        }

        _isShowingAnnounce = false;

        for (float t = _out; t > 0; t -= Time.deltaTime)
        {
            bg.color = new Color(0, 0, 0, (t / _out) * startbgalpha);
            title.color = new Color(1, 1, 1, t / _out);
            explain.color = new Color(1, 1, 1, t / _out);
            leftMark.color = new Color(1, 1, 1, t / _out);
            rightMark.color = new Color(1, 1, 1, t / _out);
            zMark.color = new Color(1, 1, 1, t / _out);
            yield return null;
        }

        bg.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        explain.gameObject.SetActive(false);
        leftMark.gameObject.SetActive(false);
        rightMark.gameObject.SetActive(false);
        zMark.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class DisplayInfo
{
    [SerializeField] private List<DisplayLayerInfo> layers = new List<DisplayLayerInfo>();

    public List<DisplayLayerInfo> Layers => layers;
}

[System.Serializable]
public class DisplayLayerInfo
{
    [SerializeField] private string title;
    [TextArea(3, 4)]
    [SerializeField] private string explain;

    public string Title => title;
    public string Explain => explain;
}
