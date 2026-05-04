using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused;

    private bool debugMode = true; // 是否启用调试模式

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保GameManager在场景切换时不会被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁新的实例
        }

    }

    void Start()
    {
        if (UIManager.Instance != null)
        {
            Debug.Log("尝试打开主菜单...");
            UIManager.Instance.Open(UIType.MainMenu);
        }
        else
        {
            Debug.LogError("UIManager 没找到！");
        }
    }

    // 开始游戏
    public void StartGame()
    {
        if(debugMode)Debug.Log("Starting Game...");
        UIManager.Instance.Close(UIType.MainMenu);
        UIManager.Instance.Open(UIType.HUD);

        if (debugMode) StartTestDialogue();
    }

    // 对话系统调试方法
    void StartTestDialogue()
    {
        // 加载第一章
        DialogueManager.Instance.LoadChapter("test.json");
        var dialogueLines = DialogueManager.Instance.GetCurrentDialogueLines();

        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            Debug.LogError("章节对话为空！");
            return;
        }

        // 打开对话UI
        UIManager.Instance.Open(UIType.Dialogue);
        BaseUI baseUI = UIManager.Instance.GetUI(UIType.Dialogue);
        if (baseUI is DialogueUI dui)
        {
            dui.StartDialogue(dialogueLines);
        }
    }

    // 暂停游戏
    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
            UIManager.Instance.Open(UIType.Pause);
        else
            UIManager.Instance.Close();
    }



    public void TestClick()
    {
        Debug.Log("按钮被点到了！");
    }
}