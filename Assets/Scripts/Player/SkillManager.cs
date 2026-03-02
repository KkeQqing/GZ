// SkillManager.cs
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Animator animator;
    public GlobalRewindManager globalRewindManager; // 拖入场景中的 Manager

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
            if (globalRewindManager != null)
            {
                // 播放玩家自己的倒流动画（如果有），比如做一个双手划时间的动作
                if (animator) animator.SetTrigger("AttackR");

                // 触发全局倒流
                globalRewindManager.StartGlobalRewind();

                Debug.Log("发动技能：世界回溯！");
            }
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