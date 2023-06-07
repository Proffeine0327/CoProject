using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] private Image bg;
    [Header("btns")]
    [SerializeField] private Button start;
    [SerializeField] private Button howto;
    [SerializeField] private Button setting;
    [SerializeField] private Button credit;
    [SerializeField] private Button exit;

    private bool isPressedStart;

    private void Awake()
    {
        start.onClick.AddListener(() =>
        {
            if(!isPressedStart)
            {
                isPressedStart = true;
                StartCoroutine(StartAnimation());
            }
        });

        howto.onClick.AddListener(() =>
        {

        });

        setting.onClick.AddListener(() =>
        {

        });

        credit.onClick.AddListener(() =>
        {

        });

        exit.onClick.AddListener(() =>
        {

        });
    }

    private IEnumerator StartAnimation()
    {
        bg.gameObject.SetActive(true);
        for (float t = 0; t < 3; t += Time.deltaTime)
        {
            bg.color = new Color(0, 0, 0, t / 3);
            yield return null;
        }
        SceneManager.LoadScene("Intro");
    }
}
