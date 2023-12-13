using UnityEngine.UI;

namespace TrueOutlaw.ColorPalette
{
    public class ImageColorComponent : ColorComponent<Image>
    {
        protected override void ApplyColor()
        {
            base.ApplyColor();
            
            Target.color = ActiveColor;
        }
    }
}