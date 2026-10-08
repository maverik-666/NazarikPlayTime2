namespace Project
{
    using UnityEngine;

    public interface IDamageable
    {
        public Transform transform { get; }
        
        public Vector2 Position { get; }
        
        public float Radius { get; }
        
        public Health Health { get; }
        
        public Team Team { get; }
        
        public DamageableType Type { get; }
        
        public DamageableProperty Properties { get; }
        
        public bool IsDead => Health.IsDead;

        public bool CanNotTakeDamage { get; }

        public void SetInvincible(bool value);
        
        public void TakeDamage(int damage);

        public void InstantDeath();
    }
}