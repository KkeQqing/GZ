using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingItem : MonoBehaviour
{
    [Header("组件引用")]
    public Image iconImage;      // 问号/结局图片的Image
    public TextMeshProUGUI titleText; // 下方的标题文本
    public Button itemButton;    // 整个格子的Button组件

    [Header("未解锁时的设置")]
    public Sprite questionMarkSprite; // 问号图
    public Color lockedColor = Color.gray; // 未解锁时的颜色
    public Color unlockedColor = Color.white; // 已解锁时的颜色

    private EndingData currentData; // 当前格子对应的结局数据

    // 初始化格子数据
    public void Init(EndingData data)
    {
        currentData = data;
        titleText.text = data.name;

        if (data.unlocked)
        {
            // 已解锁：显示结局图片，正常颜色
            iconImage.sprite = data.image;
            iconImage.color = unlockedColor;
        }
        else
        {
            // 未解锁：显示问号，灰色
            iconImage.sprite = questionMarkSprite;
            iconImage.color = lockedColor;
        }

        // 绑定点击事件
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnClick);
    }

    // 点击格子时触发
    private void OnClick()
    {
        if (currentData == null) return;

        if (currentData.unlocked)
        {
            // 已解锁：打开详情弹窗（我们后面做）
            EndingUI.Instance.ShowEndingDetail(currentData);
        }
        else
        {
            // 未解锁：可以提示“尚未解锁”
            Debug.Log("这个结局还没解锁！");
            // 你也可以在这里调用一个通用提示弹窗
        }
    }
}