using System;
using UnityEngine;
using UnityEngine.UI;

namespace Imagination.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject menuScreen;
        public GameObject dialoguePanel;
        public GameObject loadingScreen;
        public GameObject victoryScreen;
        public GameObject touchControls;

        [Header("Dialogue UI")]
        public TextMeshUGUI dialogueText;
        public TextMeshUGUI speakerNameText;
        public Image portraitImage;
        public Button continueButton;

        [Header("Prompts")]
        public GameObject promptPanel;
        public TextMeshUGUI promptMessage;

        private Action pendingQuestAction;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (continueButton != null)
                continueButton.onClick.AddListener(OnDialogueContinue);
        }

        public void ShowMenu() => menuScreen?.SetActive(true);
        public void HideMenu() => menuScreen?.SetActive(false);

        public bool ShowLoadingScreen()
        {
            loadingScreen?.SetActive(true);
            return true;
        }

        public void ShowDialogue(Managers.DialogueData data, Action onComplete = null)
        {
            dialoguePanel.SetActive(true);
            pendingQuestAction = onComplete;

            if (data != null && data.lines.Count > 0)
            {
                var line = data.lines[0];
                speakerNameText.text = line.speaker;
                dialogueText.text = line.text;
                portraitImage.sprite = line.portrait;
            }
        }

        public void HideDialogue() => dialoguePanel.SetActive(false);

        private void OnDialogueContinue()
        {
            pendingQuestAction?.Invoke();
            pendingQuestAction = null;
        }

        public void ShowVictory(int level, Action onContinue = null)
        {
            victoryScreen?.SetActive(true);
            pendingQuestAction = onContinue;
        }

        public void OnVictoryContinue()
        {
            victoryScreen?.SetActive(false);
            pendingQuestAction?.Invoke();
            pendingQuestAction = null;
        }

        public void ToggleTouchControls(bool show) => touchControls?.SetActive(show);

        public void ShowPrompt(string message)
        {
            promptMessage.text = message;
            promptPanel.SetActive(true);
        }

        public void HidePrompt()
        {
            promptPanel.SetActive(false);
        }

        public void ShowQuestDialogue(string speaker, string title, string description, Action onAccept)
        {
            dialoguePanel.SetActive(true);
            speakerNameText.text = speaker;
            dialogueText.text = $"[{title}]\n{description}\n\nTekan untuk menerima quest!";
            pendingQuestAction = onAccept;
        }

        public void ShowDialogueSimple(string speaker, string text, Action onComplete)
        {
            dialoguePanel.SetActive(true);
            speakerNameText.text = speaker;
            dialogueText.text = text;
            pendingQuestAction = onComplete;

            if (string.IsNullOrEmpty(text))
            {
                onComplete?.Invoke();
                HideDialogue();
            }
        }
    }
}
