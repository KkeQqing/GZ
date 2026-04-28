using UnityEngine;

public class MainMenuUI : BaseUI
{
    public void OnStart()
    {
        GameManager.Instance.StartGame();
    }

    public void OnEnding()
    {
        UIManager.Instance.Open(UIType.Ending);
    }

    public void OnCodex()
    {
        UIManager.Instance.Open(UIType.Codex);
    }
    public void OnSettings()
    {
        UIManager.Instance.Open(UIType.Settings);
    }
}