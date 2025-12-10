using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MythEventType
{
    None = 0,
    Creation = 1,
    HeroQuest = 2,
    Trickery = 3,
    Cataclysm = 4
}

public enum LeverState
{
    Locked,          // 不能拉
    WaitingForPull,  // 可以拉（只吃第一次）
    ChoiceResolved   // 這一輪拉桿已經處理完
}

public class ChooseEventss : MonoBehaviour
{
    [Header("事件相關設定")]
    public MythEventType currentEvent;
    public TextMeshProUGUI eventNameText;
    [Tooltip("開始說明事件")] public bool descriptionStarted = false;

    [Header("UI 元件")]
    public CanvasGroup EventShow;      // 顯示結果的面板（要掛 CanvasGroup）
    public TextMeshProUGUI resultText; // 面板上顯示結果的文字

    [Header("Flow 狀態")]
    public LeverState leverState = LeverState.Locked;
    [Tooltip("防止拉一次桿在同一段時間內重複觸發")]
    public bool hasShownCardThisPull = false;

    [Header("卡片")]
    public GameObject Card;
    public Animator CardAnimator;
    public Image CardCurrent;
    [Tooltip("災變")] public Sprite cataclysm;
    [Tooltip("創世")] public Sprite creation;
    [Tooltip("英雄之旅")] public Sprite herosQuest;
    [Tooltip("詭計 / 狡詐")] public Sprite trickery;

    [Header("腳本")]
    public AnimationDes animationDes;
    public DesCC desCCScript;
    public DialogueSystemDes dialogueSystemDesScript;

    [Header("淡入與切換設定")]
    public float fadeDuration = 0.8f;     // 淡入時間
    public float waitBeforeLoad = 1.0f;   // 面板完全出現後停留多久再切場景

    private void Awake()
    {
        if (animationDes == null)
            animationDes = GetComponent<AnimationDes>();

        if (desCCScript == null)
            desCCScript = FindObjectOfType<DesCC>();

        if (dialogueSystemDesScript == null)
            dialogueSystemDesScript = FindObjectOfType<DialogueSystemDes>();
    }

    private void Start()
    {
        if (Card != null)
        {
            CardAnimator = Card.GetComponent<Animator>();
            CardCurrent = Card.GetComponent<Image>();
        }

        // 初始化面板為隱藏
        if (EventShow != null)
        {
            EventShow.alpha = 0;
            EventShow.gameObject.SetActive(false);
        }
    }

    // ====== 給對話系統呼叫：這一輪可以拉桿了 ======
    public void EnableLever()
    {
        leverState = LeverState.WaitingForPull;
        hasShownCardThisPull = false;
        descriptionStarted = false;

        // Optional：跟 DesCC 同步一下
        if (desCCScript != null)
        {
            desCCScript.CanPullEvent = true;
            desCCScript.PullEvent = 0;
        }
    }

    // =============================
    // 🌟 統一入口：不管來源是 UI 或 Arduino
    // =============================
    public void HandleLeverChoice(int choiceId)
    {
        // 只有在「等待拉桿」狀態，第一次拉才會被處理
        if (leverState != LeverState.WaitingForPull)
        {
            Debug.Log("現在不接受拉桿，狀態：" + leverState);
            return;
        }

        if (choiceId < 1 || choiceId > 4)
        {
            Debug.LogWarning("拉桿數值不在 1~4 範圍內：" + choiceId);
            return;
        }

        // 避免後續再吃到多次拉桿
        leverState = LeverState.ChoiceResolved;
        hasShownCardThisPull = true;

        // 跟 DesCC 同步一下狀態（可有可無，看你後面要不要用）
        if (desCCScript != null)
        {
            desCCScript.CanPullEvent = false;
            desCCScript.PullEvent = 5;
        }

        dialogueSystemDesScript.TextPanel.SetActive(false);
        // 套事件 & 播動畫
        SetEvent(choiceId);
        ShowCardAnimation();
    }

    // =============================
    // ✅ 目前軟體測試用：UI 按鈕版
    // =============================
    // 版本 1：維持你原本「輸入數字＋按按鈕」
    public void OnLeverPulledByButton()
    {
        if (desCCScript == null || desCCScript.inputField == null)
        {
            Debug.LogWarning("缺少 DesCC 或 inputField 參考");
            return;
        }

        int id;
        string text = desCCScript.inputField.text;

        if (!int.TryParse(text, out id))
        {
            Debug.LogWarning("輸入不是有效數字：" + text);
            return;
        }

        HandleLeverChoice(id);
    }

    // =============================
    // 🔌 未來硬體用：Arduino 版
    // =============================
    public void OnLeverPulledFromArduino(int valueFromHardware)
    {
        HandleLeverChoice(valueFromHardware);
    }

    // =============================
    // 下方是事件邏輯 & 播動畫
    // =============================
    public void SetEvent(int eventID)
    {
        currentEvent = (MythEventType)eventID;
        CardNimber();
        Debug.Log("當前事件：" + currentEvent);
    }

    private void CardNimber()
    {
        if (CardCurrent == null || eventNameText == null)
            return;

        switch (currentEvent)
        {
            case MythEventType.Creation:
                eventNameText.text = "創世";
                CardCurrent.sprite = creation;
                break;
            case MythEventType.HeroQuest:
                eventNameText.text = "英雄之旅";
                CardCurrent.sprite = herosQuest;
                break;
            case MythEventType.Trickery:
                eventNameText.text = "詭計 / 狡詐";
                CardCurrent.sprite = trickery;
                break;
            case MythEventType.Cataclysm:
                eventNameText.text = "災變";
                CardCurrent.sprite = cataclysm;
                break;
            default:
                eventNameText.text = "";
                CardCurrent.sprite = null;
                break;
        }
    }

    private void ShowCardAnimation()
    {
        if (desCCScript != null)//關掉按鈕和輸入事件編號的UI
                                //之後接arduino可以拿掉
        {
            if (desCCScript.pulleventbutton != null)
                desCCScript.pulleventbutton.gameObject.SetActive(false);

            if (desCCScript.inputField != null)
                desCCScript.inputField.gameObject.SetActive(false);
        }
        if (Card == null) return;

        if (!Card.activeSelf)
            Card.SetActive(true);

        if (CardAnimator != null)
        {
            CardAnimator.SetBool("CardShow", true);
        }
    }

    // 卡片收回 → 開始描述卡片用的對話
    public void StartDescriptionDialogueIfNeeded()
    {
        if (!hasShownCardThisPull || descriptionStarted)
            return;

        descriptionStarted = true;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Card.SetActive(false);
            if (dialogueSystemDesScript != null)
            {
                dialogueSystemDesScript.TextPanel.SetActive(true);
                dialogueSystemDesScript.TextfileCurrent = dialogueSystemDesScript.TextfileDescriptionCard;
                dialogueSystemDesScript.KeepTalk = true;
            }
        }

        
    }

    // ======= 下面這段你原本用事件資料庫＋切場景的邏輯先註解著，就不貼了 =======
}
