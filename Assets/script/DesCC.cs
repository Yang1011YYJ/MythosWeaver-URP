using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesCC : MonoBehaviour
{
    [Header("腳本")]
    public DialogueSystemDes DialogueSystemDesScript;
    public AnimationDes animationDesScript;

    [Header("UI")]
    public GameObject Panel;

    [Header("對話")]
    public bool desStart;//起始的說明文字(沒有說話人)
    // Start is called before the first frame update
    void Start()
    {
        animationDesScript = GetComponent<AnimationDes>();
        DialogueSystemDesScript = GetComponent<DialogueSystemDes>();
        animationDesScript.FadeOut(2f, Panel.GetComponent<CanvasGroup>());

        desStart = true;
        DialogueSystemDesScript.autoNextLine = true;
        DialogueSystemDesScript.keepHistoryInPrologue = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
