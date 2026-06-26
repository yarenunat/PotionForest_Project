using UnityEngine;
using System.Collections;
using PotionForest.Core;

namespace PotionForest.Gameplay
{
    /// <summary>
    /// Karakterlerin (Müşteriler vs.) sabit durmasını engellemek için 
    /// UIThemeManager'ın yumuşak curve'lerini kullanan Idle animasyon sistemi.
    /// </summary>
    public class IdleAnimator : MonoBehaviour
    {
        [Header("Idle Ayarları")]
        public bool enableIdleAnimations = true;
        public float minWaitTime = 2f;
        public float maxWaitTime = 6f;

        private Vector3 startScale;
        private Quaternion startRotation;
        private Vector3 startPosition;

        private void Start()
        {
            startScale = transform.localScale;
            startRotation = transform.localRotation;
            startPosition = transform.localPosition;
            
            if (enableIdleAnimations)
            {
                StartCoroutine(IdleRoutine());
            }
        }

        private IEnumerator IdleRoutine()
        {
            while (true)
            {
                // Rastgele bir süre bekle
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
                
                // 3 farklı hareketten birini rastgele seç
                int randomAction = Random.Range(0, 3);
                
                switch (randomAction)
                {
                    case 0: // Küçük zıplama
                        yield return StartCoroutine(JumpAnimation());
                        break;
                    case 1: // Sağa/sola dönme (Wiggle)
                        yield return StartCoroutine(TurnAnimation());
                        break;
                    case 2: // İç çekme (Squish/Squash)
                        yield return StartCoroutine(SighAnimation());
                        break;
                }
            }
        }

        private IEnumerator JumpAnimation()
        {
            float duration = 0.4f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Sinüs ile yumuşak zıplama yayı
                float height = Mathf.Sin(t * Mathf.PI) * 0.4f; 
                transform.localPosition = startPosition + new Vector3(0, height, 0);
                
                yield return null;
            }
            transform.localPosition = startPosition;
        }

        private IEnumerator TurnAnimation()
        {
            float duration = 0.6f;
            float elapsed = 0f;
            
            // Sağa mı sola mı dönecek?
            float angle = Random.value > 0.5f ? 15f : -15f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                float currentAngle = Mathf.Sin(t * Mathf.PI) * angle;
                transform.localRotation = startRotation * Quaternion.Euler(0, 0, currentAngle);
                
                yield return null;
            }
            transform.localRotation = startRotation;
        }

        private IEnumerator SighAnimation()
        {
            float duration = 0.7f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // AnimationCurve ile yumuşatılabilir, şimdilik sinüs işimizi harika görüyor.
                // Yana genişleyip boydan kısalma (İç çekiş)
                float squish = Mathf.Sin(t * Mathf.PI) * 0.1f;
                transform.localScale = startScale + new Vector3(squish, -squish, 0);
                
                yield return null;
            }
            transform.localScale = startScale;
        }
    }
}
