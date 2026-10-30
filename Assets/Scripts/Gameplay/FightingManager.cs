using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Imagination.Core;
using Imagination.Managers;
using Imagination.UI;

namespace Imagination.Gameplay
{
    public class FightingManager : MonoBehaviour
    {
        [Header("Player")]
        public Transform player;
        public float playerSpeed = 3f;
        public float attackRange = 1f;

        [Header("Opponent")]
        public Transform opponent;
        public float opponentSpeed = 2f;
        public float opponentAttackRange = 1.5f;

        [Header("Stats")]
        public int playerMaxHealth = 100;
        public int opponentMaxHealth = 100;
        public float roundTime = 60f;

        [Header("UI")]
        public Slider playerHealthBar;
        public Slider opponentHealthBar;
        public TMP_Text timerText;
        public GameObject victoryScreen;
        public GameObject defeatScreen;

        private int playerHealth;
        private int opponentHealth;
        private float currentTime;
        private bool isRoundActive = false;

        private void Start()
        {
            playerHealth = playerMaxHealth;
            opponentHealth = opponentMaxHealth;
            currentTime = roundTime;

            UpdateUI();
            UIManager.Instance.ToggleTouchControls(true);
            isRoundActive = true;
        }

        private void Update()
        {
            if (!isRoundActive) return;
            HandlePlayerInput();
            HandleOpponentAI();
            UpdateTimer();
        }

        private void HandlePlayerInput()
        {
            Vector2 direction = Vector2.zero;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 screenTap = Camera.main.ScreenToWorldPoint(touch.position);
                direction = (screenTap - (Vector2)player.position).normalized;
            }
            else
            {
                direction.x = Input.GetAxisRaw("Horizontal");
                direction.y = Input.GetAxisRaw("Vertical");
            }

            player.position += (Vector3)(direction * playerSpeed * Time.deltaTime);

            if (direction.magnitude > 0.1f)
                FaceDirection(player, direction);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                TryAttack(player, opponent, attackRange);
        }

        private void HandleOpponentAI()
        {
            if (opponent == null || player == null) return;

            Vector2 dirToPlayer = (player.position - opponent.position).normalized;

            if (Vector2.Distance(opponent.position, player.position) > opponentAttackRange)
            {
                opponent.position += (Vector3)(dirToPlayer * opponentSpeed * Time.deltaTime);
            }
            else
            {
                TryAttack(opponent, player, opponentAttackRange);
            }

            if (dirToPlayer.magnitude > 0.1f)
                FaceDirection(opponent, dirToPlayer);
        }

        private void TryAttack(Transform attacker, Transform target, float range)
        {
            if (Vector2.Distance(attacker.position, target.position) <= range)
            {
                bool isPlayerTarget = (target == player);
                target.GetComponent<Animator>()?.SetTrigger("Hit");

                if (isPlayerTarget)
                {
                    playerHealth -= 10;
                    playerHealth = Mathf.Max(playerHealth, 0);
                    if (playerHealth <= 0)
                        EndRound(false);
                }
                else
                {
                    opponentHealth -= 10;
                    opponentHealth = Mathf.Max(opponentHealth, 0);
                    if (opponentHealth <= 0)
                        EndRound(true);
                }

                UpdateUI();
            }
        }

        private void FaceDirection(Transform t, Vector2 direction)
        {
            if (direction.x > 0)
                t.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                t.localScale = new Vector3(-1, 1, 1);
        }

        private void UpdateTimer()
        {
            currentTime -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(currentTime).ToString();

            if (currentTime <= 0)
                EndRound(false);
        }

        private void EndRound(bool playerWon)
        {
            isRoundActive = false;
            UIManager.Instance.ToggleTouchControls(false);

            if (playerWon)
            {
                victoryScreen?.SetActive(true);
                Managers.QuestManager.Instance.CompleteQuest("fighting_1");
                Managers.ProgressionManager.Instance.CompleteFightingLevel(1);
            }
            else
            {
                defeatScreen?.SetActive(true);
            }
        }

        public void OnVictoryContinue()
        {
            victoryScreen?.SetActive(false);
            SceneLoader.Instance.LoadScene("Classroom");
        }

        public void OnDefeatRetry()
        {
            defeatScreen?.SetActive(false);
            SceneLoader.Instance.LoadScene("Fighting_Level1");
        }

        private void UpdateUI()
        {
            if (playerHealthBar != null)
                playerHealthBar.value = (float)playerHealth / playerMaxHealth;
            if (opponentHealthBar != null)
                opponentHealthBar.value = (float)opponentHealth / opponentMaxHealth;
        }
    }
}
