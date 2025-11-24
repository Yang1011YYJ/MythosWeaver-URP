using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

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

    public void FadeIn(float duration, CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration,canvasGroup));
    }
    public void FadeOut(float duration, CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration, canvasGroup));
    }

    IEnumerator FadeRoutine(float start, float end, float duration, CanvasGroup canvasGroup)
    {
        float t = 0;
        canvasGroup.alpha = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t / duration);

            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
