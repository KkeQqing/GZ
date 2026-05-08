using UnityEngine;

[DefaultExecutionOrder(-1)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgm;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 游戏启动时自动加载音量并应用
            float savedVolume = LoadVolume();
            if (bgm != null)
            {
                bgm.volume = savedVolume;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float value)
    {
        if (bgm != null)
        {
            bgm.volume = value;
        }
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save(); // 确保保存
    }

    public float LoadVolume()
    {
        return PlayerPrefs.GetFloat("volume", 1f);
    }
}