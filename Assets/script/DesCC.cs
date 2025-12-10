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
    public Button pulleventbutton;
    [Tooltip("捕捉傳送回來的資訊(先這樣用)")] public TMP_InputField inputField;

    [Header("對話")]
    public bool desStart;//起始的說明文字(沒有說話人)

    [Header("Arduino相關數據")]
    [Tooltip("可以拉拉桿")]public bool CanPullEvent = false;
    [Tooltip("從拉桿接收到的數值")]public float PullEvent=0;
    
    // Start is called before the first frame update
    void Start()
    {
        animationDesScript = GetComponent<AnimationDes>();
        DialogueSystemDesScript = GetComponent<DialogueSystemDes>();
        inputField.gameObject.SetActive(false);
        pulleventbutton.gameObject.SetActive(false);
        // 把「淡入後要做什麼」包成一個 function 丟進去
        animationDesScript.Fade(
            Panel,
            0f,
            1f,
            2f/*持續時間*/,
            OnFadeFinished
        );
        
    }
    void OnFadeFinished()
    {
        DialogueSystemDesScript.playOnEnable = true;
        DialogueSystemDesScript.StartDialogue();
        desStart = true;
        DialogueSystemDesScript.autoNextLine = true;
        DialogueSystemDesScript.keepHistoryInPrologue = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PullEventSet()
    {
        PullEvent = 5;
    }
}
