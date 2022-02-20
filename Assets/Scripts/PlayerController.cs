using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runner.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Horizontal Movement")]
        Vector2 movingDirection;
        [SerializeField] float forwardSpeed = 10f;
        [SerializeField] float horizontalSpeed = .5f;

        [Header("Vertical Movement")]
        float jumpEffort;
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

        public void OnMove(InputAction.CallbackContext context)
        {
            movingDirection = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            jumpEffort = context.ReadValue<float>();
        }

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerAnimator = GetComponent<Animator>();
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
            movingVector = new Vector3(movingDirection.x * horizontalSpeed, movingVector.y + gravity, forwardSpeed) * Time.deltaTime;
            characterController.Move(movingVector);
        }

        private void Jump()
        {
            if (jumpEffort == 1 && isAbleToJump)
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
