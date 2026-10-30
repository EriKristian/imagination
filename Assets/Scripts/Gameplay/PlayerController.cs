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

        private bool isWalking = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            UI.UIManager.Instance.ToggleTouchControls(useTouch);
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
                // touch start position handled implicitly
            }

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(touch.position) - (Vector2)transform.position).normalized;
                if (direction.magnitude > 0.1f)
                {
                    moveInput = direction;
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
            rb.linearVelocity = moveInput * moveSpeed;
        }

        private void SetWalking(bool walking)
        {
            isWalking = walking;
        }

        public void DisableMovement()
        {
            rb.linearVelocity = Vector2.zero;
            moveInput = Vector2.zero;
            enabled = false;
        }

        public void EnableMovement()
        {
            enabled = true;
        }
    }
}
