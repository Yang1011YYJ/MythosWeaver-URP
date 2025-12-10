using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class Animation03 : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static WaitForSeconds Seconds(float seconds)
    {
        return new WaitForSeconds(seconds);
    }
    public void Fade(GameObject gameObject, float start,float end,float duration, Action onComplete)
    {
        StartCoroutine(FadeRoutine(start, end, duration, gameObject,  onComplete));
    }

    public IEnumerator FadeRoutine(float start, float end, float duration, GameObject gameObject, Action onComplete)
    {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();

        float t = 0;
        canvasGroup.alpha = start;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }
        canvasGroup.alpha = end;

        // 🔔 淡入做完，通知外面「可以下一步囉」
        onComplete?.Invoke();
    }
}
