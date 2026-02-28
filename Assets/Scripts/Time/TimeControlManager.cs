using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlManager : MonoBehaviour
{
    public static TimeControlManager Instance { get; private set; }

    [Header("时间设置")]
    [Range(0.1f, 1.0f)] public float slowScale = 0.5f;      // 减速比例
    [Range(1.1f, 3.0f)] public float fastScale = 2.0f;      // 加速比例
    public float defaultScale = 1.0f;

    [Header("技能持续时间 (秒，基于真实时间)")]
    public float skillDuration = 3.0f;

    private bool isSkillActive = false;
    private Coroutine currentSkillCoroutine = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保场景切换不销毁（如果需要跨场景保持可加上 DontDestroyOnLoad）
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 开启时间减速
    /// </summary>
    public void ActivateSlow()
    {
        if (isSkillActive) return; // 防止重叠

        StartSkillEffect(slowScale, EventType.Time_Slow_Start, EventType.Time_Slow_End);
    }

    /// <summary>
    /// 开启时间加速
    /// </summary>
    public void ActivateFastForward()
    {
        if (isSkillActive) return;

        StartSkillEffect(fastScale, EventType.Time_FastForward_Start, EventType.Time_FastForward_End);
    }

    /// <summary>
    /// 通用技能逻辑处理
    /// </summary>
    private void StartSkillEffect(float targetScale, EventType startEvent, EventType endEvent)
    {
        isSkillActive = true;

        // 发布开始事件
        EventBus.publish(startEvent);
        Debug.Log($"时间技能开始: {targetScale}x");

        // 应用时间缩放
        Time.timeScale = targetScale;

        // 启动协程处理持续时间
        // 注意： WaitForSeconds 受 timeScale 影响，所以我们需要用 WaitForSecondsRealtime 或者手动计算
        if (currentSkillCoroutine != null) StopCoroutine(currentSkillCoroutine);
        currentSkillCoroutine = StartCoroutine(SkillDurationRoutine(targetScale, endEvent));
    }

    private IEnumerator SkillDurationRoutine(float currentScale, EventType endEvent)
    {
        // 使用真实时间等待，不受 timeScale 影响，保证技能时长固定
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < skillDuration)
        {
            yield return null;
        }

        // 技能结束，恢复时间
        ResetTime();
        EventBus.publish(endEvent);
        Debug.Log("时间技能结束，恢复正常流速");

        isSkillActive = false;
        currentSkillCoroutine = null;
    }

    /// <summary>
    /// 强制重置时间为正常
    /// </summary>
    public void ResetTime()
    {
        Time.timeScale = defaultScale;
        // 如果此时有正在运行的协程，也可以在这里选择是否强制中断，
        // 但通常让上面的协程自然走完发布 End 事件更好。
        // 如果是被新技能打断，需要在调用此方法前处理逻辑。
    }

    // 可选：提供手动立即停止的方法（例如被眩晕打断）
    public void ForceStop()
    {
        if (currentSkillCoroutine != null) StopCoroutine(currentSkillCoroutine);
        ResetTime();
        isSkillActive = false;
        // 这里可以根据需要发布特定的中断事件，或者复用 End 事件
    }
}
