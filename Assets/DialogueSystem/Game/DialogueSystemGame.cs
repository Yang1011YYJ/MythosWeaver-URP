using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystemGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI Name;
    public Image FaceImage;

    [Header("文本")]
    public TextAsset TextfileCurrent;
    public TextAsset TextfileDes;

    [Header("其他")]
    public int index;
    List<string> TextList = new List<string>();

    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {

        GetTextFromFile(TextfileCurrent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void GetTextFromFile(TextAsset file)
    {
        TextList.Clear();
        index = 0;

        var lineData = file.text.Split('\n');

        foreach(var line in lineData)
        {
            TextList.Add(line);
        }
    }
}
