using UnityEngine;
using UnityEngine.UI;

public class PauseUI : BaseUI
{
    public Button exitButton;
    public Button backButton;
    public Button settingButton;

    protected override void Awake()
    {
        base.Awake();
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitToMainMenu);
        if (backButton != null)
            backButton.onClick.AddListener(OnResume);
        if (settingButton != null)
            settingButton.onClick.AddListener(OnSettings);
    }


    // 继续游戏
    public void OnResume()
    {
        GameManager.Instance.TogglePause();
        UIManager.Instance.Close(); // 只关自己，不动HUD
        UIManager.Instance.Open(UIType.HUD); // 确保HUD在前面显示
    }

    public void OnSettings()
    {
        UIManager.Instance.Open(UIType.Settings);
    }

    // 回到主菜单（必须用 CloseAll 清空栈）
    public void OnExitToMainMenu()
    {
        Time.timeScale = 1f;
        UIManager.Instance.CloseAll(); // 清空UI栈
        GameManager.Instance.TogglePause(); // 确保时间恢复
        UIManager.Instance.Open(UIType.MainMenu);
    }
}