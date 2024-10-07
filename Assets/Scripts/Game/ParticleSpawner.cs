using UnityEngine;

namespace Game
{
    public class ParticleSpawner : MonoBehaviour
    {
        public GameObject particlePrefab;
        
        public Transform SpawnParticle(Vector3 position)
        {
            return Instantiate(particlePrefab, position, Quaternion.identity).transform;
        }
    }
}