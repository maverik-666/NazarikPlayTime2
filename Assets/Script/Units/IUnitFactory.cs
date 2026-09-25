namespace Project
{
    using UnityEngine;

    public interface IUnitFactory
    {
        void Init(Registry<IDamageable> damageableRegistry);
        
        void Create(Vector3 position, Team team, Team enemyTeam, float direction);
    }
}