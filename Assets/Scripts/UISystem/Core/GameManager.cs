using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused;

    private bool debugMode = true; // 是否启用调试模式

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保GameManager在场景切换时不会被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁新的实例
        }

        UIManager.Instance.Open(UIType.MainMenu);
    }

    public void StartGame()
    {
        if(debugMode)Debug.Log("Starting Game...");
        UIManager.Instance.Open(UIType.HUD);
        UIManager.Instance.Close(); // 关闭主菜单
    }

    // 暂停游戏
    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
            UIManager.Instance.Open(UIType.Pause);
        else
            UIManager.Instance.Close();
    }

    public void TestClick()
    {
        Debug.Log("按钮被点到了！");
    }
}