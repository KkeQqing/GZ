using UnityEngine;

[System.Serializable]
public class CodexData
{
    public string id;           // 唯一ID
    public string name;         // 名称
    public string description;  // 描述
    public Sprite icon;         // 图标
    public CodexType type;      // 类型：怪物/能力
    public bool unlocked;       // 是否解锁
}

public enum CodexType
{
    Monster,
    Ability
}