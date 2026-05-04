using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单条对话行
/// </summary>
[System.Serializable]
public class DialogueLine
{
    public string charId;        // 角色唯一ID
    public string speakerName;   // 显示名字
    public string text;         // 对话文本
    public string portraitPath; // 立绘 Resources 路径
}

/// <summary>
/// 单个章节对话整体
/// </summary>
[System.Serializable]
public class DialogueChapter
{
    public string chapterName;
    public List<DialogueLine> lines;
}