using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RewindManager : MonoBehaviour
{
    [Header("倒流设置")]
    public float rewindDuration = 3.0f;       // 倒流多少秒的数据
    public float rewindPlaybackSpeed = 6.0f;  // 回放速度倍数 (6倍速意味着3秒的数据在0.5秒内播完)

    [Header("组件引用")]
    public Rigidbody2D rb;
    public Animator animator;

    // 历史记录队列
    private Queue<PlayerState> history = new Queue<PlayerState>();

    // 状态标记
    private bool isRewinding = false;
    private bool isRecording = true;

    public bool IsRewinding => isRewinding;

    // 缓存用于回放的数据
    private List<PlayerState> playbackBuffer = new List<PlayerState>();

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // 如果正在倒流，不记录新数据，也不进行正常的物理移动（由倒流逻辑接管）
        if (isRewinding) return;

        if (isRecording && rb != null)
        {
            RecordState();
        }
    }

    /// <summary>
    /// 记录当前帧状态
    /// </summary>
    private void RecordState()
    {
        PlayerState state = new PlayerState
        {
            position = rb.position,
            rotation = Quaternion.Euler(0f, 0f, rb.rotation), // 修复：将 float 转为 Quaternion
            velocity = rb.velocity
        };

        history.Enqueue(state);

        // 维护队列大小：只保留 rewindDuration 秒的数据
        // 假设 FixedUpdate 约为 0.02s (50FPS)，计算需要保留的帧数
        // 更准确的方法是记录时间戳，但简单估算帧数通常足够
        int maxFrames = Mathf.CeilToInt(rewindDuration / Time.fixedDeltaTime);

        while (history.Count > maxFrames)
        {
            history.Dequeue(); // 移除最旧的数据
        }
    }

    /// <summary>
    /// 启动倒流技能 (由 SkillManager 调用)
    /// </summary>
    public void StartRewind()
    {
        if (isRewinding || history.Count < 10)
        {
            Debug.Log("无法倒流：正在倒流中或历史记录不足");
            return;
        }

        // 发布事件
        EventBus.publish(EventType.Time_Rewind_Start);
        Debug.Log("开始时间倒流!");

        StartCoroutine(RewindCoroutine());
    }

    private IEnumerator RewindCoroutine()
    {
        isRewinding = true;
        isRecording = false; // 停止记录新数据

        // 1. 将队列数据转入列表，方便索引访问
        playbackBuffer.Clear();
        playbackBuffer.AddRange(history);

        // 清空历史队列，防止倒流过程中产生脏数据（或者选择保留，看设计需求）
        // 这里选择清空，因为倒流后应该从那个时间点重新开始记录
        history.Clear();

        // 2. 快速回放逻辑
        // 我们从列表的末尾（最新）往开头（最旧）遍历
        // 为了视觉流畅，我们不每帧跳一帧，而是根据 playbackSpeed 插值或跳跃

        int totalFrames = playbackBuffer.Count;
        if (totalFrames == 0) yield break;

        float timer = 0f;
        float durationToRewind = rewindDuration / rewindPlaybackSpeed; // 实际消耗的真實时间

        // 记录开始时的状态，用于插值平滑（可选，直接跳跃更简单）
        // 这里采用直接跳跃帧的方式，配合高 FPS 看起来就是连贯的倒放

        int step = Mathf.Max(1, Mathf.CeilToInt(rewindPlaybackSpeed)); // 每次跳过几帧

        for (int i = totalFrames - 1; i >= 0; i -= step)
        {
            PlayerState targetState = playbackBuffer[i];

            // 应用状态
            rb.MovePosition(targetState.position);
            rb.MoveRotation(targetState.rotation);
            rb.velocity = targetState.velocity; // 恢复速度，保证倒流结束后惯性正确

            // 等待一小段时间，控制回放速度
            // 注意：这里不使用 Time.timeScale，因为我们希望倒流过程本身是实时的快速播放
            yield return new WaitForSeconds(Time.fixedDeltaTime / rewindPlaybackSpeed);

            // 可选：添加残影特效
            // SpawnAfterImage(targetState.position); 
        }

        // 确保最后停留在最早的那一帧
        if (playbackBuffer.Count > 0)
        {
            PlayerState finalState = playbackBuffer[0];
            rb.MovePosition(finalState.position);
            rb.MoveRotation(finalState.rotation);
            rb.velocity = finalState.velocity;
        }

        // 3. 倒流结束
        isRewinding = false;
        isRecording = true; // 恢复记录

        EventBus.publish(EventType.Time_Rewind_End);
        Debug.Log("时间倒流结束");

        // 清理缓冲
        playbackBuffer.Clear();
    }

    // 可选：如果在倒流期间玩家按了移动键，是否需要中断？
    // 通常倒流期间禁用玩家输入，由协程完全控制角色
}