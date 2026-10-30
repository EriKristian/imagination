using UnityEngine;
using UnityEngine.UI;
using Imagination.Core;
using Imagination.Managers;
using TMPro;

namespace Imagination.UI
{
    public class MenuController : MonoBehaviour
    {
        [Header("Buttons")]
        public Button newGameButton;
        public Button continueButton;
        public Button settingsButton;
        public Button quitButton;

        [Header("Settings Panel")]
        public GameObject settingsPanel;
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;

        private void Start()
        {
            newGameButton.onClick.AddListener(OnNewGame);
            continueButton.onClick.AddListener(OnContinue);
            settingsButton.onClick.AddListener(OnSettings);
            quitButton.onClick.AddListener(OnQuit);

            LoadSettingsUI();
        }

        private void OnNewGame() => GameManager.Instance.StartNewGame();
        private void OnContinue() => GameManager.Instance.ContinueGame();
        private void OnSettings() => settingsPanel.SetActive(true);

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void LoadSettingsUI()
        {
            var saveData = SaveManager.Instance.GetSaveData();
            masterVolumeSlider.value = saveData.masterVolume;
            musicVolumeSlider.value = saveData.musicVolume;
            sfxVolumeSlider.value = saveData.sfxVolume;

            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        public void OnMasterVolumeChanged(float value)
        {
            var saveData = SaveManager.Instance.GetSaveData();
            saveData.masterVolume = value;
            AudioManager.Instance.SetMusicVolume(value);
            AudioManager.Instance.SetSFXVolume(value);
            SaveManager.Instance.SaveGame();
        }

        public void OnMusicVolumeChanged(float value)
        {
            var saveData = SaveManager.Instance.GetSaveData();
            saveData.musicVolume = value;
            AudioManager.Instance.SetMusicVolume(value);
            SaveManager.Instance.SaveGame();
        }

        public void OnSFXVolumeChanged(float value)
        {
            var saveData = SaveManager.Instance.GetSaveData();
            saveData.sfxVolume = value;
            AudioManager.Instance.SetSFXVolume(value);
            SaveManager.Instance.SaveGame();
        }

        public void OnBackFromSettings() => settingsPanel.SetActive(false);
    }
}
