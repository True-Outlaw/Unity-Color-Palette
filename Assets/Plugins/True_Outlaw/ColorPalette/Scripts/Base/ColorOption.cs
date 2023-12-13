using System.Collections.Generic;
using UnityEngine;

namespace TrueOutlaw.ColorPalette
{
    [CreateAssetMenu(menuName = "True Outlaw/Color Palette/Color")]
    public class ColorOption : ScriptableObject
    {
        public List<ColorVariation> ColorVariations = new();
    }
}