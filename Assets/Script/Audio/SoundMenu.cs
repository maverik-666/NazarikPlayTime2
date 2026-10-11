namespace Project
{
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class SoundMenu : MonoBehaviour
    {
        [SerializeField]
        private Slider _musicSlider;
        
        [SerializeField]
        private Slider _soundSlider;
        
        [SerializeField]
        private AudioMixerGroup _musicMixer;
        
        [SerializeField]
        private AudioMixerGroup _soundMixer;
     
        private bool _isInitialized = false;

        public void Init()
        {
            _isInitialized = true;
            
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
            _soundSlider.onValueChanged.AddListener(OnSoundVolumeChange);
        }
        
        private void Start()
        {
#if UNITY_EDITOR
            if (!_isInitialized)
            {
                Debug.LogError($"{GetType().Name} not initialized", this);
            }
#endif
            
            float musicVolume = PlayerPrefs.GetFloat("Music", 0.5f);
            float soundVolume = PlayerPrefs.GetFloat("Sound", 0.5f);
            
            _musicSlider.value = musicVolume;
            _soundSlider.value = soundVolume;
            
            SetMixerVolume(_musicMixer, "MusicVolume", musicVolume);
            SetMixerVolume(_soundMixer, "SoundVolume", soundVolume);
        }

        private void OnMusicVolumeChange(float volume)
        {
            PlayerPrefs.SetFloat("Music", volume);
            SetMixerVolume(_musicMixer, "MusicVolume", volume);
        }

        private void OnSoundVolumeChange(float volume)
        {
            PlayerPrefs.SetFloat("Sound", volume);
            SetMixerVolume(_soundMixer, "SoundVolume", volume);
        }

        private void SetMixerVolume(AudioMixerGroup mixer, string paramName, float volume)
        {
            mixer.audioMixer.SetFloat(paramName, Mathf.Log10(Mathf.Max(volume, 0.001f)) * 20);
        }
        
        private void OnDestroy()
        {
            _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChange);
            _soundSlider.onValueChanged.RemoveListener(OnSoundVolumeChange);
        }
    }
}