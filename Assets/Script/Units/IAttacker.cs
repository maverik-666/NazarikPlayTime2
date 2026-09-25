namespace Project
{
    using UnityEngine;

    public interface IAttacker
    {
        public GameObject gameObject { get; }
        
        public int Attack { get; }

        public float AttackCooldown { get; }
    }
}