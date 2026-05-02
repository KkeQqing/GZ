using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 当达成某个结局的地方（比如游戏结束场景），调用：SaveManager.Instance.UnlockEnding("ending_01"); // 对应EndingData里的id

namespace Assets.Scripts.UISystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;

        private const string SAVE_KEY = "EndingUnlocks";

        // 已解锁结局的ID列表
        public List<string> unlockedEndingIds = new List<string>();

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

        // 解锁一个结局
        public void UnlockEnding(string endingId)
        {
            if (!unlockedEndingIds.Contains(endingId))
            {
                unlockedEndingIds.Add(endingId);
                Save();
            }
        }

        // 检查结局是否已解锁
        public bool IsEndingUnlocked(string endingId)
        {
            return unlockedEndingIds.Contains(endingId);
        }

        // 保存到PlayerPrefs
        public void Save()
        {
            string json = JsonUtility.ToJson(new SaveData { unlockedIds = unlockedEndingIds });
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
                unlockedEndingIds = data.unlockedIds ?? new List<string>();
            }
        }

        public List<string> unlockedCodexIds = new List<string>();

        // 解锁一个图鉴
        public void UnlockCodex(string id)
        {
            if (!unlockedCodexIds.Contains(id))
            {
                unlockedCodexIds.Add(id);
                Save();
            }
        }

        public bool IsCodexUnlocked(string id)
        {
            return unlockedCodexIds.Contains(id);
        }

        // 序列化用的结构
        [Serializable]
        private class SaveData
        {
            public List<string> unlockedIds;
        }
    }
}