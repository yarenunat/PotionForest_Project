using UnityEngine;
using System.Collections;

namespace PotionForest.Core
{
    /// <summary>
    /// Singleton GameManager managing core game states such as stardust, 
    /// and controlling atmosphere transitions (time scale fading).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Currencies")]
        public int stardustCount;
        public int coinCount;

        [Header("Cozy Settings")]
        [Tooltip("Controls the time scale fading speed for smooth, peaceful transitions.")]
        public float timeScaleFadeDuration = 1.5f;
        
        [Tooltip("Curve for time scale fading to ensure the transition is soft and not abrupt.")]
        public AnimationCurve timeScaleFadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Coroutine timeFadeRoutine;

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

        public void AddStardust(int amount)
        {
            stardustCount += amount;
            // Optionally, we can trigger an event here for UI updates
        }

        public void AddCoins(int amount)
        {
            coinCount += amount;
        }

        /// <summary>
        /// Smoothly fades the time scale to create a peaceful, non-abrupt pause or slowdown effect.
        /// </summary>
        public void SetTimeScaleSoft(float targetTimeScale)
        {
            if (timeFadeRoutine != null)
            {
                StopCoroutine(timeFadeRoutine);
            }
            timeFadeRoutine = StartCoroutine(FadeTimeScaleRoutine(targetTimeScale));
        }

        private IEnumerator FadeTimeScaleRoutine(float target)
        {
            float start = Time.timeScale;
            float elapsed = 0f;

            // Using unscaledDeltaTime to ensure it runs even if Time.timeScale approaches 0
            while (elapsed < timeScaleFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / timeScaleFadeDuration;
                
                float curveValue = timeScaleFadeCurve.Evaluate(t);
                Time.timeScale = Mathf.Lerp(start, target, curveValue);
                
                yield return null;
            }

            Time.timeScale = target;
            timeFadeRoutine = null;
        }
    }
}
