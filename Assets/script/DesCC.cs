using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DesCC : MonoBehaviour
{
    [Header("腳本")]
    public DialogueSystemDes DialogueSystemDesScript;
    public AnimationDes animationDesScript;

    [Header("UI")]
    public GameObject Panel;
    [Tooltip("提示文字")] public TextMeshProUGUI HintText;
    public Button pulleventbutton;
    [Tooltip("捕捉傳送回來的資訊(先這樣用)")] public TMP_InputField inputField;

    [Header("放置角色")]
    [Tooltip("放置按鈕的位置01")] public GameObject Place01;
    [Tooltip("放置按鈕的位置02")] public GameObject Place02;
    [Tooltip("放置按鈕的位置03")] public GameObject Place03;
    [Tooltip("放置按鈕的位置04")] public GameObject Place04;
    [Tooltip("放置按鈕的位置05")] public GameObject Place05;
    [Tooltip("放置按鈕的位置06")] public GameObject Place06;
    [Tooltip("可以放角色了")] public bool CanPutRole;

    [Header("對話")]
    public bool desStart;//起始的說明文字(沒有說話人)

    [Header("Arduino相關數據")]
    [Tooltip("可以拉拉桿")]public bool CanPullEvent = false;
    [Tooltip("從拉桿接收到的數值")]public float PullEvent=0;

    [Header("回合設定")]
    [Tooltip("目前第幾輪（從 0 開始）")]
    public int roundIndex = 0;
    [Tooltip("總共要跑幾輪事件（可在 Inspector 改）")]public int maxRounds = 4;

    // 讓它跨場景保存（用 PlayerPrefs 最簡單）
    private const string KEY_ROUND = "ROUND_INDEX";
    private const string KEY_MAX = "MAX_ROUNDS";
    public bool IsFirstVisit() => roundIndex == 0;
    public bool HasMoreRounds() => roundIndex < maxRounds;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"[DesCC] roundIndex={roundIndex}, maxRounds={maxRounds}");


        animationDesScript = GetComponent<AnimationDes>();
        DialogueSystemDesScript = GetComponent<DialogueSystemDes>();
        inputField.gameObject.SetActive(false);
        pulleventbutton.gameObject.SetActive(false);
        HintText.gameObject.SetActive(false);
        roundIndex = PlayerPrefs.GetInt(KEY_ROUND, 0);
        maxRounds = PlayerPrefs.GetInt(KEY_MAX, maxRounds);
        // 把「淡入後要做什麼」包成一個 function 丟進去
        animationDesScript.Fade(
            Panel,
            1f,
            0f,
            2f/*持續時間*/,
            OnFadeFinished
        );
        
    }
    void OnFadeFinished()
    {
        Panel.SetActive(false);

        DialogueSystemDesScript.playOnEnable = true;
        DialogueSystemDesScript.keepHistoryInPrologue = true;

        if (IsFirstVisit())
        {
            // ✅ 第一次：播完整引導
            DialogueSystemDesScript.TextfileCurrent = DialogueSystemDesScript.TextfileDes;
            DialogueSystemDesScript.autoNextLine = true;
            DialogueSystemDesScript.KeepTalk = true;   // 讓它立即套用 TextfileCurrent 開始播
        }
        else
        {
            // ✅ 不是第一次：跳過長劇情，直接進「等待拉桿提示」
            DialogueSystemDesScript.StartShortLeverHintOnly();
        }

        desStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PullEventSet()
    {
        PullEvent = 5;
    }
    public void AdvanceRound()
    {
        roundIndex++;
        PlayerPrefs.SetInt(KEY_ROUND, roundIndex);
        PlayerPrefs.Save();
    }

    public bool IsFinalRound()
    {
        return roundIndex >= maxRounds;
    }

    public void ResetRounds()
    {
        roundIndex = 0;
        PlayerPrefs.SetInt(KEY_ROUND, roundIndex);
        PlayerPrefs.Save();
    }
}
