namespace Project
{
    using UnityEngine;

    public interface IMover
    {
        public Transform transform { get; }
        
        public Vector2 Position { get; }
        
        float MoveSpeed { get; }
        
        float VisionDistance { get; }

        public float Direction { get; }
        
        bool IsEnemyInVision(IDamageable enemy);
    }
}