namespace Project
{
    using UnityEngine;

    public interface IUnitFactory
    {
        void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry);
        
        void Create(Vector3 position, Team team, Team enemyTeam, float direction);
    }
}