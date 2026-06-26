using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PotionForest.Core;
using PotionForest.Data;

namespace PotionForest.UI
{
    /// <summary>
    /// Creates a cozy screen-edge glow when a potion completes, acting as a soft notification.
    /// Attach this to a UI Canvas Image that covers the screen edges.
    /// </summary>
    public class NotificationGlow : MonoBehaviour
    {
        public Image glowEdgeImage;
        public PotionSystem potionSystem;
        public float glowDuration = 2f;

        private void OnEnable()
        {
            if (potionSystem != null) 
            {
                potionSystem.OnBrewingCompleted += TriggerGlow;
            }
        }

        private void OnDisable()
        {
            if (potionSystem != null) 
            {
                potionSystem.OnBrewingCompleted -= TriggerGlow;
            }
        }

        private void TriggerGlow(PotionData potion)
        {
            if (glowEdgeImage != null)
            {
                StartCoroutine(GlowRoutine(potion));
            }
        }

        private IEnumerator GlowRoutine(PotionData potion)
        {
            // İksirin rengini alıp ekranın köşelerindeki glow efektine uygula
            Color baseColor = potion.GetPotionColor();
            baseColor.a = 0f;
            glowEdgeImage.color = baseColor;
            glowEdgeImage.gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < glowDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / glowDuration;
                
                // Sine dalgası ile yumuşakça belirip kaybolma
                baseColor.a = Mathf.Sin(t * Mathf.PI) * 0.4f; // Maksimum %40 opaklık (göz yormaması için)
                glowEdgeImage.color = baseColor;
                
                yield return null;
            }

            glowEdgeImage.gameObject.SetActive(false);
        }
    }
}
