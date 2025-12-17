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
    public SpriteRenderer CardCurrent;
    [Tooltip("災變")] public Sprite cataclysm;
    [Tooltip("創世")] public Sprite creation;
    [Tooltip("英雄之旅")] public Sprite herosQuest;
    [Tooltip("詭計 / 狡詐")] public Sprite trickery;

    [Header("題目與角色選擇 UI")]
    public GameObject topicPanel;                // 放題目文字的整個面板
    public TextMeshProUGUI topicText;           // 顯示「愛與毀滅」之類
    [Header("角色選擇 UI")]
    public GameObject roleSelectPanel;
    public TextMeshProUGUI roleHintText;         // 顯示提示/結果的文字
    public GameObject roleButtonsRoot;           // 六個按鈕的父物件（提示前也可以顯示）
    public CanvasGroup fadeCanvas;               // 淡出用（可選）


    [Header("腳本")]
    public AnimationDes animationDes;
    public DesCC desCCScript;
    public DialogueSystemDes dialogueSystemDesScript;
    public DissolveEffect dissolveEffectScript;

    [Header("淡入與切換設定")]
    public float fadeDuration = 0.8f;     // 淡入時間
    public float waitBeforeLoad = 1.0f;   // 面板完全出現後停留多久再切場景
    [Header("角色資料")]
    public string[] roleNames = new string[6] { "角色1", "角色2", "角色3", "角色4", "角色5", "角色6" };

    [Header("流程參數")]
    public float showChosenDelay = 1.5f;         // 顯示「你選了誰」停幾秒
    public float showEnterEraDelay = 1.5f;       // 顯示「即將進入XX」停幾秒

    // 狀態
    private bool isArmed = false;                // 提示完成後才會 true
    private List<int> chosenRoles = new List<int>(2);

    // ✅ 由 DialogueSystemDes 在「事件說明播完」後呼叫
    public void StartRoleSelectionForCurrentEvent()
    {
        StopAllCoroutines();

        chosenRoles.Clear();
        isArmed = false;

        // 題目面板打開（你要在事件文字後，再提示玩家要選角色）
        if (topicPanel != null) topicPanel.SetActive(true);
        if (topicText != null) topicText.text = GetTopicHintForEvent(currentEvent);

        // ❗角色面板先不要開
        if (roleSelectPanel != null) roleSelectPanel.SetActive(false);
        if (roleButtonsRoot != null) roleButtonsRoot.SetActive(false);

        StartCoroutine(RoleSelectionRoutine());
    }


    private IEnumerator RoleSelectionRoutine()
    {
        // 0) 先告訴玩家事件是什麼（你說的：顯示選擇的事件文字後，再說還要選角色）
        desCCScript.HintText.gameObject.SetActive(true);
        desCCScript.HintText.text = $"你選到的事件是：{GetEventDisplayName(currentEvent)}";

        yield return new WaitForSeconds(1.2f);

        // 1) 再提示玩家要選角色（提示前不接受輸入）
        desCCScript.HintText.text = "請依照心意選擇兩個角色，放到指示台上吧！";

        yield return new WaitForSeconds(1.0f);

        // 2) ✅ 提示完成後才開面板 + 才開始接收
        desCCScript.HintText.gameObject.SetActive(false);
        if (roleSelectPanel != null) roleSelectPanel.SetActive(true);
        if (roleButtonsRoot != null) roleButtonsRoot.SetActive(true);

        isArmed = true;

        if (roleHintText != null)
            roleHintText.text = "（請選擇兩個角色）";

        // 3) 等兩個角色都選到
        yield return new WaitUntil(() => chosenRoles.Count >= 2);

        // 你也可以在這裡加打字機效果（之後要我再接）
        yield return new WaitForSeconds(0.8f);

        // 4) 顯示玩家選擇結果
        string a = GetRoleName(chosenRoles[0]);
        string b = GetRoleName(chosenRoles[1]);

        if (roleHintText != null)
            roleHintText.text = $"您選擇的角色是：{a}、{b}";

        yield return new WaitForSeconds(showChosenDelay);

        // 5) 顯示即將進入XX時代（事件名）
        string era = GetEventDisplayName(currentEvent);
        if (roleHintText != null)
            roleHintText.text = $"您即將進入 {era} 時代...";

        yield return new WaitForSeconds(showEnterEraDelay);

        // 6) 淡出 + 切場景
        yield return StartCoroutine(FadeOutIfNeeded());
        LoadSceneForCurrentEvent();
    }

    // ✅ 目前用 UI 按鈕代替 RFID：按鈕 onClick 直接呼叫這個，帶入 roleId (1~6)
    public void OnRoleButtonClicked(int roleId)
    {
        // roleId 建議你用 1~6，跟之後 RFID 回傳 ID 直覺一致
        TryAcceptRole(roleId);
    }

    // ✅ 之後 Arduino / RFID 收到角色放置時，也只要呼叫這個（完全同入口）
    public void OnRoleDetectedFromArduino(int roleId)
    {
        TryAcceptRole(roleId);
    }

    private void TryAcceptRole(int roleId)
    {
        // 提示未完成：不接受，要求重新放
        if (!isArmed)
        {
            if (roleHintText != null)
                roleHintText.text = "提示尚未完成，請在提示後重新放置角色！";
            return;
        }

        // 範圍檢查
        if (roleId < 1 || roleId > 6)
        {
            Debug.LogWarning("roleId 不在 1~6：" + roleId);
            return;
        }

        // 不要重複選同一個
        if (chosenRoles.Contains(roleId))
        {
            if (roleHintText != null)
                roleHintText.text = "這個角色已選過，請選另一個。";
            return;
        }

        // 收下這次選擇
        if (chosenRoles.Count < 2)
        {
            chosenRoles.Add(roleId);
            if (roleHintText != null)
                roleHintText.text = $"已選擇：{GetRoleName(roleId)}（{chosenRoles.Count}/2）";
        }
    }

    private string GetRoleName(int roleId)
    {
        int idx = roleId - 1;
        if (roleNames == null || idx < 0 || idx >= roleNames.Length)
            return $"角色{roleId}";
        return roleNames[idx];
    }

    private string GetEventDisplayName(MythEventType e)
    {
        switch (e)
        {
            case MythEventType.Creation: return "創世";
            case MythEventType.HeroQuest: return "英雄之旅";
            case MythEventType.Trickery: return "詭計";
            case MythEventType.Cataclysm: return "災變";
            default: return "未知";
        }
    }

    private IEnumerator FadeOutIfNeeded()
    {
        if (fadeCanvas == null) yield break;

        fadeCanvas.gameObject.SetActive(true);
        float t = 0f;
        float start = fadeCanvas.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, 1f, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;
    }

    private void LoadSceneForCurrentEvent()
    {
        if (desCCScript != null)
        {
            desCCScript.AdvanceRound();
        }

        // 你原本可能有事件對應場景的表，這裡先用 switch 示範
        switch (currentEvent)
        {
            case MythEventType.Creation:
                SceneManager.LoadScene("01");
                break;
            case MythEventType.HeroQuest:
                SceneManager.LoadScene("02");
                break;
            case MythEventType.Trickery:
                SceneManager.LoadScene("03");
                break;
            case MythEventType.Cataclysm:
                SceneManager.LoadScene("04");
                break;
            default:
                Debug.LogWarning("沒有對應場景，currentEvent=" + currentEvent);
                break;
        }
    }

    private void Awake()
    {
        if (animationDes == null)
            animationDes = GetComponent<AnimationDes>();

        if (desCCScript == null)
            desCCScript = FindObjectOfType<DesCC>();

        if (dialogueSystemDesScript == null)
            dialogueSystemDesScript = FindObjectOfType<DialogueSystemDes>();
        dissolveEffectScript = FindObjectOfType<DissolveEffect>();
    }

    private void Start()
    {
        if (Card != null)
        {
            CardAnimator = Card.GetComponent<Animator>();
            CardCurrent = Card.GetComponent<SpriteRenderer>();
        }

        // ✅ 抽卡之前都先關著
        if (Card != null) Card.SetActive(false);

        // ✅ 角色/題目面板也都先關著
        if (topicPanel != null) topicPanel.SetActive(false);
        if (roleSelectPanel != null) roleSelectPanel.SetActive(false);
        if (roleButtonsRoot != null) roleButtonsRoot.SetActive(false);

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
            dissolveEffectScript.dissolveAmount = 1f;
            dissolveEffectScript.material.SetFloat("_DissolveAmount", dissolveEffectScript.dissolveAmount);
            CardAnimator.SetBool("CardShow", true);
        }
    }

    // 卡片收回 → 開始描述卡片用的對話
    public IEnumerator StartDescriptionDialogueIfNeeded()
    {
        if (!hasShownCardThisPull || descriptionStarted)
            yield return null;

        // 1. 等玩家按空白鍵
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        dissolveEffectScript.StartDissolve(true);

        yield return new WaitUntil(() => dissolveEffectScript.dissolveAmount == 0);

        yield return new WaitForSeconds(1f);
        descriptionStarted = true;
        if (dialogueSystemDesScript != null)
        {
            
            dialogueSystemDesScript.TextfileCurrent = dialogueSystemDesScript.TextfileDescriptionCard;
            dialogueSystemDesScript.TextPanel.SetActive(true);
            dialogueSystemDesScript.KeepTalk = true;
            dialogueSystemDesScript.autoNextLine = true;
            dialogueSystemDesScript.keepHistoryInPrologue = true;
        }

        
    }

    // 依照目前抽到的事件，顯示題目提示 + 打開角色選擇

    // 依事件類型回傳題目提示文字
    string GetTopicHintForEvent(MythEventType eventType)
    {
        switch (eventType)
        {
            case MythEventType.Creation:
                return "題目提示：創世與起源";          // 你之後可以改成從 ScriptableObject/表格讀
            case MythEventType.HeroQuest:
                return "題目提示：英雄的試煉";
            case MythEventType.Trickery:
                return "題目提示：詭計與欺瞞";
            case MythEventType.Cataclysm:
                return "題目提示：災變與毀滅";
            default:
                return "題目提示：未知的命運";
        }
    }

    // 之後 Arduino 收到角色 RFID 時，只要呼叫這個就好
    public void OnRolePlacedFromArduino(int slotIndex, int roleId)
    {
        // slotIndex 代表是第幾個位置（例如 0 = 左邊角色，1 = 右邊角色）
        // roleId 代表哪一隻角色公仔
        Debug.Log($"角色放置：槽位 {slotIndex}, 角色 ID {roleId}");

        // TODO：把角色記錄起來，等玩家按「完成」後一起判定
    }

}
