using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace TrueOutlaw.ColorPalette
{
    [CreateAssetMenu(menuName = "True Outlaw/Color Palette/Palette")]
    public class Palette : ScriptableObject
    {
        public bool IsActive;
        public List<ColorOption> ColorOptions = new();
    }
}