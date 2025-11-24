using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    void Start()
    {
        
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("description");
    }

    public void BackToChooseEvent()
    {
        SceneManager.LoadScene("description");
    }
}
