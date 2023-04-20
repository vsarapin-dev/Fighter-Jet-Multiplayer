using System.Collections;
using Mirror;
using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneExplosion : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;

        [ServerCallback]
        public void Explode()
        {
            GameObject explosionGo = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(explosionGo);
            StartCoroutine(DestroyPlaneAfterExplosion());
        }

        private float GetExplosionDuration()
        {
            return explosionPrefab.GetComponent<ParticleSystem>().main.duration;
        }
    
        private IEnumerator DestroyPlaneAfterExplosion()
        {
            yield return new WaitForSeconds(GetExplosionDuration());
            NetworkServer.Destroy(gameObject);
        }
    }
}
