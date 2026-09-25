namespace Project
{
    using UnityEngine;

    public interface IUnit : IDamageable
    {
        public Transform transform { get; }
        
        float Direction { get; }
        
        void SetDirection(float direction);

        void Dispose();
    }
}