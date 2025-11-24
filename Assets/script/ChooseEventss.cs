using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseEventss : MonoBehaviour
{
    

    [Header("UI 元件")]
    public CanvasGroup EventShow;   // 顯示結果的面板（要掛 CanvasGroup）
    public TextMeshProUGUI resultText;           // 面板上顯示結果的文字

    public AnimationDes animationDes;



    [Header("淡入與切換設定")]
    public float fadeDuration = 0.8f;     // 淡入時間
    public float waitBeforeLoad = 1.0f;   // 面板完全出現後停留多久再切場景

    private void Awake()
    {
        animationDes = EventSystem.current.GetComponent<AnimationDes>();
    }

    void Start()
    {
        // 初始化面板為隱藏
        if (EventShow != null)
        {
            EventShow.alpha = 0;
            EventShow.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //隨機抽事件
    public void EventOne()
    {
        var events = EventDatabase.Instance.events;
        if (events == null || events.Count == 0)
        {
            Debug.Log("List是空的!");
            return;
        }

        int index = Random.Range(0, events.Count);//抽一個
        var picked = events[index];

        //移除剛剛抽到的
        events.RemoveAt(index);

        Debug.Log("抽到:"+picked.displayName);

        // 顯示結果面板並讀取對應場景
        StartCoroutine(ShowResultAndLoadScene(picked));
    }

    IEnumerator ShowResultAndLoadScene(EventDatabase.EventConfig picked)
    {
        // 顯示結果面板
        EventShow.gameObject.SetActive(true);
        EventShow.alpha = 0;

        if (resultText != null)
        {
            resultText.text = "抽到：" + picked.displayName;
        }

        // 淡入動畫
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            EventShow.alpha = a;
            yield return null;
        }
        EventShow.alpha = 1f;

        // 停留一段時間讓玩家看結果
        yield return new WaitForSeconds(waitBeforeLoad);

        // 切換到對應場景
        if (!string.IsNullOrEmpty(picked.sceneName))
        {
            SceneManager.LoadScene(picked.sceneName);
        }
        else
        {
            Debug.LogWarning("這個事件沒有設定 sceneName。");
        }
    }
}
