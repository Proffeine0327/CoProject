using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InvokeUtility
{
    public static void Invoke(this MonoBehaviour mono, System.Action action, float time) => mono.StartCoroutine(InvokeCoroutine(action, time));

    public static IEnumerator InvokeCoroutine(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
