using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    using UnityEngine.Audio;

    [DisallowMultipleComponent]
    public sealed class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Serializable]
        public struct SfxEntry
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }

        [SerializeField] 
        private List<SfxEntry> _sfx = new List<SfxEntry>();

        [Header("Pool")]
        [SerializeField] 
        private int _initialPoolSize = 8;
        
        [SerializeField] 
        private int _maxPoolSize = 32;
        
        [SerializeField] 
        private bool _expandPoolIfNeeded = true;

        [Header("Sound Control")]
        [SerializeField]
        private AudioMixerGroup _mixerGroup;
        
        private readonly List<AudioSource> _pool = new List<AudioSource>();
        private readonly Queue<int> _free = new Queue<int>();
        private Transform _root;

        public void Init()
        {
            if (Instance != null)
            {
                Destroy(gameObject); 
                return;
            }
            
            Instance = this;
            
            transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);

            _root = new GameObject("SFX_Pool").transform;
            _root.SetParent(transform, false);

            for (int i = 0; i < _initialPoolSize; i++) 
                CreateSource();
        }

        private float GetGlobalSfxVolume()
        {
            return 1f;
        }

        public void PlayById(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            int index = _sfx.FindIndex(e => e.id == id);
            if (index >= 0) PlayByIndex(index);
        }

        public void PlayByIndex(int index)
        {
            if (index < 0 || index >= _sfx.Count) return;
            var e = _sfx[index];
            if (e.clip == null) return;
            Play(e.clip, e.volume <= 0f ? 1f : e.volume, 1f);
        }

        public void Play(SingleSoundData soundData, float pitch = 1f)
        {
            Play(soundData.AudioClip, soundData.Volume, pitch);
        }
        
        public void Play(SingleSoundData soundData)
        {
            Play(soundData.AudioClip, soundData.Volume, soundData.PitchRange == Vector2Int.one ? 1 : soundData.GetRandomPitch());
        }
        
        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            float global = GetGlobalSfxVolume();
            if (global <= 0f) return;

            int idx = Rent();
            if (idx < 0) return;

            var s = _pool[idx];
            s.pitch = pitch;
            s.volume = Mathf.Clamp01(volume * global);
            s.clip = clip;
            s.Play();

            StartCoroutine(ReturnWhenDone(idx));
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject($"SFX_{_pool.Count}");
            go.transform.SetParent(_root, false);
            var s = go.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.loop = false;
            s.outputAudioMixerGroup = _mixerGroup;
            _pool.Add(s);
            _free.Enqueue(_pool.Count - 1);
            return s;
        }

        private int Rent()
        {
            if (_free.Count == 0)
            {
                if (_expandPoolIfNeeded && _pool.Count < _maxPoolSize) CreateSource();
                else
                {
                    for (int i = 0; i < _pool.Count; i++)
                        if (!_pool[i].isPlaying) { _free.Enqueue(i); break; }
                }
            }

            if (_free.Count == 0) return -1;
            return _free.Dequeue();
        }

        private void Return(int idx)
        {
            if (idx < 0) return;
            var s = _pool[idx];
            s.clip = null;
            _free.Enqueue(idx);
        }

        private IEnumerator ReturnWhenDone(int idx)
        {
            var s = _pool[idx];
            yield return new WaitWhile(() => s && s.isPlaying);
            Return(idx);
        }
    }
}