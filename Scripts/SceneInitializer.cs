using UnityEngine;
using UnityEngine.UI;
using PotionForest.Core;
using PotionForest.Gameplay;
using PotionForest.UI;

namespace PotionForest.Setup
{
    /// <summary>
    /// Sahnede sadece bu scripti taşıyan boş bir GameObject bırakıldığında,
    /// tüm oyun sistemini (GameManagers, Objeler, UI) sıfırdan inşa eder ve birbirine bağlar.
    /// Potion Forest: Cozy Brew projesi için otomatik kurulum yöneticisidir.
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        [Header("Zorunlu Veriler (Data)")]
        [Tooltip("Projendeki PotionData (.asset) dosyalarını buraya sürükle.")]
        public PotionData[] availablePotions;

        private void Start()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            Debug.Log("SceneInitializer: Potion Forest dünyası yaratılıyor...");

            // --- 1. GAME MANAGERS (Singleton Kurulumu) ---
            GameObject managersObj = new GameObject("GameManagers");
            var gameManager = managersObj.AddComponent<GameManager>();
            var inventoryManager = managersObj.AddComponent<InventoryManager>();
            var potionSystem = managersObj.AddComponent<PotionSystem>();
            var uiThemeManager = managersObj.AddComponent<UIThemeManager>();
            var audioManager = managersObj.AddComponent<AudioManager>();

            // --- 2. CAULDRON (Kazan) KURULUMU ---
            GameObject cauldronObj = new GameObject("Cauldron");
            cauldronObj.transform.position = new Vector3(0, -1.5f, 0);
            var cauldronCollider = cauldronObj.AddComponent<BoxCollider2D>();
            cauldronCollider.size = new Vector2(2f, 2f); // Kazana tıklayabilmek için
            
            var cauldron = cauldronObj.AddComponent<Cauldron>();
            var cozyNotification = cauldronObj.AddComponent<CozyNotification>();

            // Kazanın Görsel Alt Objeleri
            GameObject cauldronVisual = new GameObject("CauldronVisual");
            cauldronVisual.transform.SetParent(cauldronObj.transform);
            cauldronVisual.AddComponent<SpriteRenderer>();

            GameObject potionVisual = new GameObject("PotionVisual");
            potionVisual.transform.SetParent(cauldronObj.transform);
            potionVisual.AddComponent<SpriteRenderer>();
            var potionCollider = potionVisual.AddComponent<BoxCollider2D>();
            potionCollider.isTrigger = true;
            potionVisual.tag = "Potion"; // Customer etkileşimi için gerekli
            var draggablePotion = potionVisual.AddComponent<DraggablePotion>();
            draggablePotion.enabled = false;

            GameObject starIcon = new GameObject("StarIcon");
            starIcon.transform.SetParent(cauldronObj.transform);
            starIcon.transform.localPosition = new Vector3(0, 2f, 0);
            var starRenderer = starIcon.AddComponent<SpriteRenderer>();

            // Cauldron Referans Bağlamaları
            cauldron.potionSystem = potionSystem;
            cauldron.cauldronVisual = cauldronVisual.transform;
            cauldron.potionVisualTransform = potionVisual.transform;
            
            if (availablePotions != null && availablePotions.Length > 0)
            {
                cauldron.targetPotion = availablePotions[0]; // Test için ilk iksiri ata
            }

            cozyNotification.potionSystem = potionSystem;
            cozyNotification.starIcon = starRenderer;

            // --- 3. CUSTOMER (Müşteri) KURULUMU ---
            GameObject customerObj = new GameObject("Customer");
            customerObj.transform.position = new Vector3(4f, -1.5f, 0);
            customerObj.AddComponent<SpriteRenderer>();
            var customerCollider = customerObj.AddComponent<BoxCollider2D>();
            customerCollider.isTrigger = true;
            customerCollider.size = new Vector2(2f, 3f);
            
            customerObj.AddComponent<Customer>();
            customerObj.AddComponent<IdleAnimator>(); // Canlılık katsın

            // --- 4. UI (Canvas ve CollectionBook) KURULUMU ---
            GameObject canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Koleksiyon Defteri Paneli
            GameObject collectionPanel = new GameObject("CollectionBookPanel");
            collectionPanel.transform.SetParent(canvasObj.transform, false);
            var panelRect = collectionPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero; // Tam ekran
            collectionPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Yarı saydam arka plan

            // Grid Container (İkonların dizileceği yer)
            GameObject gridObj = new GameObject("GridContainer");
            gridObj.transform.SetParent(collectionPanel.transform, false);
            var gridRect = gridObj.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.1f, 0.1f);
            gridRect.anchorMax = new Vector2(0.9f, 0.9f);
            gridRect.sizeDelta = Vector2.zero;
            var gridLayout = gridObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(120, 120);
            gridLayout.spacing = new Vector2(25, 25);

            // Stamp Prefab (Sanal olarak sahnede gizli yaratıyoruz)
            GameObject stampPrefab = new GameObject("StampPrefab");
            stampPrefab.AddComponent<Image>();
            stampPrefab.SetActive(false); // Ekranda görünmesin, sadece şablon

            // CollectionBook Scriptini Ekle ve Bağla
            var collectionBook = collectionPanel.AddComponent<CollectionBook>();
            collectionBook.potionSystem = potionSystem; // Data Binding!
            collectionBook.bookPageContainer = gridObj.transform;
            collectionBook.stampPrefab = stampPrefab;
            if (availablePotions != null)
            {
                collectionBook.allPotions = new System.Collections.Generic.List<PotionData>(availablePotions);
            }

            collectionPanel.SetActive(false); // Başlangıçta kapalı olsun

            // --- 5. UI BUTONLARI (Aç / Kapat) ---
            
            // Defteri Açma Butonu
            GameObject openBtnObj = new GameObject("OpenBookButton");
            openBtnObj.transform.SetParent(canvasObj.transform, false);
            var openRect = openBtnObj.AddComponent<RectTransform>();
            openRect.anchorMin = new Vector2(1, 1);
            openRect.anchorMax = new Vector2(1, 1);
            openRect.anchoredPosition = new Vector2(-120, -60);
            openRect.sizeDelta = new Vector2(150, 50);
            openBtnObj.AddComponent<Image>().color = new Color(0.8f, 0.6f, 0.3f); // Ahşap/Kağıt rengi
            var openBtn = openBtnObj.AddComponent<Button>();
            openBtn.onClick.AddListener(() => collectionBook.OpenCollection());

            // Defteri Kapatma Butonu
            GameObject closeBtnObj = new GameObject("CloseBookButton");
            closeBtnObj.transform.SetParent(collectionPanel.transform, false);
            var closeRect = closeBtnObj.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.anchoredPosition = new Vector2(-60, -60);
            closeRect.sizeDelta = new Vector2(60, 60);
            closeBtnObj.AddComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f); // Kırmızı
            var closeBtn = closeBtnObj.AddComponent<Button>();
            closeBtn.onClick.AddListener(() => collectionBook.CloseCollection());

            Debug.Log("SceneInitializer: Tüm sistemler başarıyla kuruldu ve birbirine bağlandı!");
        }
    }
}
