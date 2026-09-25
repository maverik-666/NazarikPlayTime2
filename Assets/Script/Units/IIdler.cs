namespace Project
{
    using UnityEngine;

    public interface IIdler
    {        
        public Vector2 Position { get; }
        
        float VisionDistance { get; }
        
        public Team EnemyTeam { get; }
    }
}