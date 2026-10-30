using UnityEngine;
using Imagination.Data;
using Imagination.Managers;

using Imagination.Core;
namespace Imagination.Managers
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        public int FightingLevel { get; private set; } = 0;
        public int RacingLevel { get; private set; } = 0;
        public int PlatformerLevel { get; private set; } = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResetProgression()
        {
            FightingLevel = 0;
            RacingLevel = 0;
            PlatformerLevel = 0;
        }

        public void LoadProgression(GameData saveData)
        {
            FightingLevel = saveData.fightingLevel;
            RacingLevel = saveData.racingLevel;
            PlatformerLevel = saveData.platformerLevel;

            QuestManager.Instance.LoadFromSave(saveData);
        }

        public void CompleteFightingLevel(int level)
        {
            if (level > FightingLevel)
                FightingLevel = level;

            string questId = $"fighting_{level}";
            QuestManager.Instance.CompleteQuest(questId);

            if (!QuestManager.Instance.IsQuestUnlocked($"fighting_{level + 1}"))
                QuestManager.Instance.UnlockQuest($"fighting_{level + 1}");

            var saveData = SaveManager.Instance.GetSaveData();
            saveData.fightingLevel = FightingLevel;
            SaveManager.Instance.SaveGame();
        }

        public bool IsFightingLevelUnlocked(int level)
        {
            return level <= FightingLevel || level == 1;
        }

        public int GetNextFightingLevel()
        {
            return FightingLevel + 1;
        }
    }
}