using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : BaseUI
{
    public Text dialogueText;
    public Image portrait;

    private Queue<DialogueLine> lines = new();

    private bool autoMode = false;
    private Coroutine autoCoroutine;

    public void StartDialogue(List<DialogueLine> dialogue)
    {
        lines.Clear();

        foreach (var l in dialogue)
            lines.Enqueue(l);

        ShowNext();
    }

    public void ShowNext()
    {
        if (lines.Count == 0)
        {
            UIManager.Instance.Close();
            return;
        }

        var line = lines.Dequeue();

        dialogueText.text = line.text;
        portrait.sprite = line.portrait;

        if (autoMode)
        {
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);

            autoCoroutine = StartCoroutine(AutoNext());
        }
    }

    IEnumerator AutoNext()
    {
        yield return new WaitForSeconds(1f);
        ShowNext();
    }

    public void ToggleAuto(bool value)
    {
        autoMode = value;
    }
}