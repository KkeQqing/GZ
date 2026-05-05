using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 解锁任一结局调用：SaveManager.Instance.UnlockEnding("1");1是结局1的ID，必须和你在Inspector里配置的EndingData的id完全一样！
/// debugMode：启用后会在控制台输出更多调试信息，并且在游戏中按 L 键可以快速解锁结局1，按 K 键可以清空所有存档数据，方便测试。
/// </summary>

namespace Assets.Scripts.UISystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;

        private const string SAVE_KEY = "EndingUnlocks";

        public bool debugMode = true;

        // 已解锁结局的ID列表
        public List<string> unlockedEndingIds = new List<string>();
        // 已解锁图鉴ID列表
        public List<string> unlockedCodexIds = new List<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Load();
        }

        // ====================== 测试方法：Debug 模式解锁结局1 ======================
        private void Update()
        {
            // 按 L 键 解锁结局1（对应你数据里的id "1"）
            if (debugMode && Input.GetKeyDown(KeyCode.L))
            {
                TestUnlockEnding1();
            }

            // 按 K 键 清空所有存档
            if (debugMode && Input.GetKeyDown(KeyCode.K))
            {
                ClearAllSave();
            }
        }

        // 测试：解锁结局1（和你EndingData里的id "1" 对应）
        public void TestUnlockEnding1()
        {
            // 这里改成和你Inspector里完全一样的ID！
            UnlockEnding("1");
            Debug.Log("<color=green>[测试] 已解锁结局：id=1</color>");
            Debug.Log($"当前已解锁结局数量：{unlockedEndingIds.Count}");

            // 关键：解锁后立刻刷新EndingUI界面
            if (UIManager.Instance != null)
            {
                EndingUI endingUI = UIManager.Instance.GetUI(UIType.Ending) as EndingUI;
                if (endingUI != null)
                {
                    endingUI.Refresh();
                    Debug.Log("<color=cyan>[测试] 已刷新结局界面！</color>");
                }
            }
        }

        // 测试：清空所有存档
        public void ClearAllSave()
        {
            unlockedEndingIds.Clear();
            unlockedCodexIds.Clear();
            Save();
            Debug.Log("<color=yellow>[测试] 已清空所有存档数据！</color>");

            // 清空后也刷新一下界面
            if (UIManager.Instance != null)
            {
                EndingUI endingUI = UIManager.Instance.GetUI(UIType.Ending) as EndingUI;
                if (endingUI != null)
                {
                    endingUI.Refresh();
                }
            }
        }
        // ======================================================================

        // 解锁一个结局
        public void UnlockEnding(string endingId)
        {
            if (!unlockedEndingIds.Contains(endingId))
            {
                unlockedEndingIds.Add(endingId);
                Save();

                if (debugMode)
                    Debug.Log($"解锁结局：{endingId}");
            }
        }

        // 检查结局是否已解锁
        public bool IsEndingUnlocked(string endingId)
        {
            return unlockedEndingIds.Contains(endingId);
        }

        // 解锁一个图鉴
        public void UnlockCodex(string id)
        {
            if (!unlockedCodexIds.Contains(id))
            {
                unlockedCodexIds.Add(id);
                Save();

                if (debugMode)
                    Debug.Log($"解锁图鉴：{id}");
            }
        }

        public bool IsCodexUnlocked(string id)
        {
            return unlockedCodexIds.Contains(id);
        }

        // 保存到PlayerPrefs
        public void Save()
        {
            SaveData data = new SaveData
            {
                unlockedEndingIds = this.unlockedEndingIds,
                unlockedCodexIds = this.unlockedCodexIds
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        // 从PlayerPrefs读取
        public void Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data != null)
                {
                    unlockedEndingIds = data.unlockedEndingIds ?? new List<string>();
                    unlockedCodexIds = data.unlockedCodexIds ?? new List<string>();
                }
            }
        }

        // 序列化用的结构（同时保存结局 + 图鉴）
        [Serializable]
        private class SaveData
        {
            public List<string> unlockedEndingIds;
            public List<string> unlockedCodexIds;
        }
    }
}