// SkillManager.cs
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Animator animator;

    // 缓存引用
    private TimeControlManager timeManager;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        timeManager = FindObjectOfType<TimeControlManager>();

        if (timeManager == null)
        {
            Debug.LogWarning("场景中未找到 TimeControlManager，时间技能将无效。请创建一个空物体挂载 TimeControlManager 脚本。");
        }
    }

    void Update()
    {
        // 注意：Input 检测在 timeScale 变慢时，帧率会变低，但 Input.GetKeyDown 依然会在按键按下那一帧触发，
        // 不过由于游戏变慢，玩家感觉反应时间变长了，这符合设计预期。

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerSkill("AttackQ", SkillType.TimeSlow);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerSkill("AttackE", SkillType.TimeFast);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // 假设 R 是普通攻击或其他不涉及时间变化的技能
            TriggerSkill("AttackR", SkillType.Normal);
        }
    }

    enum SkillType { Normal, TimeSlow, TimeFast }

    void TriggerSkill(string skillName, SkillType type)
    {
        // 1. 处理时间逻辑
        if (type == SkillType.TimeSlow && timeManager != null)
        {
            timeManager.ActivateSlow();
        }
        else if (type == SkillType.TimeFast && timeManager != null)
        {
            timeManager.ActivateFastForward();
        }

        // 2. 处理动画逻辑
        if (animator != null)
        {
            // 检查状态是否存在，存在则播放，不存在则尝试 SetTrigger (取决于你的 Animator 设置)
            // 你的原代码使用了 Play，这里保持一致，但通常 SetTrigger 更适合状态机转换
            int stateHash = Animator.StringToHash(skillName);

            // 简单处理：直接播放片段，或者触发 Trigger
            // 如果 Animator 里配置的是 Trigger 参数：
            animator.SetTrigger(skillName);

            // 如果 Animator 里是独立的 State 且需要直接播放（覆盖当前）：
            // animator.Play(stateHash); 

            Debug.Log($"施放技能: {skillName}, 类型: {type}");
        }
    }
}