using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexItem : MonoBehaviour
{
    [Header("必须在Inspector里拖好！")]
    public TextMeshProUGUI text;
    public Button btn;

    private CodexData data;

    // 初始化条目
    public void Init(CodexData d)
    {
        data = d;

        // 安全赋值文本
        if (text != null)
        {
            text.text = d != null && d.unlocked ? d.name : "???";
        }

        // 绑定按钮事件（先移除旧事件，防止重复绑定）
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnItemClick);
        }
    }

    // 点击事件
    void OnItemClick()
    {
        if (data == null || !data.unlocked) return;

        if (CodexUI.Instance != null)
        {
            CodexUI.Instance.ShowDetail(data);
        }
    }
}