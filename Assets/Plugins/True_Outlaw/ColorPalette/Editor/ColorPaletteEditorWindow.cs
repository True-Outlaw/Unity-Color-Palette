#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TrueOutlaw.ColorPalette
{
    public class ColorPaletteEditorWindow : EditorWindow
    {
        private PaletteList _paletteList;

        private List<Palette> _palettes = new();
        private int _selectedPaletteIndex = -1;

        private List<ColorOption> _colorOptions = new();

        private Vector2 _scrollPosition;

        private const float ButtonHeight = 30f;
        private const string PalettesFolderPath = "Assets/Plugins/True_Outlaw/ColorPalette/ColorPalettes";
        private const string ColorsFolderPath = "Assets/Plugins/True_Outlaw/ColorPalette/ColorOptions";

        private const string NewPaletteBaseName = "NewPalette";
        private const string NewColorOptionBaseName = "NewColorOption";

        [MenuItem("Tools/True Outlaw/Color Palette")]
        public static void ShowWindow()//
        {
            GetWindow<ColorPaletteEditorWindow>("Color Palette");
        }

        private void OnEnable()
        {   
            // Load or create the PaletteList asset
            string assetPath = "Assets/Plugins/True_Outlaw/ColorPalette/PaletteList.asset";
            if (File.Exists(assetPath))
            {
                _paletteList = AssetDatabase.LoadAssetAtPath<PaletteList>(assetPath);
            }
            else
            {
                if (_paletteList == null)
                {
                    _paletteList = CreateInstance<PaletteList>();
                    
                    // Check if the asset already exists
                    if (!AssetDatabase.Contains(_paletteList))
                    {
                        AssetDatabase.CreateAsset(_paletteList, assetPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        // The asset already exists, so load it
                        _paletteList = AssetDatabase.LoadAssetAtPath<PaletteList>(assetPath);
                    }
                }
            }

            // Ensure the list is populated when the window is opened
            RefreshData();
        }

        private void OnDisable()
        {
            if (_paletteList != null)
            {
                foreach (var palette in _paletteList.Palettes)
                {
                    EditorUtility.SetDirty(palette);

                    foreach (var colorOption in palette.ColorOptions)
                    {
                        EditorUtility.SetDirty(colorOption);
                    }
                }

                // Save the PaletteList asset when the window is disabled
                EditorUtility.SetDirty(_paletteList);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void RefreshData()
        {
            RefreshPalettesList();
            RefreshColorOptionsList();
            CleanUpEmptyFolders();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Flag to check if a palette is clicked
            bool isPaletteClicked = false;

            for (int i = 0; i < _palettes.Count; i++)
            {
                var palette = _palettes[i];
                GUIStyle paletteStyle = GetPaletteStyle(palette);

                EditorGUILayout.BeginVertical(paletteStyle);

                GUILayout.BeginHorizontal();

                // Add a checkbox to the left of the name text field
                EditorGUI.BeginChangeCheck();
                palette.IsActive = EditorGUILayout.Toggle(palette.IsActive, GUILayout.Width(20));
                for (int j = 0; j < _palettes.Count(); j++)
                {
                    if (!palette.IsActive)
                    {
                        continue;
                    }

                    if (palette != _palettes[j])
                    {
                        _palettes[j].IsActive = false;
                    }
                }
                string newPaletteName = EditorGUILayout.TextField(palette.name, GUILayout.Width(140));
                if (EditorGUI.EndChangeCheck())
                {
                    // Rename the palette
                    RenameAsset(palette, newPaletteName);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    DeletePalette(palette);
                    GUIUtility.ExitGUI();
                }

                GUILayout.EndHorizontal();

                // Check if the palette is clicked
                if (Event.current.type == EventType.MouseDown &&
                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    EditorGUI.BeginChangeCheck();
                    _selectedPaletteIndex = i;
                    isPaletteClicked = true;
                    Repaint();
                    EditorGUI.EndChangeCheck();
                }

                GUILayout.Space(10);

                if (_selectedPaletteIndex == i)
                {
                    foreach (var colorOption in palette.ColorOptions)
                    {
                        if (colorOption == null)
                        {
                            continue;
                        }

                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginChangeCheck();

                        string newName = EditorGUILayout.TextField(colorOption.name, GUILayout.Width(160));

                        if (EditorGUI.EndChangeCheck())
                        {
                            // Rename the color option
                            RenameAsset(colorOption, newName);
                        }

                        EditorGUI.BeginChangeCheck();

                        ColorVariation colorVariation = colorOption.ColorVariations.Find(cv => cv.Palette == palette);
                        Color newColor = EditorGUILayout.ColorField(colorVariation.Color);

                        if (EditorGUI.EndChangeCheck())
                        {
                            // Update color in the ColorVariation
                            colorVariation.Color = newColor;
                            EditorUtility.SetDirty(colorOption);
                            AssetDatabase.SaveAssets();
                        }

                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            DeleteColorOption(colorOption);
                            GUIUtility.ExitGUI();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            // Check if an empty area is clicked to deselect the palette
            if (Event.current.type == EventType.MouseDown && !isPaletteClicked &&
                GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _selectedPaletteIndex = -1;
                Repaint();
            }


            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Update Color Components", GUILayout.Height(ButtonHeight)))
            {
                UpdateAllColors();
            }
            
            EditorGUILayout.BeginHorizontal();

            // Check if the selected palette is the first one
            if (_selectedPaletteIndex == 0)
            {
                if (GUILayout.Button("Create Color", GUILayout.Height(ButtonHeight)))
                {
                    CreateColor();
                }
            }

            if (GUILayout.Button("Create Palette", GUILayout.Height(ButtonHeight)))
            {
                CreatePalette();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private GUIStyle GetPaletteStyle(Palette palette)
        {
            if (_selectedPaletteIndex < 0 || palette == null || _palettes.IndexOf(palette) != _selectedPaletteIndex)
            {
                return EditorStyles.helpBox;
            }

            GUIStyle selectedStyle = new GUIStyle(EditorStyles.helpBox);
            selectedStyle.normal.background = MakeTex(2, 2, new Color(0f, 0.3686f, 0.7216f));
            return selectedStyle;
        }

        // Helper method to create a texture for background color
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void DeletePalette(Palette palette)
        {
            if (palette == null)
            {
                return;
            }

            // Check if it's the last palette
            bool isLastPalette = _palettes.Count == 1 && _palettes[0] == palette;

            // Remove the palette from the list
            _palettes.Remove(palette);

            // Delete all associated color options only if it's the last palette
            if (isLastPalette)
            {
                foreach (var colorOption in palette.ColorOptions)
                {
                    if (colorOption != null)
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(colorOption));
                    }
                }
            }

            // Delete the asset
            string palettePath = AssetDatabase.GetAssetPath(palette);
            AssetDatabase.DeleteAsset(palettePath);

            // Remove the palette from the PaletteList
            if (_paletteList != null && _paletteList.Palettes.Contains(palette))
            {
                _paletteList.Palettes.Remove(palette);
                EditorUtility.SetDirty(_paletteList);
                AssetDatabase.SaveAssets();
            }

            // Deselect the palette
            _selectedPaletteIndex = -1;

            // Mark the window as dirty
            EditorUtility.SetDirty(this);

            // Refresh the data
            RefreshData();
        }

        private void UpdateAllColors()
        {
            List<ColorComponentBase> colorComponents;

            colorComponents = new List<ColorComponentBase>(FindObjectsOfType<ColorComponentBase>());

            foreach (var colorComponent in colorComponents)
            {
                colorComponent.GetColorValue();
            }
        }
        
        private void DeleteColorOption(ColorOption colorOption)
        {
            if (colorOption == null)
            {
                return;
            }

            foreach (var palette in _palettes)
            {
                if (palette.ColorOptions != null)
                {
                    palette.ColorOptions.RemoveAll(opt => opt == colorOption);
                    EditorUtility.SetDirty(palette);
                }
            }

            // Remove the color option from the list
            _colorOptions.Remove(colorOption);

            // Delete the asset
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(colorOption));

            RefreshColorOptionsList(); // Refresh the color options list
            CleanUpEmptyFolders();
        }

        private void CreatePalette()
        {
            // Ensure there is at least one palette in the list
            if (_palettes.Count == 0)
            {
                // Create a new palette with an empty array of color options
                Palette newPalette = CreateInstance<Palette>();
                newPalette.name = GetUniquePaletteName(NewPaletteBaseName);
                newPalette.ColorOptions = new List<ColorOption>();

                // Add the new palette to the PaletteList
                _paletteList.Palettes.Add(newPalette);

                // Create and save the asset for the new palette
                string folderPath = PalettesFolderPath;
                CreateAndSaveAsset(newPalette, folderPath, newPalette.name, _palettes, out _selectedPaletteIndex);

                EditorUtility.SetDirty(newPalette);
                // Refresh the color options list to show the new palette
                RefreshColorOptionsList();

                return;
            }

            // Duplicate the first palette
            Palette firstPalette = _palettes[0];

            // Create a new palette with the same color options
            Palette duplicatedPalette = CreateInstance<Palette>();
            duplicatedPalette.name = GetUniquePaletteName(NewPaletteBaseName);

            // Copy color variations from the first palette
            foreach (var originalColorOption in firstPalette.ColorOptions)
            {
                if (originalColorOption != null)
                {
                    // Find the corresponding color variation in the first palette
                    ColorVariation originalColorVariation =
                        originalColorOption.ColorVariations.Find(variation => variation.Palette == firstPalette);

                    if (originalColorVariation == null)
                    {
                        continue;
                    }

                    duplicatedPalette.ColorOptions.Add(originalColorOption);
                    originalColorOption.ColorVariations.Add(new ColorVariation(duplicatedPalette,
                        originalColorVariation.Color));

                    // Mark the new color option as dirty
                    //EditorUtility.SetDirty(originalColorOption);
                    EditorUtility.SetDirty(duplicatedPalette);
                }
            }

            //Mark the new color option as dirty
            EditorUtility.SetDirty(duplicatedPalette);
            AssetDatabase.SaveAssets();

            // Add the new palette to the PaletteList
            _paletteList.Palettes.Add(duplicatedPalette);

            // Create and save the asset for the new palette
            string newFolderPath = PalettesFolderPath;
            CreateAndSaveAsset(duplicatedPalette, newFolderPath, duplicatedPalette.name, _palettes,
                out _selectedPaletteIndex);

            // Refresh the color options list to show the new palette
            RefreshColorOptionsList();

            // Refresh the data
            RefreshData();
        }

        private string GetUniquePaletteName(string baseName)
        {
            string paletteName = baseName;
            int count = 1;

            while (_palettes.Any(p => p.name == paletteName))
            {
                paletteName = $"{baseName}_{count}";
                count++;
            }

            return paletteName;
        }

        private void CreateColor()
        {
            ColorOption colorOption = CreateInstance<ColorOption>();

            // Update all palettes with the new color option
            foreach (var palette in _palettes)
            {
                if (palette.ColorOptions == null || palette.ColorOptions.Count == 0)
                {
                    palette.ColorOptions = new List<ColorOption>() { colorOption };
                }
                else
                {
                    palette.ColorOptions.Add(colorOption);
                }

                // Add a reference to the palette in the ColorOption dictionary
                colorOption.ColorVariations.Add(new ColorVariation(palette, Color.white));

                // Mark the palette as dirty
                EditorUtility.SetDirty(palette);
            }

            AssetDatabase.SaveAssets();

            // Mark the color option as dirty
            EditorUtility.SetDirty(colorOption);
            AssetDatabase.SaveAssets();

            string folderPath = ColorsFolderPath;

            // Check if the folder exists; if not, create it
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string guid = AssetDatabase.CreateFolder("Assets/Plugins/True_Outlaw", "ColorOptions");
                folderPath = AssetDatabase.GUIDToAssetPath(guid);

                // Refresh the asset database to ensure the folder is recognized
                AssetDatabase.Refresh();
            }

            CreateAndSaveAsset(colorOption, folderPath, NewColorOptionBaseName, _colorOptions, out _);

            // Add the new color option to the list
            _colorOptions.Add(colorOption);

            // Refresh the color options list to show the new color option
            RefreshColorOptionsList();
        }

        private void RefreshPalettesList()
        {
            if (_paletteList != null)
            {
                // Use a copy of the PaletteList asset's list
                _palettes = new List<Palette>(_paletteList.Palettes);
            }
        }

        private void RefreshColorOptionsList()
        {
            // Load all ColorOption assets from the folder and add them to the list
            _colorOptions.Clear();
            string[] assetPaths = AssetDatabase.FindAssets($"t:{typeof(ColorOption)}", new[] { ColorsFolderPath });
            foreach (string assetPath in assetPaths)
            {
                ColorOption colorOption =
                    AssetDatabase.LoadAssetAtPath<ColorOption>(AssetDatabase.GUIDToAssetPath(assetPath));

                if (colorOption != null)
                {
                    _colorOptions.Add(colorOption);
                }
            }
        }

        private void CleanUpEmptyFolders()
        {
            CleanUpEmptyFolder(PalettesFolderPath);
            CleanUpEmptyFolder(ColorsFolderPath);
        }

        private void CleanUpEmptyFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] assetPaths = AssetDatabase.FindAssets("", new[] { folderPath });

            if (assetPaths.Length != 0)
            {
                return;
            }

            AssetDatabase.DeleteAsset(folderPath);
            AssetDatabase.Refresh();
        }

        private void CreateAndSaveAsset<T>(T asset, string folderPath, string baseName, List<T> assetList,
            out int selectedIndex) where T : ScriptableObject
        {
            // Ensure the directory exists
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string guid = AssetDatabase.CreateFolder("Assets/Plugins/True_Outlaw", "ColorPalettes");
                folderPath = AssetDatabase.GUIDToAssetPath(guid);

                // Refresh the asset database to ensure the folder is recognized
                AssetDatabase.Refresh();
            }

            // Generate a unique name for the asset using a count
            string assetName = baseName;
            int count = 1;

            while (AssetDatabase.LoadAssetAtPath<T>($"{folderPath}/{assetName}.asset") != null)
            {
                assetName = $"{baseName}_{count}";
                count++;
            }

            AssetDatabase.CreateAsset(asset, $"{folderPath}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            // Add the new asset to the list or perform any other necessary actions
            assetList.Add(asset);
            selectedIndex = assetList.Count - 1; // Set the selected index for the newly created asset
        }

        private void RenameAsset(Object asset, string newName)
        {
            if (asset == null)
            {
                return;
            }

            string oldAssetPath = AssetDatabase.GetAssetPath(asset);
            string newAssetPath = oldAssetPath.Replace(asset.name, newName);
            AssetDatabase.RenameAsset(oldAssetPath, newName);
            AssetDatabase.ImportAsset(newAssetPath);
            asset.name = newName;
            EditorUtility.SetDirty(asset);
        }
    }
}
#endif