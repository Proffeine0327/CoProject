using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroFadeInUI : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI skip;

    private bool isEndAnimation;

    private void Start()
    {
        img.gameObject.SetActive(true);
        skip.color = Color.black;
        StartCoroutine(FadeOutEffect());
    }

    private void Update() 
    {
        if(isEndAnimation)
        {
            if(Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(skip.rectTransform, Input.mousePosition))
            {
                SceneManager.LoadScene("InGame");
            }
        }
    }

    private IEnumerator FadeOutEffect()
    {
        for (float t = 0; t < 3; t += Time.deltaTime)
        {
            img.color = new Color(0, 0, 0, 1 - t / 3);
            yield return null;
        }

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            skip.color = new Color(1, 1, 1, t / 1);
            yield return null;
        }
        isEndAnimation = true;
    }
}
