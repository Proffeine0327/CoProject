using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Dialogue;
using TMPro;

public class SequenceManager : MonoBehaviour
{
    private static SequenceManager instance;

    public static bool isPlayingSequence => instance.current != null;
    public static bool isPlayingTimeline => isPlayingSequence && instance.current.playable != null;
    public static void StartSequence(Situation situation)
    {
        situation.Initialize();
        instance.current = situation;
        instance.pd.playableAsset = situation.playable;

        instance.StartCoroutine(instance.ContinueSequence());
    }
    

    [Header("ref")]
    [SerializeField] private GameObject bg;
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    [Header("var")]
    [SerializeField] private int textAnimationSpeed;
    [Header("start")]
    [SerializeField] private Situation situation;

    private PlayableDirector pd;
    private Situation current;
    private Coroutine pdmanager;

    private void Awake()
    {
        instance = this;

        pd = GetComponent<PlayableDirector>();

        StartSequence(situation);
    }

    public IEnumerator ContinueSequence()
    {
        while (true)
        {
            if (current.currentSentence.Type == SentenceType.normal)
            {
                speaker.text = current.currentSentence.Narrator;

                if(current.currentSentence.Sprite != null)
                {
                    image.sprite = current.currentSentence.Sprite;
                    image.color = Color.white;
                }
                else
                {
                    image.color = new Color(1,1,1,0);
                }
                
                for (int i = 0; i < current.currentSentence.Text.Length; i++)
                {
                    text.text = current.currentSentence.Text.Substring(0, i);

                    bool skip = false;
                    for (float t = 0; t < textAnimationSpeed / 1000f; t += Time.deltaTime)
                    {
                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            skip = true;
                            break;
                        }
                        yield return null;
                    }
                    if (skip) break;
                }
                text.text = current.currentSentence.Text;

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            }

            if (current.currentSentence.PassTime > 0)
            {
                bg.SetActive(false);
                yield return new WaitForSeconds(current.currentSentence.PassTime);
                bg.SetActive(true);
            }
            foreach(var method in current.currentSentence.Methods)
                method.Invoke();

            if (current.canContinue)
            {
                instance.bg.SetActive(true);
                current.Continue();

                if (current.currentSentence.IsPlayTimeline)
                {
                    if (pdmanager != null) StopCoroutine(pdmanager);
                    pdmanager = StartCoroutine(ManagePlayableDirector(current.currentSentence.BetweenTime, current.currentSentence.WrapMode));
                }
            }
            else break;
        }
        
        yield return null;
        EndSequence();
    }

    public void EndSequence()
    {
        current.Initialize();
        current = null;
        pd.Pause();
        bg.SetActive(false);
    }

    private IEnumerator ManagePlayableDirector(Vector2 time, DirectorWrapMode mode)
    {
        do
        {
            pd.Play();
            pd.time = time.x;
            yield return new WaitUntil(() => pd.time >= Mathf.Min(time.y, ((float)pd.duration) - 0.001f));
        }
        while (mode == DirectorWrapMode.Loop);

        pd.Pause();
        pd.time =  Mathf.Min(time.y, ((float)pd.duration) - 0.001f);

        if (mode == DirectorWrapMode.None)
        {
            pd.Play();
            pd.time = time.x;
            yield return null;
            pd.Pause();
        }
    }
}
