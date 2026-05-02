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
        Debug.Log("启动调试对话！");

        // 构造测试对话
        List<DialogueLine> testDialogue = new List<DialogueLine>
        {
            new DialogueLine
            {
                speakerName = "Test Protagonist", 
                text = "Greetings! This is the opening line.",
                portrait = null
            },
            new DialogueLine
            {
                speakerName = "Test NPC", 
                text = "And this is the second line!",
                portrait = null
            },
            new DialogueLine
            {
                speakerName = "Test ",
                text = "And this is the third line!",
                portrait = null
            },
            new DialogueLine
            {
                speakerName = "Test System",
                text = "Dialogue test complete! Click Next to close.",
                portrait = null
            }
        };

        // 打开对话UI
        UIManager.Instance.Open(UIType.Dialogue);

        // 用GetUI方法安全获取DialogueUI
        BaseUI baseUI = UIManager.Instance.GetUI(UIType.Dialogue);
        if (baseUI != null && baseUI is DialogueUI dialogueUI)
        {
            dialogueUI.StartDialogue(testDialogue);
        }
        else
        {
            Debug.LogError("DialogueUI 未找到或转换失败！请检查DialogueUI是否已注册到UIManager。");
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