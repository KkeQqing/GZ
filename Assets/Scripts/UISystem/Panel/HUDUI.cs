using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDUI : BaseUI
{
    [Header("HUD 引用")]
    public Image hpBar;
    public Button pauseButton;

    [Header("血条动画")]
    [Tooltip("血条变化速度，越大越快")]
    public float fillSpeed = 0.5f;

    private Coroutine hpCoroutine;
    private float currentHP;  // 当前血量
    private float maxHP;     // 最大血量
    private float targetFillAmount;

    protected override void Awake()
    {
        base.Awake();
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPause);
        }
    }

    // 打开界面时，初始化满血
    protected override void OnShow()
    {
        base.OnShow();
        maxHP = 100;       // 默认最大血量100
        currentHP = maxHP; // 初始满血
        SetHPImmediate(currentHP, maxHP);
    }

    // 受伤
    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage); // 防止负数
        UpdateHP(currentHP, maxHP);
    }

    // 回血
    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount); // 防止超过上限
        UpdateHP(currentHP, maxHP);
    }

    //  受伤测试（示例：扣30血）
    public void TestTakeDamage30()
    {
        TakeDamage(30);
    }

    // 回血测试（示例：回20血）
    public void TestHeal20()
    {
        Heal(20);
    }

    // 平滑更新血条
    public void UpdateHP(float current, float max)
    {
        if (hpBar == null || max <= 0) return;

        targetFillAmount = Mathf.Clamp01(current / max);

        if (gameObject.activeInHierarchy)
        {
            if (hpCoroutine != null)
                StopCoroutine(hpCoroutine);

            hpCoroutine = StartCoroutine(UpdateHPSmooth());
        }
        else
        {
            hpBar.fillAmount = targetFillAmount;
        }
    }

    IEnumerator UpdateHPSmooth()
    {
        while (!Mathf.Approximately(hpBar.fillAmount, targetFillAmount))
        {
            hpBar.fillAmount = Mathf.MoveTowards(
                hpBar.fillAmount,
                targetFillAmount,
                fillSpeed * Time.deltaTime
            );
            yield return null;
        }
        hpBar.fillAmount = targetFillAmount;
        hpCoroutine = null;
    }

    // 暂停按钮
    public void OnPause()
    {
        GameManager.Instance.TogglePause();
        UIManager.Instance.Open(UIType.Pause);
    }

    // 立即设置血量（无动画）
    public void SetHPImmediate(float current, float max)
    {
        if (hpBar == null || max <= 0) return;
        hpBar.fillAmount = Mathf.Clamp01(current / max);
    }
}