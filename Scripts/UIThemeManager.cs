using UnityEngine;

namespace PotionForest.Core
{
    /// <summary>
    /// Manages the cozy aesthetic of the game by providing standard color palettes
    /// and animation curves for UI and in-game elements.
    /// </summary>
    public class UIThemeManager : MonoBehaviour
    {
        public static UIThemeManager Instance { get; private set; }

        [Header("Cozy Color Palette")]
        [Tooltip("Soft Pink - #FFD1DC")]
        public Color softPink = new Color(1f, 0.82f, 0.86f);
        
        [Tooltip("Mint Green - #98FF98")]
        public Color mintGreen = new Color(0.6f, 1f, 0.6f);
        
        [Tooltip("Creamy Yellow - #FFFDD0")]
        public Color creamyYellow = new Color(1f, 0.99f, 0.82f);
        
        [Tooltip("Lavender - #E6E6FA")]
        public Color lavender = new Color(0.9f, 0.9f, 0.98f);
        
        [Tooltip("Soft shadow to avoid sharp black lines")]
        public Color softShadow = new Color(0.2f, 0.2f, 0.2f, 0.15f);

        [Header("Animation Curves (Cozy Feel)")]
        [Tooltip("A smooth, soft ease-in-out curve for gentle UI transitions.")]
        public AnimationCurve cozyEaseInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("A gentle bounce/pop effect for playful interactions (e.g. collecting items).")]
        public AnimationCurve softPopCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.6f, 1.1f),
            new Keyframe(1f, 1f)
        );

        [Header("Shape Properties")]
        [Tooltip("Provides standard rounded corner radius for UI elements.")]
        public float defaultCornerRadius = 24f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
