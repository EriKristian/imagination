using UnityEngine;

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