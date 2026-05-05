using System.Collections.Generic;
using UnityEngine;

//在任何需要更新血量的地方（比如玩家受伤、加血逻辑），都可以这样调用：
// GameManager.Instance.DamageHUD(20); // 扣20血
// GameManager.Instance.HealHUD(15);   // 加15血

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused;

    public bool debugMode = true; // 是否启用调试模式


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
            if(debugMode) Debug.Log("尝试打开主菜单...");
            UIManager.Instance.Open(UIType.MainMenu);
        }
        else
        {
            if(debugMode) Debug.LogError("UIManager 没找到！");
        }
    }

    // 开始游戏
    public void StartGame()
    {
        if(debugMode)Debug.Log("Starting Game...");
        UIManager.Instance.Close();
        UIManager.Instance.Open(UIType.HUD);

        if (debugMode)
        {
            StartDialogueWithChapter("Chapter1.json");
        }

    }


    // 对话系统调试方法
    public void StartDialogueWithChapter(string chapterFileName)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager 不存在！");
            return;
        }

        // 1. 加载JSON
        DialogueManager.Instance.LoadChapter(chapterFileName);
        var lines = DialogueManager.Instance.GetCurrentDialogueLines();

        if (lines == null || lines.Count == 0)
        {
            Debug.LogError($"章节 {chapterFileName} 无内容！");
            return;
        }

        // 2. 打开UI
        UIManager.Instance.Open(UIType.Dialogue);
        BaseUI ui = UIManager.Instance.GetUI(UIType.Dialogue);

        if (ui is DialogueUI dialogueUI)
        {
            dialogueUI.StartDialogue(lines);
        }
        else
        {
            Debug.LogError("获取 DialogueUI 失败！");
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

    // 扣血
    public void DamageHUD(float damage)
    {
        HUDUI hud = UIManager.Instance.GetUI(UIType.HUD) as HUDUI;
        if (hud != null)
        {
            hud.TakeDamage(damage);
        }
    }

    // 加血
    public void HealHUD(float heal)
    {
        HUDUI hud = UIManager.Instance.GetUI(UIType.HUD) as HUDUI;
        if (hud != null)
        {
            hud.Heal(heal);
        }
    }

    public void TestClick()
    {
        Debug.Log("按钮被点到了！");
    }
}