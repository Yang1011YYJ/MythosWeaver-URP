using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    public string drawEventSceneName = "DrawEventScene";
    public GameObject FatherP;
    public AnimationMenu animationMenuScript;

    public void StartNewGame()
    {
        // 新局：清回合
        PlayerPrefs.SetInt("ROUND_INDEX", 0);
        // maxRounds 你要不要清，看你是否允許玩家在首頁設定
        // PlayerPrefs.DeleteKey("MAX_ROUNDS");
        PlayerPrefs.Save();

        //SceneManager.LoadScene(drawEventSceneName);
        animationMenuScript.Fade(FatherP, 2, 1, 0, null);
        FatherP.SetActive(false);
    }

    public void ContinueGame()
    {
        // 續局：直接進抽事件場景，不重置
        //SceneManager.LoadScene(drawEventSceneName);
        animationMenuScript.Fade(FatherP, 2, 1, 0, null);
        FatherP.SetActive(false);
    }
}
