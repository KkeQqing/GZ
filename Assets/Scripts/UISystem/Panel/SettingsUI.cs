using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : BaseUI
{
    [Header("场景引用")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    public Button saveBtn;
    public Button backBtn;
    public Button resetBtn;

    private float oldVolume;

    protected override void Awake()
    {
        base.Awake();

        saveBtn.onClick.AddListener(SaveSettings);
        backBtn.onClick.AddListener(OnBack);
        resetBtn.onClick.AddListener(OnReset);
        volumeSlider.onValueChanged.AddListener(OnSliderDrag);
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 打开界面时记录当前音量
        oldVolume = AudioManager.Instance.LoadVolume();
        volumeSlider.value = oldVolume;
        UpdateVolumeText(oldVolume);
    }

    // 滑动时：只临时试听（不保存）
    void OnSliderDrag(float value)
    {
        UpdateVolumeText(value);
        AudioManager.Instance.SetVolume(value);
    }

    // 保存：确认修改
    public void SaveSettings()
    {
        AudioManager.Instance.SetVolume(volumeSlider.value);
        oldVolume = volumeSlider.value;
        Debug.Log("音量已保存");
    }

    // 返回：恢复打开前的音量
    public void OnBack()
    {
        AudioManager.Instance.SetVolume(oldVolume);
        UIManager.Instance.Close();
    }

    // 重置为 100%
    public void OnReset()
    {
        volumeSlider.value = 1f;
        AudioManager.Instance.SetVolume(1f);
        UpdateVolumeText(1f);
    }

    void UpdateVolumeText(float value)
    {
        volumeValueText.text = (value * 100).ToString("0") + "%";
    }
}