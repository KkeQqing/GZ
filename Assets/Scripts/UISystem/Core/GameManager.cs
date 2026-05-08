using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 外部调整血量：调用 GameManager.Instance.DamageHUD(10) 来减少血量，调用 GameManager.Instance.HealHUD(10) 来增加血量。
/// 外部触发某一章节对话：调用 GameManager.Instance.StartDialogueWithChapter("Chapter1.json") 来加载并显示章节对话，参数是 StreamingAssets/Dialogues 文件夹下的 JSON 文件名。
/// debug模式下：可以在 Start() 方法中直接调用 StartDialogueWithChapter 来测试对话系统
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused;
    public bool debugMode = true;

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

    void Start()
    {
        if (UIManager.Instance != null)
        {
            if (debugMode) Debug.Log("打开主菜单");
            UIManager.Instance.Open(UIType.MainMenu);
        }
        else
        {
            if (debugMode) Debug.LogError("UIManager 丢失");
        }
    }

    // 开始游戏
    public void StartGame()
    {
        if (debugMode) Debug.Log("开始游戏");
        UIManager.Instance.Close();
        UIManager.Instance.Open(UIType.HUD);

        if (debugMode)
        {
            StartDialogueWithChapter("Chapter1.json");
        }
    }

    // 对话调试
    public void StartDialogueWithChapter(string chapterFileName)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager 不存在");
            return;
        }

        DialogueManager.Instance.LoadChapter(chapterFileName);
        var lines = DialogueManager.Instance.GetCurrentDialogueLines();

        if (lines == null || lines.Count == 0)
        {
            Debug.LogError("章节无内容");
            return;
        }

        UIManager.Instance.Open(UIType.Dialogue);
        BaseUI ui = UIManager.Instance.GetUI(UIType.Dialogue);

        if (ui is DialogueUI dialogueUI)
        {
            dialogueUI.StartDialogue(lines);
        }
    }

    // 暂停开关
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
            UIManager.Instance.Open(UIType.Pause);
        else
            UIManager.Instance.Close();
    }

    // 血量调整接口
    public void DamageHUD(float damage)
    {
        if (UIManager.Instance == null) return;

        HUDUI hud = UIManager.Instance.GetUI(UIType.HUD) as HUDUI;
        if (hud != null)
        {
            hud.TakeDamage(damage);
        }
    }

    public void HealHUD(float amount)
    {
        if (UIManager.Instance == null) return;

        HUDUI hud = UIManager.Instance.GetUI(UIType.HUD) as HUDUI;
        if (hud != null)
        {
            hud.Heal(amount);
        }
    }

    public void TestClick()
    {
        Debug.Log("按钮点击成功！");
    }
}