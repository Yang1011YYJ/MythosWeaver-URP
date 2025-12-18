using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMenu : MonoBehaviour
{
    public GameObject Circle;
    public GameObject Circle1;
    public Animator Circleanimator;
    public Animator Circle1animator;
    private void Start()
    {
        Circleanimator = Circle.GetComponent<Animator>();
        Circle1animator = Circle1.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void CircleOnC()
    {
        Circleanimator.SetBool("open", true);
        Circle1animator.SetBool("open",true);
    }

    public void Fade(GameObject panel, float duration, float from, float to, Action onComplete)
    {
        StartCoroutine(FadeRoutine(panel,duration,from,to,onComplete));
    }

    public IEnumerator FadeRoutine(GameObject panel,float duration,float from,float to,Action onComplete)
    {
        if (panel == null) yield break;

        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = from;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        cg.alpha = to;

        onComplete?.Invoke();
    }
}
