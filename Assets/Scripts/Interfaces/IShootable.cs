using UnityEngine;

namespace Interfaces
{
    public interface IShootable
    {
        float DamagePerShot { get; }
        float DelayBetweenShots { get; }
        KeyCode AttackKey { get; }
    }
}