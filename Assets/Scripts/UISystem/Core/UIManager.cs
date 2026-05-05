using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<UIType, BaseUI> uiDict = new();
    private Stack<BaseUI> uiStack = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    public void Register(BaseUI ui)
    {
        if (!uiDict.ContainsKey(ui.uiType))
        {
            uiDict.Add(ui.uiType, ui);
            ui.Init();
            ui.Hide();
        }
    }

    public void Open(UIType type)
    {
        if (!uiDict.TryGetValue(type, out var ui)) return;

        // 栈顶相同就不重复打开
        if (uiStack.Count > 0 && uiStack.Peek() == ui)
            return;

        // 隐藏当前
        if (uiStack.Count > 0)
            uiStack.Peek().Hide();

        uiStack.Push(ui);
        ui.Show();
    }

    // 关闭顶层界面
    public void Close()
    {
        if (uiStack.Count == 0) return;

        var top = uiStack.Pop();
        top.Hide();

        // 显示前一层
        if (uiStack.Count > 0)
            uiStack.Peek().Show();
    }

    // 关闭所有，回到主界面专用
    public void CloseAll()
    {
        while (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            top.Hide();
        }
    }

    public BaseUI GetUI(UIType type)
    {
        uiDict.TryGetValue(type, out var ui);
        return ui;
    }
}