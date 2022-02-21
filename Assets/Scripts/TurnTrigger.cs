using Runner.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnTrigger : MonoBehaviour
    {
        [SerializeField] MovingDirection triggerDirection;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<PlayerMover>().StartTurning(triggerDirection);
        }
    }
}