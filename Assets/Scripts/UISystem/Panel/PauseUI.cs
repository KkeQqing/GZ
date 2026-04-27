using UnityEngine;

public class PauseUI : BaseUI
{
    public void OnResume()
    {
        GameManager.Instance.TogglePause();
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