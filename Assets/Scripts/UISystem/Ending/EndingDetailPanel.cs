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
        closeButton.onClick.AddListener(Hide);
    }

    public void Show(EndingData data)
    {
        detailImage.sprite = data.image;
        detailTitle.text = data.name;
        detailDescription.text = data.description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}