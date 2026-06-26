using UnityEngine;
using System.Collections;
using PotionForest.Core;
using PotionForest.Data;

namespace PotionForest.Gameplay
{
    /// <summary>
    /// Represents the cauldron object in the game world. 
    /// Handles user clicks to start brewing, pop animations, and particle effects.
    /// </summary>
    public class Cauldron : MonoBehaviour
    {
        [Header("Cauldron Settings")]
        public PotionData targetPotion;
        public PotionSystem potionSystem;

        [Header("Visual & Effects")]
        [Tooltip("The actual pot visual that bounces.")]
        public Transform cauldronVisual;
        
        [Tooltip("The potion that pops up.")]
        public Transform potionVisualTransform;
        
        [Tooltip("The Particle System attached to the cauldron for stardust effects.")]
        public ParticleSystem starDustParticles; 

        private void OnEnable()
        {
            if (potionSystem != null)
            {
                potionSystem.OnBrewingStarted += HandleBrewingStarted;
                potionSystem.OnBrewingCompleted += HandleBrewingCompleted;
            }
        }

        private void OnDisable()
        {
            if (potionSystem != null)
            {
                potionSystem.OnBrewingStarted -= HandleBrewingStarted;
                potionSystem.OnBrewingCompleted -= HandleBrewingCompleted;
            }
        }

        private void OnMouseDown()
        {
            if (targetPotion == null || potionSystem == null) return;

            if (InventoryManager.Instance.HasIngredient("glowing_mushroom", 1))
            {
                InventoryManager.Instance.ConsumeIngredient("glowing_mushroom", 1);
                potionSystem.StartBrewing(targetPotion);
                
                if (potionVisualTransform != null)
                {
                    potionVisualTransform.localScale = Vector3.zero;
                    
                    var draggable = potionVisualTransform.GetComponent<DraggablePotion>();
                    if (draggable != null) draggable.enabled = false;
                }
            }
            else
            {
                Debug.Log($"[Cauldron] Not enough glowing_mushroom to brew {targetPotion.potionName}!");
            }
        }

        private void HandleBrewingStarted(PotionData data)
        {
            if (data == targetPotion)
            {
                // Bubble sesi oynat (İksir kaynıyor)
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("bubble");
                }
            }
        }

        private void HandleBrewingCompleted(PotionData data)
        {
            if (data == targetPotion)
            {
                // Parçacık efektini çalıştır ve rengini UIThemeManager'dan al
                if (starDustParticles != null && UIThemeManager.Instance != null)
                {
                    var mainModule = starDustParticles.main;
                    // Yıldız tozu için Creamy Yellow rengini kullanıyoruz
                    mainModule.startColor = UIThemeManager.Instance.creamyYellow;
                    starDustParticles.Play();
                }

                // Tatlı bir pop sesi oynat
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("pop");
                }

                StartCoroutine(PopAnimation());
                
                if (cauldronVisual != null)
                {
                    StartCoroutine(CauldronBounceAnimation());
                }
            }
        }

        private IEnumerator CauldronBounceAnimation()
        {
            float elapsed = 0f;
            float duration = 0.4f;
            Vector3 startScale = cauldronVisual.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Squish and squash efektini yaratmak için sinüs dalgası
                float bounce = Mathf.Sin(t * Mathf.PI) * 0.15f; 
                cauldronVisual.localScale = startScale + new Vector3(bounce, -bounce, 0f); 
                
                yield return null;
            }
            
            cauldronVisual.localScale = startScale;
        }

        private IEnumerator PopAnimation()
        {
            if (potionVisualTransform == null) yield break;

            float elapsed = 0f;
            float duration = 0.6f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                float curveValue = UIThemeManager.Instance.softPopCurve.Evaluate(t);
                potionVisualTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);
                
                yield return null;
            }

            potionVisualTransform.localScale = endScale;

            var draggable = potionVisualTransform.GetComponent<DraggablePotion>();
            if (draggable != null) draggable.enabled = true;
        }
    }
}
