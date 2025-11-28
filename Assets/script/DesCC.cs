using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesCC : MonoBehaviour
{
    public AnimationDes animationDesScript;

    [Header("UI")]
    public GameObject Panel;
    // Start is called before the first frame update
    void Start()
    {
        animationDesScript = GetComponent<AnimationDes>();

        animationDesScript.FadeOut(2f, Panel.GetComponent<CanvasGroup>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
