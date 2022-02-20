using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runner.Control
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProvider : MonoBehaviour
    {
        public Vector2 MovingDirection { get; private set; }
        public float JumpEffort { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovingDirection = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpEffort = context.ReadValue<float>();
        }

    }
}