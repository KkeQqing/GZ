using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : BaseUI
{
    [Header("场景引用")]
    public TextMeshProUGUI speakerNameText; // 说话人名字
    public TextMeshProUGUI dialogueText;    // 对话文本
    public Image portrait;                  // 立绘
    public Button nextBtn;                  // Next按钮
    public Toggle autoToggle;               // 自动模式开关

    [Header("设置")]
    public float typeSpeed = 0.05f; // 打字机速度（每个字的间隔）
    public float autoDelay = 1.5f;  // 自动模式下的停留时间

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping;      // 是否正在打字
    private bool autoMode;      // 是否自动模式
    private Coroutine autoCoroutine;

    private void Awake()
    {
        // 绑定按钮事件
        nextBtn.onClick.AddListener(OnNextClicked);
        autoToggle.onValueChanged.AddListener(ToggleAutoMode);
    }

    // 打开对话界面时初始化
    protected override void OnShow()
    {
        base.OnShow();
        // 初始化状态
        isTyping = false;
        dialogueText.text = "";
        speakerNameText.text = "";
        portrait.sprite = null;
    }

    // 开始对话（外部调用，传入对话列表）
    public void StartDialogue(List<DialogueLine> dialogue)
    {
        dialogueQueue.Clear();
        foreach (var line in dialogue)
        {
            dialogueQueue.Enqueue(line);
        }
        ShowNextLine();
    }

    // 显示下一句
    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // 取出下一句
        var line = dialogueQueue.Dequeue();

        // 更新立绘和名字
        speakerNameText.text = line.speakerName;
        portrait.sprite = line.portrait;

        // 开始打字机效果
        StartCoroutine(TypeLine(line.text));
    }

    // 打字机效果协程
    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        // 逐字显示
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        // 如果是自动模式，等待后自动下一句
        if (autoMode)
        {
            autoCoroutine = StartCoroutine(AutoNext());
        }
    }

    // 自动模式下的延迟跳转
    IEnumerator AutoNext()
    {
        yield return new WaitForSeconds(autoDelay);
        ShowNextLine();
    }

    // 点击Next按钮的逻辑
    void OnNextClicked()
    {
        // 如果正在打字，直接跳过打字，显示完整文本
        if (isTyping)
        {
            StopAllCoroutines();
            var line = dialogueQueue.Peek();
            dialogueText.text = line.text;
            isTyping = false;

            // 如果是自动模式，停止自动协程，重新计时
            if (autoMode)
            {
                if (autoCoroutine != null)
                    StopCoroutine(autoCoroutine);
                autoCoroutine = StartCoroutine(AutoNext());
            }
        }
        // 打字结束，显示下一句
        else
        {
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);
            ShowNextLine();
        }
    }

    // 切换自动/手动模式
    void ToggleAutoMode(bool value)
    {
        autoMode = value;
        autoToggle.isOn = value;

        // 如果当前正在自动模式，切换为手动时停止自动协程
        if (!autoMode && autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
        }
    }

    // 对话结束，关闭界面
    void EndDialogue()
    {
        StopAllCoroutines();
        UIManager.Instance.Close();
    }
}