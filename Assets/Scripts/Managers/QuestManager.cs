using System;
using Imagination.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Imagination.Managers
{
    public enum QuestType
    {
        Fighting,
        Racing,
        Platformer
    }

    [Serializable]
    public class Quest
    {
        public string questId;
        public string title;
        public string description;
        public QuestType type;
        public int level;
        public bool isCompleted;
        public bool isUnlocked;
    }

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeQuests();
        }

        private void InitializeQuests()
        {
            allQuests.Clear();

            AddQuest("fighting_1", "Pertarungan Api", "Lawani pendudukan pertama di dunia pertarungan.", QuestType.Fighting, 1);
            AddQuest("fighting_2", "Pertempuran Pedang", "Tantang lawan kedua yang lebih kuat.", QuestType.Fighting, 2);
            AddQuest("fighting_3", "Boss Akhir", "Hadapi boss terakhir dari dunia pertarungan.", QuestType.Fighting, 3);
        }

        private void AddQuest(string id, string title, string desc, QuestType type, int level)
        {
            allQuests[id] = new Quest
            {
                questId = id,
                title = title,
                description = desc,
                type = type,
                level = level,
                isCompleted = false,
                isUnlocked = false
            };
        }

        public Quest GetQuest(string questId) => allQuests.ContainsKey(questId) ? allQuests[questId] : null;

        public Quest GetFirstAvailableQuest(QuestType type)
        {
            foreach (var q in allQuests.Values)
            {
                if (q.type == type && q.isUnlocked && !q.isCompleted)
                    return q;
            }
            return null;
        }

        public void UnlockQuest(string questId)
        {
            if (allQuests.ContainsKey(questId))
                allQuests[questId].isUnlocked = true;
        }

        public void CompleteQuest(string questId)
        {
            if (allQuests.ContainsKey(questId))
                allQuests[questId].isCompleted = true;
        }

        public bool IsQuestUnlocked(string questId)
        {
            if (!allQuests.ContainsKey(questId))
                return false;
            return allQuests[questId].isUnlocked;
        }

        public bool IsQuestCompleted(string questId)
        {
            if (!allQuests.ContainsKey(questId))
                return false;
            return allQuests[questId].isCompleted;
        }

        public void LoadFromSave(GameData saveData)
        {
            if (saveData == null) return;

            UnlockQuest("fighting_1");

            if (saveData.fightingLevel >= 1)
                CompleteQuest("fighting_1");

            if (saveData.fightingLevel >= 1)
                UnlockQuest("fighting_2");

            if (saveData.fightingLevel >= 2)
                CompleteQuest("fighting_2");

            if (saveData.fightingLevel >= 1)
                UnlockQuest("fighting_3");
        }
    }
}