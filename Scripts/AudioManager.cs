using UnityEngine;
using System.Collections;

namespace PotionForest.Core
{
    /// <summary>
    /// Handles cozy background music and soft sound effects like pops and bubbles.
    /// Includes fade-in and fade-out mechanics so audio never starts abruptly.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [Tooltip("Source for the looping cozy background music.")]
        public AudioSource musicSource;
        [Tooltip("Source for short UI and interaction sounds.")]
        public AudioSource sfxSource;

        [Header("Cozy Audio Clips")]
        public AudioClip backgroundMusic;
        public AudioClip popSound;
        public AudioClip bubbleSound;

        [Header("Fade Settings")]
        public float fadeDuration = 1.5f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (backgroundMusic != null)
            {
                PlayMusicWithFade(backgroundMusic);
            }
        }

        /// <summary>
        /// Plays a sound effect by string name. Pitch is slightly randomized for a natural feel.
        /// </summary>
        public void PlaySFX(string clipName)
        {
            AudioClip clipToPlay = null;
            if (clipName.ToLower() == "pop") clipToPlay = popSound;
            else if (clipName.ToLower() == "bubble") clipToPlay = bubbleSound;

            if (clipToPlay != null && sfxSource != null)
            {
                // Slight pitch variation so repeated sounds feel organic, not robotic
                sfxSource.pitch = Random.Range(0.95f, 1.05f);
                sfxSource.PlayOneShot(clipToPlay);
            }
        }

        /// <summary>
        /// Smoothly fades out current music and fades in the new music.
        /// </summary>
        public void PlayMusicWithFade(AudioClip newClip)
        {
            StartCoroutine(FadeMusicRoutine(newClip));
        }

        private IEnumerator FadeMusicRoutine(AudioClip newClip)
        {
            if (musicSource.isPlaying)
            {
                float startVolume = musicSource.volume;
                while (musicSource.volume > 0)
                {
                    musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                    yield return null;
                }
                musicSource.Stop();
            }

            musicSource.clip = newClip;
            musicSource.volume = 0f;
            musicSource.Play();

            float targetVolume = 0.5f; // Soft volume so it's not overwhelming
            while (musicSource.volume < targetVolume)
            {
                musicSource.volume += targetVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }
    }
}
