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

    public void SetVolume(float value)
    {
        if (bgm != null)
        {
            bgm.volume = value;
        }
        PlayerPrefs.SetFloat("volume", value);
    }

    public float LoadVolume()
    {
        return PlayerPrefs.GetFloat("volume", 1f);
    }
}