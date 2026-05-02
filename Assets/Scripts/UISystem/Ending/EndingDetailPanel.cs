using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingDetailPanel : MonoBehaviour
{
    [Header("组件引用")]
    public Image detailImage;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDescription;
    public Button closeButton;

    private void Awake()
    {
        // 默认隐藏
        gameObject.SetActive(false);

        // 安全绑定关闭按钮
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        else
        {
            Debug.LogWarning("EndingDetailPanel: closeButton 未赋值！请在Inspector拖入！");
        }
    }

    public void Show(EndingData data)
    {
        // 先检查data是否为空
        if (data == null)
        {
            Debug.LogError("EndingDetailPanel: 传入的EndingData为空！");
            return;
        }

        // 逐个检查引用，防止空引用报错
        if (detailImage != null)
        {
            detailImage.sprite = data.image;
        }
        else
        {
            Debug.LogError("EndingDetailPanel: detailImage 未赋值！请拖入详情图片！");
        }

        if (detailTitle != null)
        {
            detailTitle.text = data.name;
        }
        else
        {
            Debug.LogError("EndingDetailPanel: detailTitle 未赋值！请拖入标题文本！");
        }

        if (detailDescription != null)
        {
            detailDescription.text = data.description;
        }
        else
        {
            Debug.LogError("EndingDetailPanel: detailDescription 未赋值！请拖入描述文本！");
        }

        // 显示面板
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}