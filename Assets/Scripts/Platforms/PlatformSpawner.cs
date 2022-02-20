using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Platforms
{
    public class PlatformSpawner : MonoBehaviour
    {
        public List<Platform> platforms = new List<Platform>();
        [SerializeField] Platform platformPrefab;
        [SerializeField] Transform platformFolder;

        void Update()
        {
            while (platforms.Count < 3)
            {
                SpawnPlatform();
            }
        }

        public void SpawnPlatform()
        {
            Platform lastSpawnedPlatform = platforms[platforms.Count - 1];
            Platform newPlatform;
            Vector3 newPlatformPos = lastSpawnedPlatform.end.position + platformPrefab.transform.position - platformPrefab.origin.position;
            newPlatform = Instantiate(platformPrefab, newPlatformPos, Quaternion.identity);
            newPlatform.transform.SetParent(platformFolder);
        }
    }
}