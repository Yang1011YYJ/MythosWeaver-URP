using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueSystemDes : MonoBehaviour
{
    [Header("UI")]
    public GameObject TextPanel;
    public TextMeshProUGUI TextLabel;
    public Image FaceImage;
    public TextMeshProUGUI Name;

    [Header("文本")]
    public TextAsset TextfileCurrent;
    public TextAsset TextfileDes;
    public TextAsset TextfileHowToPlay;
    public TextAsset TextfileDescriptionCard;

    [Header("其他設定")]
    [Tooltip("讀到第幾行")] public int index;   
    [Tooltip("控制打字節奏（字元出現的間隔時間）")] public float TextSpeed = 0.06f;
    [Tooltip("每個字元淡入所需的時間（建議略大於 TextSpeed）")]public float CharFadeInDuration = 0.18f;
    [Tooltip("繼續對話")] public bool KeepTalk;
    [Tooltip("對話中")] public bool IsTalking;

    [Header("自動播放設定")]
    [Tooltip("true 就自動下一行")]public bool autoNextLine = false;
    [Tooltip("每行播完後停多久再自動下一行")] public float autoNextDelay = 0.5f;

    [Header("控制設定")]
    [Tooltip("物件啟用時是否自動開始播放對話")]public bool playOnEnable;
    [Tooltip("顯示文字是否清空")] public bool keepHistoryInPrologue;
    public GameObject TextNextHint;
    List<string> TextList = new List<string>();
    [Tooltip("標記是否正在打字")] public bool isTyping = false;

    [Header("腳本")]
    public DesCC desCCScript;
    public ChooseEventss chooseEventssScript;

    // 可以選擇把當前打字協程記著，方便 Stop
    private Coroutine typingRoutine;

    private void Awake()
    {
        desCCScript = EventSystem.current.GetComponent<DesCC>();
        chooseEventssScript = EventSystem.current.GetComponent<ChooseEventss>();

        TextfileCurrent = TextfileDes;
        GetTextFromFile(TextfileCurrent);
    }

    void Start()
    {
        if (desCCScript.desStart)
        {
            // 目前你沒寫東西，就先留著
        }

        index = 0;
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            // 一開始直接抓到第一句
            if (TextList.Count > 0)
            {
                typingRoutine = StartCoroutine(SetTextUI());
            }
        }
        
    }

    void Update()
    {
        if (autoNextLine)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                CompleteCurrentLine();
                return;
            }

            if (index == TextList.Count)
            {
                if(TextfileCurrent == TextfileDes)
                {
                    Debug.Log("11");
                    TextfileCurrent = TextfileHowToPlay;
                    GetTextFromFile(TextfileCurrent);
                    index = 0;
                    autoNextLine = true;
                    keepHistoryInPrologue = true;
                    typingRoutine = StartCoroutine(SetTextUI());
                }
                else if(TextfileCurrent == TextfileHowToPlay)//等待玩家拉桿
                {
                    Debug.Log("111");
                    chooseEventssScript.EnableLever();
                    desCCScript.inputField.gameObject.SetActive(true);
                    desCCScript.pulleventbutton.gameObject.SetActive(true);
                }
                else if(TextfileCurrent == TextfileDescriptionCard)
                {

                }
                else
                {
                    TextPanel.SetActive(false);
                    TextNextHint.SetActive(false);
                    index = 0;
                    return;
                }
                
            }
            else
            {
                // 播下一行
                if (typingRoutine != null)
                    StopCoroutine(typingRoutine);

                typingRoutine = StartCoroutine(SetTextUI());
            }

            
        }

        if (KeepTalk)
        {
            KeepTalk = false;
            index = 0;
            GetTextFromFile(TextfileCurrent);
            StartDialogue();
        }
    }

    void GetTextFromFile(TextAsset file)
    {
        TextList.Clear();
        index = 0;

        var lineData = file.text.Split('\n');

        foreach (var line in lineData)
        {
            TextList.Add(line);
        }
    }

    public void StartDialogue()
    {
        playOnEnable = true;
        if (TextList.Count == 0) return;

        // 避免 index 跑出範圍
        if (index < 0 || index >= TextList.Count)
        {
            index = 0;
        }

        TextPanel.SetActive(true);

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(SetTextUI());
    }
    // 立刻完成當前行的顯示
    void CompleteCurrentLine()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }

        // 重新組出「這一瞬間完整應該看到的文字」
        BuildTextForCurrentLine(out string fullText, out int baseCharCount);

        TextLabel.text = fullText;
        TextLabel.ForceMeshUpdate();

        TMP_TextInfo textInfo = TextLabel.textInfo;
        int totalChars = textInfo.characterCount;

        TextLabel.maxVisibleCharacters = totalChars;

        // 把所有字 alpha 拉滿（包含前面舊的 + 本行新的）
        for (int i = 0; i < totalChars; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

            byte r = vertexColors[vertIndex + 0].r;
            byte g = vertexColors[vertIndex + 0].g;
            byte b = vertexColors[vertIndex + 0].b;
            byte a = 255;

            vertexColors[vertIndex + 0] = new Color32(r, g, b, a);
            vertexColors[vertIndex + 1] = new Color32(r, g, b, a);
            vertexColors[vertIndex + 2] = new Color32(r, g, b, a);
            vertexColors[vertIndex + 3] = new Color32(r, g, b, a);
        }

        TextLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        isTyping = false;
        index++;
    }
    IEnumerator SetTextUI()
    {
        isTyping = true;

        // 組出完整文字 + 前面舊文字字數
        BuildTextForCurrentLine(out string fullText, out int baseCharCount);

        // 塞進 TextLabel
        TextLabel.text = fullText;
        TextLabel.ForceMeshUpdate();

        TMP_TextInfo textInfo = TextLabel.textInfo;
        int totalChars = textInfo.characterCount;

        // 先讓前面「舊的文字」全部可見 & alpha = 255
        TextLabel.maxVisibleCharacters = totalChars; // 先全部打開，等下再管 alpha

        for (int i = 0; i < baseCharCount && i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                var c = vertexColors[vertIndex + j];
                vertexColors[vertIndex + j] = new Color32(c.r, c.g, c.b, 255);
            }
        }
        TextLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        // 現在處理「本行新文字」──從 baseCharCount 開始，逐字出現＋淡入
        // 這邊用 maxVisibleCharacters 來控制「打字機」的感覺
        TextLabel.maxVisibleCharacters = baseCharCount;

        for (int i = baseCharCount; i < totalChars; i++)
        {
            TextLabel.maxVisibleCharacters = i + 1;

            // 柔柔淡入這個字
            StartCoroutine(FadeInCharacter(i));

            yield return new WaitForSeconds(TextSpeed);
        }

        isTyping = false;
        // 這一行已經完整顯示完了，準備進下一行
        index++;

        if (autoNextLine)
        {

            // 已經是最後一行了
            if (index >= TextList.Count)
            {
                // 播完全部 → 可以直接關面板或留著
                autoNextLine = false;
                typingRoutine = null;
                yield break;
            }

            // 還有下一行 → 等一小段時間再播下一句
            yield return new WaitForSeconds(autoNextDelay);

            typingRoutine = StartCoroutine(SetTextUI());
        }
        else
        {
            // 手動模式：停在這裡，等玩家按空白
            typingRoutine = null;
        }
    }


    // 單個字元的柔和淡入效果（風格 1）
    IEnumerator FadeInCharacter(int charIndex)
    {
        // 再保險更新一次（避免在打字過程中 TextInfo 沒更新到）
        TextLabel.ForceMeshUpdate();
        TMP_TextInfo textInfo = TextLabel.textInfo;

        if (charIndex < 0 || charIndex >= textInfo.characterCount)
            yield break;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

        if (!charInfo.isVisible)
            yield break; // 空格或換行直接跳過

        int matIndex = charInfo.materialReferenceIndex;
        int vertIndex = charInfo.vertexIndex;

        Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

        // 先記住原本 RGB（避免你之後用別的顏色時被寫死成白色）
        byte r = vertexColors[vertIndex + 0].r;
        byte g = vertexColors[vertIndex + 0].g;
        byte b = vertexColors[vertIndex + 0].b;

        float timer = 0f;

        // 一開始先把這個字的 alpha 歸 0（真正從「浮出來」開始）
        for (int j = 0; j < 4; j++)
        {
            vertexColors[vertIndex + j].a = 0;
        }
        TextLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        while (timer < CharFadeInDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / CharFadeInDuration);

            // 🔮 使用「緩出（ease-out）」的曲線，讓開頭快、尾巴慢
            // 這樣看起來會比較柔，不會像直線那麼硬
            float eased = t * t * (2f - t); // 0~1 的 easeOutQuad

            byte alpha = (byte)Mathf.Lerp(0, 255, eased);

            vertexColors[vertIndex + 0] = new Color32(r, g, b, alpha);
            vertexColors[vertIndex + 1] = new Color32(r, g, b, alpha);
            vertexColors[vertIndex + 2] = new Color32(r, g, b, alpha);
            vertexColors[vertIndex + 3] = new Color32(r, g, b, alpha);

            TextLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return null;
        }

        // 收尾保證滿亮
        vertexColors[vertIndex + 0] = new Color32(r, g, b, 255);
        vertexColors[vertIndex + 1] = new Color32(r, g, b, 255);
        vertexColors[vertIndex + 2] = new Color32(r, g, b, 255);
        vertexColors[vertIndex + 3] = new Color32(r, g, b, 255);
        TextLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    /// <summary>
    /// 取得這一刻要顯示在 TextLabel 上的完整文字，
    /// 以及「前面已存在的字數」（用來當打字的起點）。
    /// </summary>
    void BuildTextForCurrentLine(out string fullText, out int baseCharCount)
    {
        // 普通模式：只顯示當前這一句
        if (!keepHistoryInPrologue)
        {
            fullText = TextList[index];
            baseCharCount = 0;
            return;
        }

        // 序章模式：把前面所有已經播過的句子留著
        string previousBlock = "";

        if (index > 0)
        {
            // 把 0 ~ index-1 行接在一起，中間用換行
            previousBlock = string.Join("\n", TextList.GetRange(0, index));
        }

        if (!string.IsNullOrEmpty(previousBlock))
        {
            // 前面舊文字 + 換行 + 當前這一句
            string prefix = previousBlock + "\n";
            baseCharCount = prefix.Length;  // 這些字一開始就該是「已存在」
            fullText = prefix + TextList[index];
        }
        else
        {
            // 這是第一句，前面沒有歷史
            fullText = TextList[index];
            baseCharCount = 0;
        }
    }

}
