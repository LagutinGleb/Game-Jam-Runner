using Runner.Control;
using UnityEngine;

namespace Runner.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [Header("Running")]
        [SerializeField] float forwardSpeed = 10f;
        [SerializeField] float horizontalSpeed = .5f;

        [Header("Jumping")]
        [SerializeField] float gravity = -9.8f;
        [SerializeField] float jumpForce = 100;
        [SerializeField] float groundRayDistance = 2f;
        [SerializeField] AnimationCurve jumpCurve;
        float jumpCurrentTime, jumpTotalTime;
        bool isAbleToJump = true;
        bool isJumping;

        [Header("Sliding")]
        [SerializeField] AnimationCurve slideCurve;
        float slideSpeedMultiplier = 1;
        float slideCurrentTime, slideTotalTime;
        bool isAbleToSlide = true;
        bool isSliding;


        CharacterController characterController;
        PlayerAnimatorUpdater animatorUpdater;
        Vector3 movingVector;
        InputProvider inputProvider;

        delegate void OnProcessStarted();

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animatorUpdater = GetComponent<PlayerAnimatorUpdater>();
            inputProvider = GetComponent<InputProvider>();
        }

        private void Start()
        {
            jumpTotalTime = jumpCurve.keys[jumpCurve.length - 1].time;
            slideTotalTime = slideCurve.keys[slideCurve.length - 1].time;
        }

        void Update()
        {
            Move();
            HitTheFloor();
            Jump();
            Slide();
        }

        private void UpdateCurveTime(ref float currentTime, ref float totalTime, ref bool isProcessing)
        {
            if (currentTime >= totalTime)
            {
                currentTime = 0;
                isProcessing = false;
            }
        }

        private void Move()
        {
            movingVector = new Vector3(inputProvider.MovingDirection.x * horizontalSpeed, 
                movingVector.y + gravity, 
                forwardSpeed * slideSpeedMultiplier) * Time.deltaTime;

            characterController.Move(movingVector);
        }

        private void Jump()
        {
            UpdateCurveTime(ref jumpCurrentTime, ref jumpTotalTime, ref isJumping);

            if (inputProvider.JumpStarted && isAbleToJump)
            {
                isJumping = true;
                isAbleToJump = false;
                animatorUpdater.OnStartJumping();
            }

            if (isJumping)
            {
                jumpCurrentTime += Time.deltaTime;
                movingVector.y = jumpForce * jumpCurve.Evaluate(jumpCurrentTime);
            }
        }

        private void Slide()
        {
            UpdateCurveTime(ref slideCurrentTime, ref slideTotalTime, ref isSliding);

            if (isAbleToSlide && inputProvider.SlideStarted)
            {
                isSliding = true;
                isAbleToSlide = false;
                animatorUpdater.OnStartSliding();
            }

            if (isSliding)
            {
                slideCurrentTime += Time.deltaTime;
                slideSpeedMultiplier = slideCurve.Evaluate(slideCurrentTime);
                movingVector.y = -jumpForce * slideCurve.Evaluate(jumpCurrentTime);
            }
            else
            {
                animatorUpdater.OnEndSliding();
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
                isAbleToSlide = true;
                animatorUpdater.OnLanding();
            }
            else
            {
                isAbleToJump = false;
                animatorUpdater.Falling();
            }

            Debug.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundRayDistance, Color.red);
        }

    }
}
