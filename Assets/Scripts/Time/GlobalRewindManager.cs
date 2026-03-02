using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class GlobalRewindManager : MonoBehaviour
{
    public static GlobalRewindManager Instance { get; private set; }

    [Header("倒流设置")]
    public float rewindDuration = 3.0f;       // 倒流时长
    public float rewindSpeedMultiplier = 5.0f; // 倒流播放速度 (5倍速)

    [Header("调试")]
    public bool isRewinding = false;

    private List<RewindableEntity> allEntities = new List<RewindableEntity>();
    private Coroutine rewindCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初始化时查找所有实体，或者动态添加
        RefreshEntities();
    }

    // 如果有物体动态生成，需要调用此方法注册
    public void RegisterEntity(RewindableEntity entity)
    {
        if (!allEntities.Contains(entity))
            allEntities.Add(entity);
    }

    public void UnregisterEntity(RewindableEntity entity)
    {
        allEntities.Remove(entity);
    }

    void RefreshEntities()
    {
        allEntities = FindObjectsOfType<RewindableEntity>().ToList();
    }

    // 每帧记录所有实体的状态
    // 注意：放在 FixedUpdate 中以匹配物理更新频率
    void FixedUpdate()
    {
        if (isRewinding) return; // 倒流期间停止记录

        // 动态刷新一下列表，防止有新生成的怪物没被记录
        // 实际项目中建议用事件注册/注销代替每帧 FindObjects
        if (allEntities.Count == 0) RefreshEntities();

        foreach (var entity in allEntities)
        {
            if (entity != null)
                entity.Record();
        }
    }

    /// <summary>
    /// 启动全局倒流 (除玩家外)
    /// </summary>
    public void StartGlobalRewind()
    {
        if (isRewinding)
        {
            Debug.Log("无法倒流：正在倒流中");
            return;
        }

        if (allEntities.Count == 0)
        {
            Debug.Log("无法倒流：无可回溯物体");
            return;
        }

        // 1. 先计算一下深度，看看有多少数据
        int minDepth = GetMinHistoryDepth();
        int totalFramesNeeded = Mathf.CeilToInt(rewindDuration / Time.fixedDeltaTime);

        Debug.Log($"[倒流启动] 涉及物体数：{allEntities.Count}");
        Debug.Log($"[倒流启动] 需要回溯帧数：{totalFramesNeeded} (约 {rewindDuration}秒)");
        Debug.Log($"[倒流启动] 实际可用最小历史帧数：{minDepth}");

        if (minDepth <= 0)
        {
            Debug.LogWarning("[倒流启动] 历史记录为 0，无法倒流！请确保物体已运动一段时间。");
            return;
        }

        Debug.Log($"开始全局倒流！");
        EventBus.publish(EventType.Time_Rewind_Start);

        isRewinding = true;

        if (rewindCoroutine != null) StopCoroutine(rewindCoroutine);
        rewindCoroutine = StartCoroutine(RewindProcess(minDepth)); // 把深度传进去
    }

    private IEnumerator RewindProcess(int availableFrames)
    {
        Debug.Log($"[协程开始] 可用帧数：{availableFrames}");

        // 1. 【关键修改】倒流开始前：禁用所有实体的 Animator
        foreach (var entity in allEntities)
        {
            if (entity != null)
                entity.SetRewindMode(true); // 进入倒流模式 (禁用 Animator)
        }

        int step = Mathf.Max(1, Mathf.CeilToInt(rewindSpeedMultiplier));
        int currentOffset = 0;

        // 限制最大回缩量，防止越界
        int maxOffset = availableFrames;

        Debug.Log($"[协程逻辑] 将从 offset 0 回退到 {maxOffset}, 步长 {step}");

        if (maxOffset <= 0)
        {
            Debug.LogError("[协程错误] maxOffset <= 0, 退出协程");
            isRewinding = false;
            yield break;
        }

        float waitTime = Time.fixedDeltaTime / rewindSpeedMultiplier;
        int loopCount = 0;

        // --- 核心循环 ---
        while (currentOffset < maxOffset)
        {
            loopCount++;
            // Debug.Log($"[倒流循环] 第 {loopCount} 次，当前 offset: {currentOffset}");

            foreach (var entity in allEntities)
            {
                if (entity == null) continue;

                EntityState state;
                if (entity.GetStateAtOffset(currentOffset, out state))
                {
                    entity.ApplyState(state);
                }
                else
                {
                    // 如果拿不到状态，说明 offset 超出了该物体的记录范围
                    // Debug.LogWarning($"[倒流警告] {entity.name} 在 offset {currentOffset} 处无数据");
                }
            }

            currentOffset += step;

            // 【关键修改】使用 Realtime 等待，不受 Time.timeScale 影响
            yield return new WaitForSecondsRealtime(waitTime);
        }
        // --------------

        Debug.Log($"[协程结束] 循环执行了 {loopCount} 次，最终 offset: {currentOffset}");

        // 确保最后停在最远的那一帧
        if (maxOffset > 0)
        {
            foreach (var entity in allEntities)
            {
                if (entity == null) continue;
                EntityState state;
                // 取最后一帧
                if (entity.GetStateAtOffset(maxOffset - 1, out state))
                {
                    entity.ApplyState(state);
                }
            }
        }

        // 2. 【关键修改】倒流结束后：恢复所有实体的 Animator
        foreach (var entity in allEntities)
        {
            if (entity != null)
                entity.SetRewindMode(false); // 退出倒流模式 (启用 Animator)
        }

        isRewinding = false;
        Debug.Log("全局倒流结束");
        EventBus.publish(EventType.Time_Rewind_End);
    }
    // 【缺失的方法】获取所有实体中最短的历史深度，防止访问越界
    private int GetMinHistoryDepth()
    {
        int minDepth = int.MaxValue;

        foreach (var entity in allEntities)
        {
            if (entity == null) continue;

            // 调用 RewindableEntity 中的公开方法
            int count = entity.GetHistoryCount();
            minDepth = Mathf.Min(minDepth, count);
        }

        // 如果没有找到任何实体，返回 0
        return minDepth == int.MaxValue ? 0 : minDepth;
    }
}