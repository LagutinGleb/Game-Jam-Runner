using UnityEngine;

namespace Runner.Player
{
    public class PlayerAnimatorUpdater : MonoBehaviour
    {
        Animator playerAnimator;

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
        }

        public void OnStartJumping()
        {
            playerAnimator.SetBool("Jump", true);
        }

        public void Falling()
        {
            playerAnimator.SetBool("Land", false);
        }

        public void OnLanding()
        {
            playerAnimator.SetBool("Land", true);
            playerAnimator.SetBool("Jump", false);
        }
    }
}