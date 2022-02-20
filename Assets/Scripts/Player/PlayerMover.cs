using Runner.Control;
using UnityEngine;

namespace Runner.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [Header("Horizontal Movement")]
        [SerializeField] float forwardSpeed = 10f;
        [SerializeField] float horizontalSpeed = .5f;

        [Header("Vertical Movement")]
        [SerializeField] float gravity = -9.8f;
        [SerializeField] float jumpForce = 100;
        [SerializeField] float groundRayDistance = 2f;
        [SerializeField] AnimationCurve jumpCurve;
        float currentTime, totalTime;
        bool isAbleToJump = true;
        bool isJumping;

        CharacterController characterController;
        Animator playerAnimator;
        Vector3 movingVector;
        InputProvider inputProvider;


        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerAnimator = GetComponent<Animator>();
            inputProvider = GetComponent<InputProvider>();
        }

        private void Start()
        {
            totalTime = jumpCurve.keys[jumpCurve.length - 1].time;
        }

        void Update()
        {
            Move();
            HitTheFloor();
            Jump();
            UpdateCurveTime();
        }
        private void UpdateCurveTime()
        {
            if (currentTime >= totalTime)
            {
                currentTime = 0;
                isJumping = false;
            }
        }

        private void Move()
        {
            movingVector = new Vector3(inputProvider.MovingDirection.x * horizontalSpeed, movingVector.y + gravity, forwardSpeed) * Time.deltaTime;
            characterController.Move(movingVector);
        }

        private void Jump()
        {
            if (inputProvider.JumpStarted && isAbleToJump)
            {
                isJumping = true;
                isAbleToJump = false;
                playerAnimator.SetBool("Jump", true);
            }

            if (isJumping)
            {
                currentTime += Time.deltaTime;
                movingVector.y = jumpForce * jumpCurve.Evaluate(currentTime);
            }
        }

        void HitTheFloor()
        {
            Vector3 rayOrigin = transform.position + Vector3.up * groundRayDistance / 2;

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundRayDistance))
            {
                isAbleToJump = true;
                playerAnimator.SetBool("Land", true);
                playerAnimator.SetBool("Jump", false);
            }
            else
            {
                isAbleToJump = false;
                playerAnimator.SetBool("Land", false);
            }

            Debug.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundRayDistance, Color.red);
        }
    }
}
