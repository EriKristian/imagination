using UnityEngine;

namespace Imagination.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float touchThreshold = 0.1f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool useTouch = true;
        private Vector2 touchStartPos;

        private bool isWalking = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            UIManager.Instance.ToggleTouchControls(useTouch);
        }

        private void Update()
        {
            if (useTouch)
                HandleTouchInput();
        }

        private void FixedUpdate()
        {
            if (useTouch)
                HandleMovement();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                moveInput = Vector2.zero;
                SetWalking(false);
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = touch.position - touchStartPos;
                if (direction.magnitude > touchThreshold)
                {
                    moveInput = direction.normalized;
                    SetWalking(true);
                }
                else
                {
                    moveInput = Vector2.zero;
                    SetWalking(false);
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                moveInput = Vector2.zero;
                SetWalking(false);
            }
        }

        private void HandleMovement()
        {
            rb.velocity = moveInput * moveSpeed;
        }

        private void SetWalking(bool walking)
        {
            if (walking != isWalking)
            {
                isWalking = walking;
            }
        }

        public void DisableMovement()
        {
            rb.velocity = Vector2.zero;
            moveInput = Vector2.zero;
            enabled = false;
        }

        public void EnableMovement()
        {
            enabled = true;
        }
    }
}