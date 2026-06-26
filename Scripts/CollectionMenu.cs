using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PotionForest.Core;
using PotionForest.Data;

namespace PotionForest.UI
{
    /// <summary>
    /// A "Stamp Book" style collection menu to view discovered potions.
    /// Fills silhouettes with real icons when unlocked.
    /// </summary>
    public class CollectionMenu : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject stampPrefab; // A prefab that has an Image component
        public Transform stampContainer; // e.g. a GridLayoutGroup

        [Header("Data")]
        public List<PotionData> allAvailablePotions;

        private Dictionary<string, Image> instantiatedStamps = new Dictionary<string, Image>();

        private void Start()
        {
            InitializeAlbum();
        }

        private void InitializeAlbum()
        {
            foreach (var potion in allAvailablePotions)
            {
                GameObject newStamp = Instantiate(stampPrefab, stampContainer);
                Image stampImage = newStamp.GetComponent<Image>();
                stampImage.sprite = potion.potionIcon;
                
                // Başlangıçta hepsi gölge/silüet şeklinde (Keşfedilmemiş)
                stampImage.color = new Color(0.15f, 0.15f, 0.15f, 0.6f); 
                
                instantiatedStamps.Add(potion.potionName, stampImage);
            }
            RefreshAlbum();
        }

        public void RefreshAlbum()
        {
            if (GameManager.Instance == null) return;

            foreach (var unlockedName in GameManager.Instance.unlockedPotions)
            {
                if (instantiatedStamps.TryGetValue(unlockedName, out Image img))
                {
                    img.color = Color.white; // Orijinal, renkli hale geri döndür
                }
            }
        }

        /// <summary>
        /// Menüyü açarken çağrılır.
        /// </summary>
        public void OpenMenu()
        {
            gameObject.SetActive(true);
            RefreshAlbum();
            
            if (AudioManager.Instance != null)
            {
                // Kitap açılma/yaprak hışırtısı hissi verecek yumuşak bir ses eklenebilir
                AudioManager.Instance.PlaySFX("pop"); 
            }
        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
