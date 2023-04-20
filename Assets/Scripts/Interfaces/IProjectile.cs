namespace Interfaces
{
    public interface IProjectile
    {
        float ProjectileSpeed { get; }
        float ProjectileLifetime { get; }
        float ProjectileDamage { get; set; }
    }
}