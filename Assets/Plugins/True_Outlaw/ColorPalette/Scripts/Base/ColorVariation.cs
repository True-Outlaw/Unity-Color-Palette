using System;
using UnityEngine;

namespace TrueOutlaw.ColorPalette
{
    [Serializable]
    public class ColorVariation
    {
        [SerializeField]
        public Palette Palette;
        [SerializeField]
        public Color Color;

        public ColorVariation(Palette palette, Color color)
        {
            Palette = palette;
            Color = color;
        }
    }
}