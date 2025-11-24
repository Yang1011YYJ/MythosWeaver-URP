using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythRippleTrigger : MonoBehaviour
{
    [Header("套著水波 Shader 的材質")]
    public Material rippleMat;

    [Header("波紋維持多久(秒)")]
    public float rippleDuration = 1.2f;

    float rippleStartTime = -1f;

    void Start()
    {
        if (rippleMat != null)
        {
            // 一開始關閉波紋
            rippleMat.SetFloat("_RippleActive", 0f);
        }
    }

    // 這個給你點擊或事件呼叫用：uv 是 0~1 的位置
    public void TriggerRippleAtUV(Vector2 uv)
    {
        if (rippleMat == null) return;

        rippleStartTime = Time.time;

        rippleMat.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
        rippleMat.SetFloat("_StartTime", rippleStartTime);
        rippleMat.SetFloat("_RippleActive", 1f);   // 打開波紋
    }

    // 如果只是要在畫面中心出現，可以包一個這個
    public void TriggerAtCenter()
    {
        TriggerRippleAtUV(new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (rippleMat == null) return;

        // 有啟動過才檢查
        if (rippleStartTime > 0f)
        {
            float elapsed = Time.time - rippleStartTime;

            if (elapsed > rippleDuration)
            {
                // 關掉波紋，恢復到完全靜止
                rippleMat.SetFloat("_RippleActive", 0f);
                rippleStartTime = -1f;
            }
        }

        // ↓ 如果你還是想保留「用滑鼠點哪裡就哪裡起波紋」，可以加在這：
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 uv = new Vector2(
                mousePos.x / Screen.width,
                mousePos.y / Screen.height
            );
            TriggerRippleAtUV(uv);
        }
    }
}
