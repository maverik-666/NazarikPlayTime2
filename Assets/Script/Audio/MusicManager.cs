namespace Project
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.Audio;

    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _source1;
        
        [SerializeField]
        private AudioSource _source2;

        [SerializeField]
        private AudioMixerGroup _mixerGroup;
        
        [SerializeField]
        private float _crossFadeDuration = 0.5f;
        
        private Sequence _crossFadeTween;
        private bool _isActiveSource2;

        public void Init()
        {
            if (Instance != null)
            {
                Destroy(gameObject); 
                return;
            }
            
            Instance = this;

            _source1.loop = _source2.loop = true;
            _source1.outputAudioMixerGroup = _source2.outputAudioMixerGroup = _mixerGroup;
            
            transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);
        }
        
        public void PlayMusic(AudioClip clip, float volume = 1)
        {
            if (_isActiveSource2)
            {
                _source1.clip = clip;
                CrossFade(_source2, _source1, volume);
            }
            else
            {
                _source2.clip = clip;
                CrossFade(_source1, _source2, volume);
            }
        }

        public void Pause()
        {
           _source1.Pause();
           _source2.Pause();
        }

        public void Resume()
        {
            _source1.Play();
            _source2.Play();
        }

        private void CrossFade(AudioSource previousSource, AudioSource newSource, float targetVolume)
        {
            _crossFadeTween?.Kill();
            
            newSource.Play();
            newSource.volume = 0;
            
            _crossFadeTween = DOTween.Sequence();
            _crossFadeTween.SetUpdate(true);
            _crossFadeTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _crossFadeTween.Append(previousSource.DOFade(0, _crossFadeDuration));
            _crossFadeTween.Join(newSource.DOFade(targetVolume, _crossFadeDuration));
            _crossFadeTween.OnComplete(AfterCrossFade);
        }

        public void Stop()
        {
            _source1.DOFade(0, 0.3f)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _source2.DOFade(0, 0.3f)           
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
        
        private void AfterCrossFade()
        {
            _isActiveSource2 = !_isActiveSource2;
        }
    }
}