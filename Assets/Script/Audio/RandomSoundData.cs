namespace Project
{
    using System;
    using UnityEngine;

    [Serializable]
    public class RandomSoundData
    {
        [SerializeField]
        private AudioClip[] _audioClips;
        
        [SerializeField]
        private float _soundVolume = 0.5f;
        
        public SingleSoundData GetRandomSound()
        {
            return new SingleSoundData(_audioClips[UnityEngine.Random.Range(0, _audioClips.Length)], _soundVolume);
        }
    }

    [Serializable]
    public class SingleSoundData
    {
        [SerializeField]
        private AudioClip _audioClip;
        
        [SerializeField]
        private float _soundVolume = 0.5f;

        [SerializeField]
        private Vector2 _pitchRange;
        
        public AudioClip AudioClip => _audioClip;

        public float Volume => _soundVolume;
        
        public Vector2 PitchRange => _pitchRange;

        public SingleSoundData()
        {
            _audioClip = null;
            _soundVolume = 0.5f;
            _pitchRange = Vector2.one;
        }
        
        public SingleSoundData(AudioClip audioClip, float volume)
        {
            _audioClip = audioClip;
            _soundVolume = volume;
            _pitchRange = Vector2.one;
        }
        
        public float GetRandomPitch()
        {
            return UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y);
        }
    }
}