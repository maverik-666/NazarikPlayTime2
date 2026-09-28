namespace Project
{
    public class DamageableRegistry : Registry<IDamageable>
    {
        public DamageableRegistry(int capacity) : base(capacity)
        {
        }
    }
}