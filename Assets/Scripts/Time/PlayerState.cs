using UnityEngine;

[System.Serializable]
public struct PlayerState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity; // 对于 2D 游戏
    // 如果有其他需要回滚的状态（如血量、冷却时间），也可以加在这里
    // public float health; 
}