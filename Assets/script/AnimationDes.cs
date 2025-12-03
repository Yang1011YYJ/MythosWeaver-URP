using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using System;

public class AnimationDes : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private SpriteRenderer spriteRenderer;
    private TextMeshProUGUI tmpText;
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
    public void FadeIn(float duration, GameObject gameObject, bool a, Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration, gameObject, a, onComplete));
    }
    public void FadeOut(float duration, GameObject gameObject, bool a, Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration, gameObject, a, onComplete));
    }

    IEnumerator FadeRoutine(float start, float end, float duration, GameObject gameObject, bool a, Action onComplete)
    {
        CanvasGroup canvasGroup;
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        a = false;
        float t = 0;
        canvasGroup.alpha = start;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t / duration);

            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = end;
        gameObject.SetActive(false);
        a = true;
        // 🔔 淡入做完，通知外面「可以下一步囉」
        onComplete?.Invoke();
    }
}
