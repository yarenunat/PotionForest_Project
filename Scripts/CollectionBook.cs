using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PotionForest.Core;
using PotionForest.Data;

namespace PotionForest.UI
{
    /// <summary>
    /// Oyuncunun keşfettiği iksirleri gösteren koleksiyon defteri scripti.
    /// ScriptableObject listesi kullanır ve GameManager üzerinden açılmış iksirleri kontrol eder.
    /// </summary>
    public class CollectionBook : MonoBehaviour
    {
        [Header("UI Kurulumu")]
        [Tooltip("İçinde Image olan minik mühür/ikon Prefab'i")]
        public GameObject stampPrefab; 
        
        [Tooltip("İkonların dizileceği UI Paneli (Örn: GridLayoutGroup içeren obje)")]
        public Transform bookPageContainer; 

        [Header("İksir Veritabanı")]
        [Tooltip("Oyunda var olan tüm PotionData dosyalarını buraya sürükle.")]
        public List<PotionData> allPotions;

        [Header("Event Bağlantıları")]
        [Tooltip("Koleksiyonun anında güncellenmesi için PotionSystem referansını verin.")]
        public PotionSystem potionSystem;

        private Dictionary<string, Image> potionIcons = new Dictionary<string, Image>();

        private void Start()
        {
            InitializeBook();
        }

        private void OnEnable()
        {
            if (potionSystem != null)
            {
                potionSystem.OnBrewingCompleted += HandlePotionBrewed;
            }
        }

        private void OnDisable()
        {
            if (potionSystem != null)
            {
                potionSystem.OnBrewingCompleted -= HandlePotionBrewed;
            }
        }

        private void InitializeBook()
        {
            foreach (var potion in allPotions)
            {
                // Her iksir için sayfaya bir ikon yarat
                GameObject stampObj = Instantiate(stampPrefab, bookPageContainer);
                Image img = stampObj.GetComponent<Image>();
                img.sprite = potion.potionIcon;
                
                // Başlangıçta hepsi kilitli (Gri bir silüet olarak gösterilir)
                img.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);
                
                potionIcons.Add(potion.potionName, img);
            }
            
            // Eğer oyun başlarken zaten yapılmış iksirler varsa onları aç
            UpdateBookVisually();
        }

        /// <summary>
        /// GameManager'dan (Singleton) kilitli olmayan iksirleri okur ve görsellerini aydınlatır.
        /// </summary>
        public void UpdateBookVisually()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[CollectionBook] GameManager.Instance is null! Veriler okunamadı.");
                return;
            }

            foreach (var potionName in GameManager.Instance.unlockedPotions)
            {
                UnlockPotionUI(potionName);
            }
        }

        /// <summary>
        /// PotionSystem'den gelen tetikleyici (Data Binding). Yeni iksir yapıldığında defterdeki silüeti anında renklendirir.
        /// </summary>
        private void HandlePotionBrewed(PotionData potionData)
        {
            UnlockPotionUI(potionData.potionName);
        }

        private void UnlockPotionUI(string potionName)
        {
            if (potionIcons.TryGetValue(potionName, out Image img))
            {
                img.color = Color.white; // Orijinal rengine dön
            }
        }

        public void OpenCollection()
        {
            gameObject.SetActive(true);
            UpdateBookVisually();
            
            // Defterin açılma sesini AudioManager (Singleton) üzerinden tetikle
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("pop");
            }
        }

        public void CloseCollection()
        {
            gameObject.SetActive(false);
        }
    }
}
