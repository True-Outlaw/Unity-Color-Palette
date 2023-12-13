using UnityEngine;

namespace TrueOutlaw.ColorPalette
{
    public class ColorComponentBase : MonoBehaviour
    {
        [SerializeField] protected ColorOption _colorOption;
        
        protected Color ActiveColor;
        
        public virtual void GetColorValue()
        {
            
        }

        protected virtual void ApplyColor()
        {
            
        }
    }
}