using System.Collections.Generic;
using UnityEngine;

public class CodexUI : BaseUI
{
    public List<CodexData> list;

    public void ShowDetail(CodexData data)
    {
        Debug.Log(data.name + ":" + data.description);
    }

    public void OnClose()
    {
        UIManager.Instance.Close();
    }
}