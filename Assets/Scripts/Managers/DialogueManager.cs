using System;
using System.Collections.Generic;
using UnityEngine;
using Imagination.UI;

namespace Imagination.Managers
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
        public Sprite portrait;
    }

    [Serializable]
    public class DialogueData
    {
        public string dialogueId;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private Action onDialogueCompleted;

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

        public void StartDialogue(DialogueData data, Action onComplete = null)
        {
            onDialogueCompleted = onComplete;
            UIManager.Instance.ShowDialogue(data);
        }

        public void CompleteDialogue()
        {
            onDialogueCompleted?.Invoke();
            onDialogueCompleted = null;
            UIManager.Instance.HideDialogue();
        }
    }
}