using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("对话数据")]
    public string speakerName;  // 说话人名字
    public string text;         // 对话内容
    public Sprite portrait;     // 立绘图片
}