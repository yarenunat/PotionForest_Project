using UnityEngine;
using UnityEngine.UI;
using PotionForest.Core;

namespace PotionForest.UI
{
    /// <summary>
    /// Attaching this script to a UI element (like a Button or Image) 
    /// will automatically apply the chosen cozy color from the UIThemeManager.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIThemeApplier : MonoBehaviour
    {
        public enum ThemeColorType 
        { 
            SoftPink, 
            MintGreen, 
            CreamyYellow, 
            Lavender 
        }

        [Tooltip("Select which cozy color should be applied to this UI element.")]
        public ThemeColorType colorType;

        private void Start()
        {
            ApplyTheme();
        }

        /// <summary>
        /// Reads the UIThemeManager Instance and sets the Image color accordingly.
        /// </summary>
        public void ApplyTheme()
        {
            if (UIThemeManager.Instance == null)
            {
                Debug.LogWarning("[UIThemeApplier] UIThemeManager instance not found. Make sure it exists in the scene.");
                return;
            }

            Image img = GetComponent<Image>();

            switch (colorType)
            {
                case ThemeColorType.SoftPink:
                    img.color = UIThemeManager.Instance.softPink;
                    break;
                case ThemeColorType.MintGreen:
                    img.color = UIThemeManager.Instance.mintGreen;
                    break;
                case ThemeColorType.CreamyYellow:
                    img.color = UIThemeManager.Instance.creamyYellow;
                    break;
                case ThemeColorType.Lavender:
                    img.color = UIThemeManager.Instance.lavender;
                    break;
            }
        }
    }
}
