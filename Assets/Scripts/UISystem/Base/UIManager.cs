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

    // 注册UI实例的方法，通常在UI对象的Awake或Start方法中调用
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

        if (uiStack.Count > 0)
            uiStack.Peek().Hide();

        BaseUI ui = uiDict[type];
        ui.Show();
        uiStack.Push(ui);
    }

    // 关闭UI的方法，从UI栈中弹出当前打开的UI实例，并将其隐藏，同时如果栈中还有其他UI实例，则将其显示在屏幕上
    public void Close()
    {
        if (uiStack.Count == 0) return;

        BaseUI top = uiStack.Pop();
        top.Hide();

        if (uiStack.Count > 0)
            uiStack.Peek().Show();
    }

    // 新增：支持指定类型关闭
    public void Close(UIType type)
    {
        if (!uiDict.ContainsKey(type)) return;

        // 如果栈顶不是要关闭的那个，说明它不在最上面，逻辑上可能需要处理
        // 但为了简单，我们直接从栈里移除（这里逻辑较复杂，建议先用方案一）

        // 简单的做法：如果栈顶是它，就弹出
        if (uiStack.Count > 0 && uiStack.Peek().uiType == type)
        {
            BaseUI top = uiStack.Pop();
            top.Hide();

            // 恢复前一个
            if (uiStack.Count > 0)
                uiStack.Peek().Show();
        }
    }

}