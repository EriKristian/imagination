using System.IO;
using Imagination.Data;
using UnityEngine;

namespace Imagination.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private GameData currentSaveData;
        private string saveFilePath;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
            LoadSaveData();
        }

        public GameData GetSaveData()
        {
            if (currentSaveData == null)
                currentSaveData = new GameData();
            return currentSaveData;
        }

        public void SaveGame()
        {
            string json = JsonUtility.ToJson(currentSaveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"[SaveManager] Game saved to {saveFilePath}");
        }

        public void LoadSaveData()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                currentSaveData = JsonUtility.FromJson<GameData>(json);
                Debug.Log("[SaveManager] Save data loaded.");
            }
            else
            {
                ResetSaveData();
                Debug.Log("[SaveManager] No save file found. Created new save data.");
            }
        }

        public void ResetSaveData()
        {
            currentSaveData = new GameData
            {
                hasPlayed = false,
                unlockedFighting = false,
                unlockedRacing = false,
                unlockedPlatformer = false,
                fightingLevel = 0,
                racingLevel = 0,
                platformerLevel = 0
            };
            SaveGame();
        }
    }
}