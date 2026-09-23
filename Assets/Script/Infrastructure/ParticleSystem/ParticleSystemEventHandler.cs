namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemEventHandler : MonoBehaviour
    {
        public event Action ParticleSystemStopped;

        private void OnParticleSystemStopped()
        {
            ParticleSystemStopped?.Invoke();
        }
    }
}