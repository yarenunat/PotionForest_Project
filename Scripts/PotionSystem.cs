using UnityEngine;
using System.Collections;
using System;
using PotionForest.Data;

namespace PotionForest.Core
{
    /// <summary>
    /// Manages the brewing process of potions, reading PotionData and 
    /// handling soft visual transitions during the crafting phase.
    /// </summary>
    public class PotionSystem : MonoBehaviour
    {
        [Header("Cauldron Visuals")]
        [Tooltip("The sprite renderer inside the cauldron where the potion appears.")]
        public SpriteRenderer potionVisualRenderer;
        
        [Header("Animation Settings")]
        [Tooltip("Curve for the soft, cozy growth of the potion when it finishes brewing.")]
        public AnimationCurve brewingGrowthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        public float growthAnimationDuration = 1.0f;

        // Events
        public event Action<PotionData> OnBrewingStarted;
        public event Action<PotionData> OnBrewingCompleted;

        private bool isBrewing = false;

        /// <summary>
        /// Begins the brewing process for a specific potion.
        /// </summary>
        public void StartBrewing(PotionData potionData)
        {
            if (isBrewing)
            {
                Debug.LogWarning("Cauldron is already brewing something.");
                return;
            }

            StartCoroutine(BrewPotionRoutine(potionData));
        }

        private IEnumerator BrewPotionRoutine(PotionData potionData)
        {
            isBrewing = true;
            OnBrewingStarted?.Invoke(potionData);

            // Prepare the visual representation but keep it hidden/scaled down
            if (potionVisualRenderer != null)
            {
                potionVisualRenderer.transform.localScale = Vector3.zero;
                potionVisualRenderer.color = potionData.GetPotionColor();
                potionVisualRenderer.sprite = potionData.potionIcon;
            }

            Debug.Log($"Started brewing {potionData.potionName}. Time: {potionData.craftingTimeInSeconds}s");

            // Wait for the crafting time duration
            yield return new WaitForSeconds(potionData.craftingTimeInSeconds);

            // Brewing time finished, play the soft growth animation to reveal the potion
            yield return StartCoroutine(PlaySoftGrowthAnimation());

            isBrewing = false;
            OnBrewingCompleted?.Invoke(potionData);
            
            // Add to collection
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnlockPotion(potionData.potionName);
            }
            
            Debug.Log($"Finished brewing {potionData.potionName}!");
        }

        /// <summary>
        /// Plays a smooth scaling animation using the designated curve to pop the potion into existence.
        /// </summary>
        private IEnumerator PlaySoftGrowthAnimation()
        {
            if (potionVisualRenderer == null) yield break;

            float elapsed = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;

            while (elapsed < growthAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / growthAnimationDuration;
                
                float curveValue = brewingGrowthCurve.Evaluate(t);
                potionVisualRenderer.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);
                
                yield return null;
            }

            potionVisualRenderer.transform.localScale = endScale;
        }
    }
}
