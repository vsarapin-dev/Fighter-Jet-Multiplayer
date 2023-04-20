using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        float Health { get; }
        void TakeDamage(float damage);
        void MakePlayerDead();
    }
}