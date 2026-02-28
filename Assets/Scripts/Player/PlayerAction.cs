using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("组件引用")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("移动设置")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        // 获取倒流管理器状态 (可以通过静态单例或 GetComponent 获取)
        RewindManager rewindMgr = GetComponent<RewindManager>();

        // 如果正在倒流，忽略玩家输入
        if (rewindMgr != null && IsRewinding(rewindMgr))
        {
            moveInput = Vector2.zero;
            // 注意：动画可能还需要播放倒流相关的动画，由 SkillManager 或 RewindManager 控制
            return;
        }

        // 获取原始输入 (-1, 0, 1)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(h, v).normalized;

        // 更新动画参数
        if (animator != null)
        {
            // 将速度大小传给 Animator (0 或 1)
            animator.SetFloat("Speed", moveInput.magnitude);

            if (Time.timeScale > 0.001f)
            {
                animator.speed = 1f / Time.timeScale;
            }
            else
            {
                animator.speed = 1f; // 防止除以 0
            }

            if (Input.GetKeyDown(KeyCode.Q)) animator.SetTrigger("AttackQ");
            if (Input.GetKeyDown(KeyCode.E)) animator.SetTrigger("AttackE");
            if (Input.GetKeyDown(KeyCode.R)) animator.SetTrigger("AttackR");
        }
    }

    // 2. FixedUpdate: 只负责物理移动 (固定时间间隔执行，与帧率无关)
    void FixedUpdate()
    {
        if (rb != null)
        {
            float timeScaleFactor = Time.timeScale > 0.001f ? Time.timeScale : 1f;

            Vector2 movement = moveInput * moveSpeed * (Time.fixedDeltaTime / timeScaleFactor);

            Vector2 targetPosition = rb.position + movement;

            // 执行移动
            rb.MovePosition(targetPosition);

        }
    }

    bool IsRewinding(RewindManager mgr)
    {
        // 由于字段是 private，建议在 RewindManager 中添加: public bool IsRewinding => isRewinding;
        // 这里假设你已经添加了该属性
        return false; // 占位，请实际调用 mgr.IsRewinding
    }
}