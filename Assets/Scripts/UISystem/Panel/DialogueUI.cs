using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : BaseUI
{
    [Header("场景引用")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image portrait;
    public Button nextBtn;
    public Toggle autoToggle;

    [Header("设置")]
    public float typeSpeed = 0.05f;
    public float autoDelay = 1f;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping;
    private Coroutine autoWaitCoroutine; // 自动等待协程

    private void Awake()
    {
        nextBtn.onClick.AddListener(OnNextClicked);
        autoToggle.onValueChanged.AddListener(OnAutoToggleChanged);
    }

    protected override void OnShow()
    {
        base.OnShow();
        isTyping = false;
        dialogueText.text = "";
        speakerNameText.text = "";
        portrait.sprite = null;

        if (autoWaitCoroutine != null)
            StopCoroutine(autoWaitCoroutine);
        autoWaitCoroutine = null;
    }

    public void StartDialogue(List<DialogueLine> dialogue)
    {
        dialogueQueue.Clear();
        foreach (var line in dialogue)
            dialogueQueue.Enqueue(line);

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        var line = dialogueQueue.Dequeue();
        speakerNameText.text = line.speakerName;
        portrait.sprite = line.portrait;

        StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        // 打字结束 → 如果是自动模式，直接启动等待
        if (autoToggle.isOn)
            StartAutoWait();
    }

    // 自动等待后跳下一句
    void StartAutoWait()
    {
        if (autoWaitCoroutine != null)
            StopCoroutine(autoWaitCoroutine);

        autoWaitCoroutine = StartCoroutine(AutoWaitCoroutine());
    }

    IEnumerator AutoWaitCoroutine()
    {
        yield return new WaitForSeconds(autoDelay);
        ShowNextLine();
    }

    void OnNextClicked()
    {
        if (isTyping)
        {
            // 跳过打字
            StopAllCoroutines();
            isTyping = false;
            var currentLine = dialogueQueue.Peek();
            dialogueText.text = currentLine.text;

            // 跳完后如果是自动 → 立刻启动自动
            if (autoToggle.isOn)
                StartAutoWait();
        }
        else
        {
            // 正常下一句
            if (autoWaitCoroutine != null)
                StopCoroutine(autoWaitCoroutine);

            ShowNextLine();
        }
    }

    void OnAutoToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // 打开自动：
            // 如果当前句子已经播完 → 直接启动自动
            if (!isTyping)
                StartAutoWait();
        }
        else
        {
            // 关闭自动：停止等待
            if (autoWaitCoroutine != null)
            {
                StopCoroutine(autoWaitCoroutine);
                autoWaitCoroutine = null;
            }
        }
    }

    void EndDialogue()
    {
        if (autoWaitCoroutine != null)
            StopCoroutine(autoWaitCoroutine);

        UIManager.Instance.Close();
    }
}