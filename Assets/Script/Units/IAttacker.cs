namespace Project
{
    using UnityEngine;

    public interface IAttacker
    {
        public GameObject gameObject { get; }
        
        public Vector2 Position { get; }
        
        public int Attack { get; }
        
        public Team EnemyTeam { get; }
        
        public float AttackCooldown { get; }
    }
}