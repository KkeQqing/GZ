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

    private float oldVolume;   // 打开界面时记住原始音量
    private float tempVolume;  // 拖动时临时音量

    private void Awake()
    {
        saveBtn.onClick.AddListener(SaveSettings);
        backBtn.onClick.AddListener(OnBack);
        resetBtn.onClick.AddListener(OnReset);

        // 拖动实时更新临时音量 + 改试听音量
        volumeSlider.onValueChanged.AddListener(OnSliderDrag);
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 打开设置先记住原本音量
        oldVolume = AudioManager.Instance.LoadVolume();
        tempVolume = oldVolume;

        // 初始化滑块和文字
        volumeSlider.value = oldVolume;
        UpdateVolumeText(oldVolume);

        // 应用当前真实音量
        AudioManager.Instance.SetVolume(oldVolume);
    }

    // 拖动滑块：只临时改、实时试听，不保存
    void OnSliderDrag(float value)
    {
        tempVolume = value;
        UpdateVolumeText(value);

        // 实时改音量，马上能听到
        AudioManager.Instance.bgm.volume = tempVolume;
    }

    // 保存：把临时音量真正存档
    public void SaveSettings()
    {
        AudioManager.Instance.SetVolume(tempVolume);
        oldVolume = tempVolume; // 更新为新基准
        Debug.Log("已保存音量设置");
    }

    // 返回不保存：恢复旧音量
    public void OnBack()
    {
        // 恢复回进入设置前的旧音量
        AudioManager.Instance.bgm.volume = oldVolume;
        UIManager.Instance.Close();
        
    }

    // 重置默认
    public void OnReset()
    {
        tempVolume = 1f;
        volumeSlider.value = 1f;
        UpdateVolumeText(1f);
        // 只临时生效，不保存
        AudioManager.Instance.bgm.volume = tempVolume;
    }

    // 显示百分比
    void UpdateVolumeText(float value)
    {
        volumeValueText.text = (value * 100).ToString("0") + "%";
    }
}