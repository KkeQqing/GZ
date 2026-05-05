using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// debug模式下，Z 键扣血 20，X 键回血 10
/// 血条UI：不直接被外部调用，统一由 GameManager 访问
/// </summary>
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

    public static HUDUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPause);

        maxHP = 100;
        currentHP = maxHP;
    }

    protected override void OnShow()
    {
        base.OnShow();

        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
            hpCoroutine = null;
        }

        SetHPImmediate(currentHP, maxHP);
    }

    // DEBUG 快捷键
    private void Update()
    {
        if (!debugMode || !gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(20);
            Debug.Log("<color=orange>[DEBUG] Z 扣血 20</color>");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Heal(10);
            Debug.Log("<color=green>[DEBUG] X 回血 10</color>");
        }
    }

    // 这些方法不建议外部直接调用，统一走 GameManager
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

    public void ResetFullHP()
    {
        currentHP = maxHP;
        SetHPImmediate(currentHP, maxHP);
    }
}