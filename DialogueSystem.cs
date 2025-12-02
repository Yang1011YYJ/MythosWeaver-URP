using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Tooltip("打字協程")] public Coroutine _typing;
    [Tooltip("下一句按鈕")] public GameObject _continueButton;
    [Tooltip("對話面板")] public GameObject _dialoguePanel;
    [Tooltip("角色大頭照")] public Image _characterImage;
    [Tooltip("對話的內容")] public Queue<Dialogue> _dialogues;
    [Tooltip("對話框文字")] public TextMeshProUGUI _dialogueText;
    [Tooltip("說話的姓名")] public TextMeshProUGUI _nameText;
    [Tooltip("對話速度")] public WordSpeedTypes _wordSpeedTypes;
    [Tooltip("對話速度")] public float _wordSpeed;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 對話文字數度
    /// </summary>
    

}
