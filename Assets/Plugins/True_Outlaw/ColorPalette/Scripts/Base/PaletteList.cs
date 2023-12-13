using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace TrueOutlaw.ColorPalette
{
    [CreateAssetMenu(menuName = "True Outlaw/Color Palette/Palette List", order = 1)]
    public class PaletteList : ScriptableObject
    {
        public List<Palette> Palettes = new();
    }
}