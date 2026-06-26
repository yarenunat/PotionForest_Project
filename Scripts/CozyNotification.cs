using UnityEngine;
using System.Collections;
using PotionForest.Core;
using PotionForest.Data;
using PotionForest.UI;

namespace PotionForest.Gameplay
{
    /// <summary>
    /// Bir iksir tamamlandığında kazanın üzerinde ufak bir yıldız ikonunun 
    /// belirip zıplamasını (bounce) sağlayan script.
    /// </summary>
    public class CozyNotification : MonoBehaviour
    {
        [Header("Referanslar")]
        public PotionSystem potionSystem;
        
        [Tooltip("Kazanın üstünde duran Yıldız ikonu (SpriteRenderer)")]
        public SpriteRenderer starIcon; 
        
        [Tooltip("Yıldızın UIThemeManager'dan çekeceği renk.")]
        public UIThemeApplier.ThemeColorType iconColorType = UIThemeApplier.ThemeColorType.CreamyYellow;

        [Header("Animasyon")]
        public float bounceDuration = 2.5f;
        
        private Vector3 startPosition;

        private void Start()
        {
            if (starIcon != null)
            {
                startPosition = starIcon.transform.localPosition;
                starIcon.gameObject.SetActive(false); // Başlangıçta gizli
            }
        }

        private void OnEnable()
        {
            if (potionSystem != null) potionSystem.OnBrewingCompleted += ShowNotification;
        }

        private void OnDisable()
        {
            if (potionSystem != null) potionSystem.OnBrewingCompleted -= ShowNotification;
        }

        private void ShowNotification(PotionData data)
        {
            if (starIcon != null)
            {
                // UIThemeManager'dan rengi çek ve yıldıza uygula
                if (UIThemeManager.Instance != null)
                {
                    switch (iconColorType)
                    {
                        case UIThemeApplier.ThemeColorType.SoftPink: starIcon.color = UIThemeManager.Instance.softPink; break;
                        case UIThemeApplier.ThemeColorType.MintGreen: starIcon.color = UIThemeManager.Instance.mintGreen; break;
                        case UIThemeApplier.ThemeColorType.CreamyYellow: starIcon.color = UIThemeManager.Instance.creamyYellow; break;
                        case UIThemeApplier.ThemeColorType.Lavender: starIcon.color = UIThemeManager.Instance.lavender; break;
                    }
                }
                
                StartCoroutine(BounceRoutine());
            }
        }

        private IEnumerator BounceRoutine()
        {
            starIcon.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < bounceDuration)
            {
                elapsed += Time.deltaTime;
                
                // Sinüs ile hızlı ve tatlı bir zıplama (Saniyede 2 kez zıplar: PI * 4)
                float bounce = Mathf.Sin(elapsed * Mathf.PI * 4f) * 0.25f; 
                
                // Zıplarken sıfırın altına inmemesi için Abs kullanılabilir
                bounce = Mathf.Abs(bounce);

                starIcon.transform.localPosition = startPosition + new Vector3(0, bounce, 0);
                
                yield return null;
            }

            // Animasyon bitince başlangıç pozisyonuna dön ve gizlen
            starIcon.transform.localPosition = startPosition;
            starIcon.gameObject.SetActive(false);
        }
    }
}
