using System.Collections.Generic;
using UnityEngine;

public class EventDatabase : MonoBehaviour
{
    public static EventDatabase Instance;

    [System.Serializable]
    public class EventConfig
    {
        public string displayName; // 顯示在面板上的文字，例如「事件一：洪水」
        public string sceneName;   // 對應要切換的場景名稱（要跟 Build Settings 一樣）
        public int id;     // 你可以加 ID 或額外屬性
    }

    [Header("事件清單（可自由增減）")]
    public List<EventConfig> events = new List<EventConfig>();//事件數量

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ★ 永不刪除
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public EventConfig GetEventByIndex(int index)
    {
        if (index >= 0 && index < events.Count)
            return events[index];
        return null;
    }
}
