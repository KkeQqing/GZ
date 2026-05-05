using Assets.Scripts.UISystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexUI : BaseUI
{
    public static CodexUI Instance;

    [Header("场景引用")]
    public Transform listParent;
    public CodexItem codexItemPrefab;
    public GameObject detailPanel;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDesc;
    public Image detailIcon;
    public Button monsterBtn;
    public Button abilityBtn;
    public Button backButton;

    [Header("数据")]
    public List<CodexData> allCodexData;

    private CodexType currentType = CodexType.Monster;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 绑定按钮事件
        if (monsterBtn != null) monsterBtn.onClick.AddListener(() => SwitchType(CodexType.Monster));
        if (abilityBtn != null) abilityBtn.onClick.AddListener(() => SwitchType(CodexType.Ability));
        if (backButton != null) backButton.onClick.AddListener(OnClose);

        if (debugMode) UnlockAllDebug();
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 打开界面时默认显示怪物列表
        SwitchType(CodexType.Monster);
    }

    // 切换怪物/能力（修复版，解决对象销毁报错）
    void SwitchType(CodexType type)
    {
        currentType = type;

        // 隐藏详情面板
        if (detailPanel != null) detailPanel.SetActive(false);

        // 清空旧条目（先移除事件再销毁，避免报错）
        foreach (Transform t in listParent)
        {
            CodexItem item = t.GetComponent<CodexItem>();
            if (item != null)
            {
                // 移除按钮事件，防止销毁后被引用
                if (item.btn != null)
                    item.btn.onClick.RemoveAllListeners();
            }
            Destroy(t.gameObject);
        }

        // 生成新条目
        foreach (var data in allCodexData)
        {
            if (data.type != currentType) continue;

            CodexItem newItem = Instantiate(codexItemPrefab, listParent);
            newItem.Init(data);
        }
    }

    // 显示详情（带空引用保护）
    public void ShowDetail(CodexData data)
    {
        if (data == null) return;

        if (detailPanel != null) detailPanel.SetActive(true);
        if (detailTitle != null) detailTitle.text = data.name;
        if (detailDesc != null) detailDesc.text = data.description;
        if (detailIcon != null) detailIcon.sprite = data.icon;

        Debug.Log("显示详情：" + data.name);
    }

    public void OnClose()
    {
        UIManager.Instance.Close();
        UIManager.Instance.Open(UIType.MainMenu);
    }

    // 调试：一键解锁所有
    void UnlockAllDebug()
    {
        foreach (var d in allCodexData)
            d.unlocked = true;
    }
}