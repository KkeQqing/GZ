using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("组件引用")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("移动设置")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;

    // 缓存全局倒流管理器引用
    private GlobalRewindManager globalRewindManager;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        if (rb != null) rb.freezeRotation = true;

        // 获取全局倒流管理器单例
        globalRewindManager = GlobalRewindManager.Instance;

        if (globalRewindManager == null)
        {
            Debug.LogWarning("未在场景中找到 GlobalRewindManager！时间倒流功能将无效。请创建一个空物体并挂载该脚本。");
        }
    }

    void Update()
    {
        // 优先处理技能触发 (无论是否正在倒流，或者仅在非倒流时触发)
        // 注意：通常倒流期间不能再次开启倒流，所以加一个 !isRewinding 判断
        bool canUseSkill = (globalRewindManager == null || !globalRewindManager.isRewinding);

        if (canUseSkill)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("检测到 Q 键"); // 调试用
                if (animator) animator.SetTrigger("AttackQ");
                // 调用 SkillManager 的逻辑...
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("检测到 E 键");
                if (animator) animator.SetTrigger("AttackE");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("检测到 R 键 - 准备倒流"); // <--- 如果这里没打印，说明输入没被捕获
                if (animator) animator.SetTrigger("AttackR");
                // 关键：确保 SkillManager 被调用
                SkillManager skillMgr = GetComponent<SkillManager>();
                if (skillMgr != null)
                {
                    // 如果 SkillManager 里有逻辑，让它处理
                    // 或者直接调用全局管理器
                    if (GlobalRewindManager.Instance != null)
                    {
                        GlobalRewindManager.Instance.StartGlobalRewind();
                    }
                }
                return; // 处理完 R 键后可以暂时返回，避免后续逻辑干扰
            }
        }

        // 【修改点 1】检查全局倒流状态
        // 如果正在倒流，忽略玩家输入，防止玩家在怪物回溯时乱跑（可选设计，也可允许玩家移动）
        if (globalRewindManager != null && globalRewindManager.isRewinding)
        {
            moveInput = Vector2.zero;
        }

        // 获取原始输入 (-1, 0, 1)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(h, v).normalized;

        // 更新动画参数
        if (animator != null)
        {
            // 将速度大小传给 Animator
            animator.SetFloat("Speed", moveInput.magnitude);

            // 【重要】抵消 Time.timeScale 对玩家动画的影响
            // 让世界变慢/快，但玩家动作保持正常速度
            if (Time.timeScale > 0.001f)
            {
                animator.speed = 1f / Time.timeScale;
            }
            else
            {
                animator.speed = 1f;
            }

            // 触发技能动画 (逻辑在 SkillManager 中处理，这里仅做动画触发备用)
            if (Input.GetKeyDown(KeyCode.Q)) animator.SetTrigger("AttackQ");
            if (Input.GetKeyDown(KeyCode.E)) animator.SetTrigger("AttackE");
            if (Input.GetKeyDown(KeyCode.R)) animator.SetTrigger("AttackR");
        }
    }

    void FixedUpdate()
    {
        // 如果正在全局倒流，且设计意图是玩家定格，则跳过物理移动
        if (globalRewindManager != null && globalRewindManager.isRewinding)
        {
            // 如果希望玩家在倒流期间也能移动，请注释掉整个 if 块
            return;
        }

        if (rb != null)
        {
            // 【核心逻辑】抵消 Time.timeScale 对玩家移动速度的影响
            float timeScaleFactor = Time.timeScale > 0.001f ? Time.timeScale : 1f;

            // 公式：位移 = 输入 * 速度 * (固定时间步长 / 时间缩放)
            // 这样无论世界快慢，玩家的真实移动速度保持不变
            Vector2 movement = moveInput * moveSpeed * (Time.fixedDeltaTime / timeScaleFactor);

            Vector2 targetPosition = rb.position + movement;

            // 执行移动
            rb.MovePosition(targetPosition);
        }
    }
}