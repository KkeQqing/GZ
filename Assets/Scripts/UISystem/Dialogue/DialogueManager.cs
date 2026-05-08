using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private DialogueChapter currentChapter;

    // 统一配置资源文件夹
    private readonly string PORTRAIT_FOLDER = "UIResources/";

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

    public void LoadChapter(string chapterFileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", chapterFileName);

        if (!File.Exists(path))
        {
            Debug.LogError("章节JSON不存在：" + path);
            return;
        }

        string json = File.ReadAllText(path);
        currentChapter = JsonUtility.FromJson<DialogueChapter>(json);
        Debug.Log("加载章节成功：" + currentChapter.chapterName);
    }

    public List<DialogueLine> GetCurrentDialogueLines()
    {
        return currentChapter?.lines;
    }

    // 全局统一加载立绘
    public Sprite LoadPortrait(string resPath)
    {
        if (string.IsNullOrEmpty(resPath)) return null;

        // 自动拼接 UIResources 路径
        string fullPath = PORTRAIT_FOLDER + resPath;
        Sprite sprite = Resources.Load<Sprite>(fullPath);

        if (sprite == null)
            Debug.LogError("❌ 图片加载失败：" + fullPath);
        else
            Debug.Log("✅ 图片加载成功：" + fullPath);

        return sprite;
    }
}