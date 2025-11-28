using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythRippleTrigger : MonoBehaviour
{
    public Material rippleMat;
    public Camera cam;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 m = Input.mousePosition;
            Vector2 uv = new Vector2(m.x / Screen.width, m.y / Screen.height);

            // 告訴 Shader：新的水滴中心＆開始時間
            rippleMat.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
            rippleMat.SetFloat("_StartTime", Time.time);
        }
    }
}
