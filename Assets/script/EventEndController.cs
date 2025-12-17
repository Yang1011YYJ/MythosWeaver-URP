using UnityEngine;
using UnityEngine.SceneManagement;

public class EventEndController : MonoBehaviour
{

    [Header("回合設定（預設值）")]
    public int defaultMaxRounds = 4;

    private const string KEY_ROUND = "ROUND_INDEX";
    private const string KEY_MAX = "MAX_ROUNDS";

    // UI Button onClick -> 你可以直接勾這個（不用傳 bool 也行）
    public void OnEventFinished()
    {
        // 讀目前值
        int roundIndex = PlayerPrefs.GetInt(KEY_ROUND, 0);
        int maxRounds = PlayerPrefs.GetInt(KEY_MAX, defaultMaxRounds);

        // ✅ 次數 +1
        roundIndex++;
        PlayerPrefs.SetInt(KEY_ROUND, roundIndex);
        PlayerPrefs.Save();

        // ✅ 判斷跳哪裡
        if (roundIndex >= maxRounds)
        {
            SceneManager.LoadScene("Des");
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    // 如果你真的想保留 bool 版本也行
    public void OnEventFinishedBool(bool finished)
    {
        if (finished) OnEventFinished();
    }
}
