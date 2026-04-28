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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 设置背景音乐音量的方法，接受一个float类型的值作为参数，并将其应用到bgm的音量属性上，同时将该值保存到PlayerPrefs中，以便在下次游戏启动时加载
    public void SetVolume(float value)
    {
        bgm.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }

    // 加载背景音乐音量的方法，从PlayerPrefs中获取之前保存的音量值，如果没有保存过，则默认为1f（最大音量）
    public float LoadVolume()
    {
        return PlayerPrefs.GetFloat("volume", 1f);
    }
}