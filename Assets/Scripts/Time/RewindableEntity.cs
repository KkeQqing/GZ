using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EntityState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}

public class RewindableEntity : MonoBehaviour
{
    [Header("设置")]
    public bool restoreVelocity = true;
    public int maxFrames = 150; // 3 秒 @ 50FPS

    private Rigidbody rb;
    private Rigidbody2D rb2d;
    private Animator animator; // 缓存 Animator 引用

    // 【关键修改】使用 List 代替 Queue，支持直接索引访问，性能更高
    private List<EntityState> history = new List<EntityState>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // 获取 Animator
    }

    public int GetHistoryCount() => history.Count;

    public void Record()
    {
        EntityState state = new EntityState
        {
            position = transform.position,
            rotation = transform.rotation
        };

        if (restoreVelocity)
        {
            if (rb != null)
            {
                state.velocity = rb.velocity;
                state.angularVelocity = rb.angularVelocity;
            }
            else if (rb2d != null)
            {
                state.velocity = new Vector3(rb2d.velocity.x, rb2d.velocity.y, 0);
                state.angularVelocity = new Vector3(0, 0, rb2d.angularVelocity);
            }
        }

        history.Add(state); // 直接添加到末尾

        // 如果超过最大帧数，移除最旧的（列表头部）
        if (history.Count > maxFrames)
        {
            history.RemoveAt(0);
        }
    }

    // offset: 0 = 最新 (列表末尾), offset = Count-1 = 最旧 (列表开头)
    public bool GetStateAtOffset(int offset, out EntityState state)
    {
        state = default;

        if (history.Count == 0) return false;

        // 计算索引：列表长度 - 1 - 偏移量
        int index = history.Count - 1 - offset;

        if (index >= 0 && index < history.Count)
        {
            state = history[index];
            return true;
        }

        return false;
    }

    public void ApplyState(EntityState state)
    {
        // 1. 强制设置 Transform
        transform.position = state.position;
        transform.rotation = state.rotation;

        // 2. 如果有刚体，必须同步刚体状态，否则物理引擎会把位置拉回去
        if (restoreVelocity)
        {
            if (rb != null)
            {
                rb.velocity = state.velocity;
                rb.angularVelocity = state.angularVelocity;

                // 【关键】强制刚体位置与 Transform 同步，防止插值干扰
                rb.MovePosition(state.position);
                rb.WakeUp(); // 唤醒刚体
            }
            else if (rb2d != null)
            {
                rb2d.velocity = new Vector2(state.velocity.x, state.velocity.y);
                rb2d.angularVelocity = state.angularVelocity.z;

                // 【关键】2D 下使用 MovePosition 强制更新
                rb2d.MovePosition(state.position);
                rb2d.WakeUp();
            }
        }
    }

    // 【新增方法】用于在倒流开始/结束时控制 Animator
    public void SetRewindMode(bool isRewinding)
    {
        if (animator != null)
        {
            // 方案 A: 完全禁用 Animator (最稳妥，彻底消除冲突)
            animator.enabled = !isRewinding;

            // 方案 B (可选): 如果不想完全禁用，可以只禁用 Apply Root Motion
            // animator.applyRootMotion = !isRewinding; 
            // 但通常方案 A 效果最好，因为倒流时不需要播放新动画
        }
    }

    public void ClearHistory()
    {
        history.Clear();
    }
}