using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [Header("材質與參數")]
    public Material material;                 // 使用溶解 Shader 的材質
    public string dissolvePropertyName = "_DissolveAmount"; // Shader 內的參數名稱
    public float dissolveAmount = 1f;         // 1 = 完整顯示, 0 = 完全溶解
    //public float defaultDuration = 1f;        // 預設溶解時間（秒）

    [Header("狀態")]
    public bool isDissolving = false;

    private Coroutine dissolveCoroutine;
    private void Awake()
    {
        if (material != null)
        {
            material.SetFloat(dissolvePropertyName, dissolveAmount);
        }
    }
    void Start()
    {
        material.SetFloat("_DissolveAmount", dissolveAmount);
    }

    public void StartDissolve(bool dissolveOut)
    {
        if (material == null)
        {
            Debug.LogWarning("DissolveEffect：沒有指定材質！");
            return;
        }

        // 目標值：溶出 -> 0，溶入 -> 1
        float target = dissolveOut ? 0f : 1f;

        // 起始值：溶出 -> 1，溶入 -> 0
        dissolveAmount = dissolveOut ? 1f : 0f;
        material.SetFloat("_DissolveAmount", dissolveAmount);

        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        dissolveCoroutine = StartCoroutine(DissolveRoutine(target));
    }


    public IEnumerator DissolveRoutine(float target)
    {
        isDissolving = true;

        while (!Mathf.Approximately(dissolveAmount, target))
        {
            if(target == 1)//起始是0要加
            {
                dissolveAmount = Mathf.Clamp01(dissolveAmount + Time.deltaTime);
            }
            else if(target == 0)//起始是1要減
            {
                dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            }
            material.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;//等一幀
        }

        yield return new WaitForSeconds(1f);

        // 收尾：確保數值剛好等於 target
        dissolveAmount = target;
        material.SetFloat(dissolvePropertyName, dissolveAmount);

        isDissolving = false;
        dissolveCoroutine = null;
    }
}
