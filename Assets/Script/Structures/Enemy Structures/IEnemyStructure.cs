namespace Project
{
    public interface IEnemyStructure
    {
        Structure Structure { get; }

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry);
    }
}