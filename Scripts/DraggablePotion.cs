using UnityEngine;
using System.Collections;
using PotionForest.Core;

namespace PotionForest.Gameplay
{
    /// <summary>
    /// Makes the potion visual draggable using 2D physics.
    /// Needs a Collider2D (IsTrigger = true recommended) to interact with the mouse and Customer.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DraggablePotion : MonoBehaviour
    {
        private Vector3 offset;
        private bool isDragging = false;
        private Vector3 startPosition;

        private void OnMouseDown()
        {
            if (!enabled) return;

            startPosition = transform.position;
            offset = transform.position - GetMouseWorldPosition();
            isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (!enabled || !isDragging) return;

            transform.position = GetMouseWorldPosition() + offset;
        }

        private void OnMouseUp()
        {
            if (!enabled || !isDragging) return;

            isDragging = false;
            
            // Mouse bırakıldığında eğer bir müşterinin üzerinde değilsek geri eski yerine (kazana) dönsün.
            // Müşterinin üzerinde bıraktıysak Customer scripti OnTriggerEnter2D ile bunu yok edecektir.
            StartCoroutine(SnapBackRoutine());
        }

        private IEnumerator SnapBackRoutine()
        {
            float elapsed = 0f;
            float duration = 0.2f;
            Vector3 releasePosition = transform.position;

            // UIThemeManager'dan varsa popCurve veya easeCurve kullanıp tatlı bir geri dönüş sağlanabilir
            while (elapsed < duration)
            {
                // Obje yok edildiyse (Customer aldıysa) Coroutine'i kır
                if (this == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Basit ve yumuşak bir Lerp
                if (UIThemeManager.Instance != null)
                {
                    float curveValue = UIThemeManager.Instance.cozyEaseInOut.Evaluate(t);
                    transform.position = Vector3.Lerp(releasePosition, startPosition, curveValue);
                }
                else
                {
                    transform.position = Vector3.Lerp(releasePosition, startPosition, t);
                }
                
                yield return null;
            }

            if (this != null)
            {
                transform.position = startPosition;
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = Mathf.Abs(Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }
    }
}
