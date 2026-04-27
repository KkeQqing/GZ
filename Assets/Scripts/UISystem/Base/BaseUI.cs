using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public UIType uiType;

    public virtual void Init() { }

    public virtual void Show()
    {
        gameObject.SetActive(true); // 激活UI对象，使其可见
        OnShow(); // 调用子类的OnShow方法，执行特定UI的显示逻辑
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false); // 隐藏UI对象，使其不可见
        OnHide(); // 调用子类的OnHide方法，执行特定UI的隐藏逻辑
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}