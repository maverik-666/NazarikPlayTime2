namespace Project
{
    public interface IAttackState
    {
        public bool CanAttack { get; }

        public string ID { get; }
    }
}