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
}