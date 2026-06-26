using UnityEngine;

namespace PotionForest.Data
{
    [CreateAssetMenu(fileName = "NewPotionData", menuName = "PotionForest/Potion Data")]
    public class PotionData : ScriptableObject
    {
        [Header("General Information")]
        public string potionName;
        
        [TextArea]
        public string description;

        [Header("Economic Values")]
        public int stardustValue;
        public float craftingTimeInSeconds;

        [Header("Visual Aesthetics")]
        [Tooltip("The hex color code for this potion, e.g., #FFD1DC")]
        public string hexColorCode = "#FFD1DC";
        
        [Tooltip("The soft, cozy icon for the UI and Cauldron.")]
        public Sprite potionIcon;

        [Tooltip("Optional particle effect or prefab for the potion completion.")]
        public GameObject potionPrefab;

        /// <summary>
        /// Retrieves the parsed Color based on the hexColorCode. 
        /// Returns white if the parse fails.
        /// </summary>
        public Color GetPotionColor()
        {
            if (ColorUtility.TryParseHtmlString(hexColorCode, out Color color))
            {
                return color;
            }
            Debug.LogWarning($"Could not parse hex color '{hexColorCode}' for potion {potionName}. Using white instead.");
            return Color.white;
        }
    }
}
