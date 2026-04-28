using UnityEngine;

public class PauseUI : BaseUI
{
    // 继续游戏
    public void OnResume()
    {
        GameManager.Instance.TogglePause();
        UIManager.Instance.Close();
    }

    public void OnSettings()
    {
        UIManager.Instance.Open(UIType.Settings);
    }

    public void OnExit()
    {
        Time.timeScale = 1f;
        UIManager.Instance.Open(UIType.MainMenu);
    }
}