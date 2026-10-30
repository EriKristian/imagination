using UnityEngine;
using System.Collections;
using Imagination.Managers;
using Imagination.Core;

namespace Imagination.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Gameplay Settings")]
        public bool isNewGame = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();

            // Auto-start the game flow: show MainMenu
            StartCoroutine(StartGameFlow());
        }

        private IEnumerator StartGameFlow()
        {
            // Wait for all managers to initialize
            yield return new WaitForSeconds(0.1f);
            // Load MainMenu
            SceneLoader.Instance.LoadSceneImmediate("MainMenu");
        }

        private void InitializeManagers()
        {
            _ = SaveManager.Instance;
            _ = ProgressionManager.Instance;
            _ = AudioManager.Instance;
        }

        public void StartNewGame()
        {
            isNewGame = true;
            SaveManager.Instance.ResetSaveData();
            ProgressionManager.Instance.ResetProgression();
            SceneLoader.Instance.LoadScene("Classroom");
        }

        public void ContinueGame()
        {
            isNewGame = false;
            SceneLoader.Instance.LoadScene("Classroom");
        }
    }
}