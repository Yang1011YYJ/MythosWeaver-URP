using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorEvents : MonoBehaviour
{
    [Header("¸}¥»")]
    public ChooseEventss chooseEventssScript;
    // Start is called before the first frame update
    void Start()
    {
        chooseEventssScript = FindAnyObjectByType<ChooseEventss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hideObject()
    {
        gameObject.SetActive(false);
        chooseEventssScript.StartDescriptionDialogueIfNeeded();
    }
}
