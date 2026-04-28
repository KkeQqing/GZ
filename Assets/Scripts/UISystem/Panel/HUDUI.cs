using UnityEngine;
using UnityEngine.UI;

public class HUDUI : BaseUI
{
    public Image hpBar;

    public void UpdateHP(float current, float max)
    {
        hpBar.fillAmount = current / max;
    }

    public void OnPause()
    {
        GameManager.Instance.TogglePause();
        UIManager.Instance.Open(UIType.Pause);
    }
}