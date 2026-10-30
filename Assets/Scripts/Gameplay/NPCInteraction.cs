using UnityEngine;

namespace Imagination.Gameplay
{
    public class NPCInteraction : MonoBehaviour
    {
        [Header("NPC Settings")]
        public string npcName = "NPC";
        public Managers.QuestType questType = Managers.QuestType.Fighting;
        public int questLevel = 1;
        public float interactionRadius = 1.5f;

        [Header("Dialogue")]
        public string firstTimeDialogId = "npc1_first";
        public string repeatableDialogId = "npc1_repeat";

        private Transform player;
        private bool isInRange = false;
        private bool isFirstInteraction = true;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            var questId = $"fighting_{questLevel}";
            if (Managers.QuestManager.Instance.IsQuestUnlocked(questId))
                isFirstInteraction = false;
        }

        private void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);
            bool wasInRange = isInRange;
            isInRange = distance <= interactionRadius;

            if (isInRange && !wasInRange)
                UIManager.Instance?.ShowPrompt("Tekan untuk berinteraksi");

            if (!isInRange && wasInRange)
                UIManager.Instance?.HidePrompt();

            if (isInRange && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Interact();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = true;
                UIManager.Instance?.ShowPrompt("Tekan untuk berinteraksi");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = false;
                UIManager.Instance?.HidePrompt();
            }
        }

        private void Interact()
        {
            if (isFirstInteraction)
            {
                isFirstInteraction = false;
                var quest = Managers.QuestManager.Instance.GetFirstAvailableQuest(questType);
                if (quest != null)
                {
                    UIManager.Instance.ShowQuestDialogue(npcName, quest.title, quest.description, () =>
                    {
                        Managers.QuestManager.Instance.UnlockQuest(quest.questId);
                        EnterGameplayMode();
                    });
                }
            }
            else
            {
                var nextQuest = GetNextUnlockedQuest();
                if (nextQuest != null)
                {
                    UIManager.Instance.ShowQuestDialogue(npcName, nextQuest.title, nextQuest.description, () =>
                    {
                        EnterGameplayMode();
                    });
                }
                else
                {
                    UIManager.Instance.ShowDialogueSimple(npcName, "Kamu sudah menyelesaikan semua tantangan ku!", null);
                }
            }
        }

        private Managers.Quest GetNextUnlockedQuest()
        {
            var quests = new System.Collections.Generic.List<Managers.Quest>();
            foreach (var q in new[] { "fighting_1", "fighting_2", "fighting_3" })
            {
                var quest = Managers.QuestManager.Instance.GetQuest(q);
                if (quest != null)
                    quests.Add(quest);
            }

            foreach (var q in quests)
            {
                if (q.isUnlocked && !q.isCompleted)
                    return q;
            }
            return null;
        }

        private void EnterGameplayMode()
        {
            PlayerController playerCtrl = player.GetComponent<PlayerController>();
            if (playerCtrl != null)
                playerCtrl.DisableMovement();

            SceneLoader.Instance.LoadScene("Fighting_Level1");
        }
    }
}