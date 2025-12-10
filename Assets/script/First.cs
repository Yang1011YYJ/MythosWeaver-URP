using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class First : MonoBehaviour
{
    [Header("事件相關")]
    [Tooltip("顯示事件的文字")] public TextMeshProUGUI EventText;

    [Header("UI")]
    public GameObject BlackPanel;

    [Header("腳本")]
    public Animation01 animation01Script;
    // Start is called before the first frame update
    void Start()
    {
        animation01Script = FindAnyObjectByType<Animation01>();


        EventText.gameObject.SetActive(false);
        BlackPanel.SetActive(false);

        StartCoroutine(SceneFlow());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator SceneFlow()
    {
        //0.黑幕淡入
        BlackPanel.SetActive(true);
        animation01Script.Fade(BlackPanel, 1f, 0f, 1f/*持續時間*/, null);
        yield return new WaitForSeconds(1.5f);
        BlackPanel.SetActive(false);

        //1.地點顯示
        EventText.gameObject.SetActive(true);
        animation01Script.Fade(EventText.gameObject, 0f, 1f, 2f/*持續時間*/, null);
        yield return new WaitUntil(() => EventText.gameObject.GetComponent<CanvasGroup>().alpha == 1);
        yield return new WaitForSeconds(1f);
        animation01Script.Fade(EventText.gameObject, 1f, 0f, 2f/*持續時間*/, null);
        yield return new WaitUntil(() => EventText.gameObject.GetComponent<CanvasGroup>().alpha == 0);
        yield return new WaitForSeconds(1f);
        EventText.gameObject.SetActive(false);
    }
}
