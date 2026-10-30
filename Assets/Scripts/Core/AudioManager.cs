using UnityEngine;
using UnityEngine.Audio;

namespace Imagination.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Mixer")]
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup musicGroup;

        private AudioSource musicSource;
        private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicGroup;
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = sfxGroup;
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 0f)
        {
            if (clip == null) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public void StopMusic() => musicSource.Stop();
        public void StopAllSFX() => sfxSource.Stop();

        public void SetMusicVolume(float volume) => musicSource.volume = volume;
        public void SetSFXVolume(float volume) => sfxSource.volume = volume;
    }
}