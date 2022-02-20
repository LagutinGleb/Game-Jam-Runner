using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Platforms
{
    public class Platform : MonoBehaviour
    {
        public Transform origin;
        public Transform end;
        public Collider spawnPortal;

        private void Awake()
        {
            GameObject.FindObjectOfType<PlatformSpawner>().platforms.Add(this);
        }
    }
}
