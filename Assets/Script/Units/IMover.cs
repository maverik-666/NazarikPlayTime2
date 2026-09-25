namespace Project
{
    using UnityEngine;

    public interface IMover
    {
        public Transform transform { get; }
        
        public Vector2 Position { get; }
        
        float MoveSpeed { get; }
        
        float VisionDistance { get; }

        public Team EnemyTeam { get; }
        
        public float Direction { get; }
    }
}