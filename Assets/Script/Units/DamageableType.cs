namespace Project
{
    using System;

    [Flags]
    public enum DamageableType
    {
        None = 0,
        Unit = 1 << 0,
        Structure = 1 << 1,
        Base = 1 << 2,
    }

    [Flags]
    public enum DamageableProperty
    {
        None = 0,
        Intangible = 1 << 0,
    }
}