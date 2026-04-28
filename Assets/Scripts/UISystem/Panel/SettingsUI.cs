using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : BaseUI
{
    public Slider volumeSlider;

    public override void Init()
    {
        float v = AudioManager.Instance.LoadVolume();
        volumeSlider.value = v;

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }

    public void OnClose()
    {
        UIManager.Instance.Close();
    }

    public void OnReset()
    {
        volumeSlider.value = 1f;
    }
}