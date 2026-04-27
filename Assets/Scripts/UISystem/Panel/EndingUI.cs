using System.Collections.Generic;
using UnityEngine;

public class EndingUI : BaseUI
{
    public List<EndingData> endings;

    public void Refresh()
    {
        foreach (var e in endings)
        {
            if (e.unlocked)
            {
                Debug.Log("显示结局：" + e.name);
            }
            else
            {
                Debug.Log("未解锁");
            }
        }
    }

    public void OnClose()
    {
        UIManager.Instance.Close();
    }
}