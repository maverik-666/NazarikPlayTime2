namespace Project
{
    public class UnitRegistry : Registry<IUnit>
    {
        public UnitRegistry(int capacity) : base(capacity)
        {
        }
    }
}
