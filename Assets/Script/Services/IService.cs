namespace Project
{
    using System;

    public interface IService
    {
        public int Price { get; } 
        
        public event Action StateChanged;
        
        public float Cooldown { get; }

        public float CurrentCooldown { get; }
        
        public bool IsAvailable { get; } 
        
        public ServiceButton ServiceButtonPf { get; }

        public void Execute();
    }
}