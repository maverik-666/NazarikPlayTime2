namespace Project
{

    public interface IUnit : IDamageable
    {
        float Direction { get; }
        
        public Team EnemyTeam { get; }
        
        void SetDirection(float direction);

        public void PickedUp();

        void Dispose();
    }
}