using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<UIType, BaseUI> uiDict = new(); // 存储UI实例的字典，键为UI类型，值为对应的UI实例
    private Stack<BaseUI> uiStack = new(); // UI栈，管理当前打开的UI顺序，最新打开的UI在栈顶

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保UIManager在场景切换时不会被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁新的实例
        }
    }

    // 注册UI实例
    public void Register(BaseUI ui)
    {
        if (!uiDict.ContainsKey(ui.uiType))
        {
            uiDict.Add(ui.uiType, ui);
            ui.Init();
            ui.Hide();
        }
    }

    // 打开UI的方法，根据UI类型从字典中获取对应的UI实例，并将其显示在屏幕上，同时将其压入UI栈中
    public void Open(UIType type)
    {
        if (!uiDict.ContainsKey(type)) return;

        // 隐藏旧界面
        if (uiStack.Count > 0)
            uiStack.Peek().Hide();

        BaseUI ui = uiDict[type];
        ui.Show();
        uiStack.Push(ui);
    }

    // 关闭UI
    public void Close()
    {
        if (uiStack.Count == 0) return;

        BaseUI top = uiStack.Pop();
        top.Hide();

        if (uiStack.Count > 0)
            uiStack.Peek().Show();
    }

    // 安全获取UI实例
    public BaseUI GetUI(UIType type)
    {
        if (uiDict.TryGetValue(type, out var ui))
        {
            return ui;
        }
        return null;
    }

}