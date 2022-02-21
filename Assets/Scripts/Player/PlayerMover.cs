using Runner.Control;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Player
{
    public class PlayerMover : MonoBehaviour
    {
        Vector3 movingVector;

        [Header("Running")]
        [SerializeField] float forwardSpeed = 10f;
        [SerializeField] float horizontalSpeed = .5f;

        [Header("Jumping")]
        [SerializeField] float gravity = -9.8f;
        [SerializeField] float jumpForce = 100;
        [SerializeField] float groundRayDistance = 2f;
        [SerializeField] AnimationCurve jumpCurve;
        float jumpCurrentTime, jumpTotalTime;
        bool isJumping;

        [Header("Sliding")]
        [SerializeField] AnimationCurve slideCurve;
        [SerializeField] float downForce = 10f;
        float slideSpeedMultiplier = 1;
        float slideCurrentTime, slideTotalTime;
        bool isSliding;

        bool isOnGround = true;

        CharacterController characterController;
        PlayerAnimatorUpdater animatorUpdater;
        InputProvider inputProvider;

        public MovingDirection movingDirection = MovingDirection.Forward;
        Dictionary<MovingDirection, Vector3> directions = new Dictionary<MovingDirection, Vector3>();
        [SerializeField] float turnSpeed = 50f;
        public float turnFraction = 0;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animatorUpdater = GetComponent<PlayerAnimatorUpdater>();
            inputProvider = GetComponent<InputProvider>();

            directions[MovingDirection.Forward] = new Vector3 (0, 0, 0);
            directions[MovingDirection.Back] = new Vector3 (0, 180f, 0);
            directions[MovingDirection.Left] = new Vector3 (0, -90f, 0);
            directions[MovingDirection.Right] = new Vector3 (0, 90f, 0);
        }

        private void Start()
        {
            jumpTotalTime = jumpCurve.keys[jumpCurve.length - 1].time;
            slideTotalTime = slideCurve.keys[slideCurve.length - 1].time;
        }

        void Update()
        {
            Move();
            RaycastFloor();

            Jump();
            Slide();
        }

        //Update Animation curves for Jumping and Sliding processes
        private void UpdateCurveTime(ref float currentTime, ref float totalTime, ref bool isProcessing)
        {
            if (currentTime >= totalTime)
            {
                currentTime = 0;
                isProcessing = false;
            }
        }

        //Calculate result moving vector including jumping and sliding
        private void Move()
        {
            Turn(movingDirection);

            Vector3 forward = transform.forward * forwardSpeed;
            Vector3 strafe = transform.right * inputProvider.MovingDirection.x * horizontalSpeed;
            Vector3 vertical = transform.up * (movingVector.y + gravity);

            movingVector = (forward + strafe + vertical) * Time.deltaTime;
            characterController.Move(movingVector);
        }

        public void StartTurning(MovingDirection triggerDirection)
        {
            turnFraction = 0;
            movingDirection = triggerDirection;
        }

        void Turn(MovingDirection movingDirection)
        {
            turnFraction = Mathf.MoveTowards(turnFraction, 1, turnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(directions[movingDirection]), turnFraction);

            //Vector3 targetRotation = Vector3.MoveTowards(transform.rotation.eulerAngles, directions[movingDirection], turnSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(targetRotation);
        }

        private void Jump()
        {
            UpdateCurveTime(ref jumpCurrentTime, ref jumpTotalTime, ref isJumping);

            if (inputProvider.JumpStarted && isOnGround && !isJumping)
            {
                isJumping = true;
                isOnGround = false;
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

            if (inputProvider.SlideStarted)
            {
                isSliding = true;
            }

            if (isSliding)
            {
                movingVector.y = -downForce * slideCurve.Evaluate(jumpCurrentTime);
                if (isOnGround)
                {
                    slideCurrentTime += Time.deltaTime;
                    slideSpeedMultiplier = slideCurve.Evaluate(slideCurrentTime);
                }
            }
            else
            {
                animatorUpdater.OnEndSliding();
            }
        }

        void RaycastFloor()
        {
            Vector3 rayOrigin = transform.position + Vector3.up * groundRayDistance / 2;

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, groundRayDistance))
            {
                isOnGround = true;

                if(isSliding)
                    animatorUpdater.OnStartSliding();
                else
                    animatorUpdater.OnLanding();
            }
            else
            {
                isOnGround = false;
                animatorUpdater.Falling();
            }

            Debug.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundRayDistance, Color.red);
        }
    }
}
