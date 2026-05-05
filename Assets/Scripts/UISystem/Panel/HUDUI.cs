using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDUI : BaseUI
{
    [Header("HUD 引用")]
    public Image hpBar;
    public Button pauseButton;

    [Header("血条动画")]
    public float fillSpeed = 0.5f;

    private Coroutine hpCoroutine;
    private float currentHP;
    private float maxHP;
    private float targetFillAmount;

    // 单例防止重复
    public static HUDUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPause);

        // 只初始化一次
        maxHP = 100;
        currentHP = maxHP;
    }

    protected override void OnShow()
    {
        base.OnShow();

        // 防止协程残留
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
            hpCoroutine = null;
        }

        SetHPImmediate(currentHP, maxHP);
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        UpdateHP(currentHP, maxHP);
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UpdateHP(currentHP, maxHP);
    }

    public void UpdateHP(float current, float max)
    {
        if (hpBar == null || max <= 0) return;

        targetFillAmount = Mathf.Clamp01(current / max);

        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine);

        if (gameObject.activeInHierarchy)
            hpCoroutine = StartCoroutine(UpdateHPSmooth());
        else
            hpBar.fillAmount = targetFillAmount;
    }

    IEnumerator UpdateHPSmooth()
    {
        while (!Mathf.Approximately(hpBar.fillAmount, targetFillAmount))
        {
            hpBar.fillAmount = Mathf.MoveTowards(
                hpBar.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);
            yield return null;
        }
        hpBar.fillAmount = targetFillAmount;
        hpCoroutine = null;
    }

    public void OnPause()
    {
        UIManager.Instance.Open(UIType.Pause);
        GameManager.Instance.TogglePause();
    }

    public void SetHPImmediate(float current, float max)
    {
        if (hpBar == null || max <= 0) return;
        hpBar.fillAmount = Mathf.Clamp01(current / max);
    }

    // 重新开始游戏时重置血量
    public void ResetFullHP()
    {
        currentHP = maxHP;
        SetHPImmediate(currentHP, maxHP);
    }
}