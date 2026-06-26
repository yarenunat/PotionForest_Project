using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using PotionForest.Core;

namespace PotionForest.UI
{
    /// <summary>
    /// Handles smooth fading between scenes to maintain a peaceful, unbroken atmosphere.
    /// </summary>
    public class CozySceneManager : MonoBehaviour
    {
        [Header("Transition Visuals")]
        [Tooltip("A full-screen Image covering the canvas, used for fading in and out.")]
        public Image fadeOverlay;
        
        [Tooltip("How long the screen takes to fade.")]
        public float fadeDuration = 1.5f;

        private void Start()
        {
            // When the scene loads, fade the black/colored overlay out so the scene becomes visible softly
            if (fadeOverlay != null)
            {
                StartCoroutine(FadeRoutine(1f, 0f));
            }
        }

        /// <summary>
        /// Called by a UI Button (like 'Start Game'). Plays a pop sound and fades out to the next scene.
        /// </summary>
        public void StartGame(string sceneName)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("pop");
            }
            StartCoroutine(TransitionToSceneRoutine(sceneName));
        }

        private IEnumerator TransitionToSceneRoutine(string sceneName)
        {
            // Fade the overlay to fully opaque
            if (fadeOverlay != null)
            {
                yield return StartCoroutine(FadeRoutine(0f, 1f));
            }

            // Once the screen is hidden behind the overlay, load the next scene
            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
        {
            fadeOverlay.gameObject.SetActive(true);
            
            // Try to use a cozy color (like Soft Pink) from the ThemeManager instead of harsh black
            if (UIThemeManager.Instance != null)
            {
                Color baseColor = UIThemeManager.Instance.softPink;
                fadeOverlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, startAlpha);
            }

            float elapsed = 0f;
            Color color = fadeOverlay.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                
                // Use cozy curve for smooth fade if available
                float curveVal = t;
                if (UIThemeManager.Instance != null)
                {
                    curveVal = UIThemeManager.Instance.cozyEaseInOut.Evaluate(t);
                }

                color.a = Mathf.Lerp(startAlpha, targetAlpha, curveVal);
                fadeOverlay.color = color;
                
                yield return null;
            }

            color.a = targetAlpha;
            fadeOverlay.color = color;

            // Hide the overlay if it's completely transparent so it doesn't block raycasts
            if (targetAlpha == 0f)
            {
                fadeOverlay.gameObject.SetActive(false);
            }
        }
    }
}
