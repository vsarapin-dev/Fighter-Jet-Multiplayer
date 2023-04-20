using Interfaces;
using Mirror;
using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneLaser : NetworkBehaviour, IProjectile
    {
        private float _laserSpeed = 500f;
        private float _laserLifetime = 3f;
        private float _timerLaserLifeTime;
        private float _damage;
        
        public float ProjectileSpeed => _laserSpeed;
        public float ProjectileLifetime => _laserLifetime;

        public float ProjectileDamage
        {
            get => _damage;
            set
            {
                if (value > 0)
                {
                    _damage = value;
                }
            }
        }

        private void Update()
        {
            MoveLaser();
        }

        private void MoveLaser()
        {
            transform.Translate(Vector3.forward * _laserSpeed * Time.deltaTime);
            _timerLaserLifeTime += Time.deltaTime;

            if (_timerLaserLifeTime >= _laserLifetime)
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.TryGetComponent(out IDamageable playerToDamage))
            {
                playerToDamage.TakeDamage(_damage);
            }
            NetworkServer.Destroy(gameObject);
        }
    }
}
