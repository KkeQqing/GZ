using UnityEngine;

public class SettingManager  : MonoBehaviour
{
	// 单例
	public static SettingManager instance { get; private set; }

	private const string Key_Volume = "MasterVolume";
	private const string Key_FullScreen = "IsFullScreen";

	// 缓存变量，避免每次都去读盘
	private float masterVolume = 0.8f;
	private bool isFullScreen = true;

	[Header("Debug调试")]
	public bool DebugMode = true;

    private void Awake()
    {
        if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject); 
		}

		// 游戏启动时自动加载设置
		LoadSettings();

    }

	// 加载数据
	private void LoadSettings()
	{
		        
	}

	// 保存数据
	public void SaveSettings()
	{
		PlayerPrefs.SetFloat(Key_Volume, masterVolume);
		PlayerPrefs.SetInt(Key_FullScreen, isFullScreen ? 1 : 0);
		PlayerPrefs.Save();
		if (DebugMode) { Debug.Log("设置已保存"); }
	}

}