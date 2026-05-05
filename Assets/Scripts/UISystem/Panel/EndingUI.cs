using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.UISystem;

public class EndingUI : BaseUI
{
    public static EndingUI Instance;

    [Header("场景引用")]
    public List<EndingItem> endingItems; // 把场景里的4个EndingItem拖进来
    public EndingDetailPanel detailPanel; // 详情弹窗

    [Header("结局数据")]
    public List<EndingData> endings; // 在Inspector里配置4个结局数据

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 打开界面时自动刷新
    protected override void OnShow()
    {
        base.OnShow();
        Refresh();
    }

    // 刷新所有格子的状态
    public void Refresh()
    {
        // 先检查endingItems和endings是否为空
        if (endingItems == null || endings == null)
        {
            Debug.LogError("EndingUI: endingItems 或 endings 列表为空！请在Inspector赋值！");
            return;
        }

        Debug.Log($"[EndingUI] 开始刷新，共 {endings.Count} 个结局");

        for (int i = 0; i < endingItems.Count; i++)
        {
            // 先检查格子本身是否为空
            if (endingItems[i] == null)
            {
                Debug.LogWarning($"EndingUI: 第{i}个EndingItem为空！请检查场景赋值！");
                continue;
            }

            if (i < endings.Count)
            {
                EndingData data = endings[i];
                if (data == null)
                {
                    Debug.LogWarning($"EndingUI: 第{i}个EndingData为空！请检查Inspector赋值！");
                    endingItems[i].gameObject.SetActive(false);
                    continue;
                }

                // 安全读取解锁状态（如果SaveManager没初始化，就用Inspector的初始值）
                bool isUnlocked = data.unlocked;
                if (SaveManager.Instance != null)
                {
                    isUnlocked = SaveManager.Instance.IsEndingUnlocked(data.id);
                    Debug.Log($"[EndingUI] 结局 {data.name} (id={data.id})：存档状态={isUnlocked}");
                }
                else
                {
                    Debug.LogWarning("SaveManager.Instance 未初始化，使用初始解锁状态！");
                }

                // 更新数据并初始化格子
                data.unlocked = isUnlocked;
                endingItems[i].Init(data);
                endingItems[i].gameObject.SetActive(true);
            }
            else
            {
                // 隐藏多余的格子
                endingItems[i].gameObject.SetActive(false);
            }
        }
    }

    // 显示结局详情弹窗
    public void ShowEndingDetail(EndingData data)
    {
        if (detailPanel != null)
        {
            detailPanel.Show(data);
        }
        else
        {
            Debug.LogWarning("EndingDetailPanel 未赋值！无法显示详情！");
        }
    }

    // 关闭按钮点击事件
    public void OnClose()
    {
        UIManager.Instance.Close();
    }
}