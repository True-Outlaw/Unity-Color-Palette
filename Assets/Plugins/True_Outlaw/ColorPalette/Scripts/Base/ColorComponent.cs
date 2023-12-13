using UnityEngine;

namespace TrueOutlaw.ColorPalette
{
    public class ColorComponent<T> : ColorComponentBase
    {
        protected T Target;

        private void OnValidate()
        {
            GetColorValue();
        }

        public override void GetColorValue()
        {
            if (_colorOption == null)
            {
                return;
            }

            Target ??= GetComponent<T>();

            foreach (var colorVariation in _colorOption.ColorVariations)
            {
                if (!colorVariation.Palette.IsActive)
                {
                    continue;
                }

                ActiveColor = colorVariation.Color;
                break;
            }
            
            ApplyColor();
        }

        protected override void ApplyColor()
        {
            if (Target == null)
            {
                Debug.LogError($"No Target assigned in ColorComponent {gameObject.name}");
                return;
            }
        }
    }
}