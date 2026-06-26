using UnityEngine;
using PotionForest.Core;
using PotionForest.Data;

namespace PotionForest.Gameplay
{
    /// <summary>
    /// Represents a customer that waits for a potion.
    /// When the player drags and drops the correct potion on them, they pay Gold and leave.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Customer : MonoBehaviour
    {
        [Header("Customer Settings")]
        [Tooltip("The potion data this customer is waiting for.")]
        public PotionData desiredPotion;
        
        [Tooltip("How much Gold the player receives upon successful delivery.")]
        public int rewardGold = 15;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // İksir sürüklenip müşterinin üzerine (Collider'ına) bırakıldığında tetiklenir.
            if (collision.CompareTag("Potion"))
            {
                // Sürüklenen objenin içinde Potion bilgilerini tutan bir script varsa kontrol edilebilir
                // Şimdilik basitçe tag üzerinden doğru kabul ediyoruz veya DraggablePotion içerisindeki data'ya bakabiliriz.
                
                DeliverPotion();

                // İksir görselini/objesini yok et (veya gizle)
                Destroy(collision.gameObject);
            }
        }

        private void DeliverPotion()
        {
            // Oyuncuya para kazandır
            GameManager.Instance.AddCoins(rewardGold);
            
            if (desiredPotion != null)
            {
                Debug.Log($"[Customer] Received {desiredPotion.potionName} and paid {rewardGold} coins!");
            }
            else
            {
                Debug.Log($"[Customer] Received a potion and paid {rewardGold} coins!");
            }
            
            // Müşteri ayrılsın
            Leave();
        }

        private void Leave()
        {
            // TODO: Buraya cozy bir yürüme, fade-out veya 'mutlu emoji' pop-up animasyonu eklenebilir.
            Destroy(gameObject);
        }
    }
}
