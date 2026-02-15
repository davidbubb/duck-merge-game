using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace DuckMergeGame.Editor
{
    /// <summary>
    /// Editor utility for generating simple bird sprites (colored circles) for MVP
    /// </summary>
    public class BirdSpriteGenerator : EditorWindow
    {
        private GameConfig gameConfig;
        private int spriteSize = 512;
        private string savePath = "Assets/Sprites/Birds/";
        
        [MenuItem("Duck Merge/Generate Bird Sprites")]
        public static void ShowWindow()
        {
            GetWindow<BirdSpriteGenerator>("Bird Sprite Generator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Bird Sprite Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            gameConfig = EditorGUILayout.ObjectField("Game Config", gameConfig, typeof(GameConfig), false) as GameConfig;
            spriteSize = EditorGUILayout.IntField("Sprite Size (px)", spriteSize);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            
            GUILayout.Space(10);
            
            if (gameConfig == null)
            {
                EditorGUILayout.HelpBox("Please assign a GameConfig to generate sprites.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox($"Will generate {gameConfig.totalBirdTiers} bird sprites as colored circles.", MessageType.Info);
            }
            
            GUILayout.Space(10);
            
            GUI.enabled = gameConfig != null;
            if (GUILayout.Button("Generate All Bird Sprites", GUILayout.Height(40)))
            {
                GenerateAllSprites();
            }
            GUI.enabled = true;
            
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("This will create simple colored circle sprites for each bird tier. " +
                                   "These are placeholder graphics for MVP testing. " +
                                   "Replace with proper artwork later.", MessageType.Info);
        }
        
        private void GenerateAllSprites()
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not assigned!");
                return;
            }
            
            // Create directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                string[] folders = savePath.Split('/');
                string currentPath = folders[0];
                
                for (int i = 1; i < folders.Length; i++)
                {
                    if (!string.IsNullOrEmpty(folders[i]))
                    {
                        if (!AssetDatabase.IsValidFolder(currentPath + "/" + folders[i]))
                        {
                            AssetDatabase.CreateFolder(currentPath, folders[i]);
                        }
                        currentPath += "/" + folders[i];
                    }
                }
            }
            
            // Generate sprite for each tier
            for (int tier = 1; tier <= gameConfig.totalBirdTiers; tier++)
            {
                GenerateBirdSprite(tier);
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"Generated {gameConfig.totalBirdTiers} bird sprites in {savePath}");
            EditorUtility.DisplayDialog("Success", 
                $"Generated {gameConfig.totalBirdTiers} bird sprites successfully!", 
                "OK");
        }
        
        private void GenerateBirdSprite(int tier)
        {
            // Create texture
            Texture2D texture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[spriteSize * spriteSize];
            
            Color birdColor = gameConfig.GetBirdColor(tier);
            float radius = spriteSize / 2f;
            Vector2 center = new Vector2(spriteSize / 2f, spriteSize / 2f);
            
            // Draw circle
            for (int y = 0; y < spriteSize; y++)
            {
                for (int x = 0; x < spriteSize; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    
                    if (distance < radius)
                    {
                        // Inside circle - use bird color
                        // Add subtle gradient for depth
                        float normalizedDist = distance / radius;
                        Color pixelColor = Color.Lerp(birdColor, birdColor * 0.7f, normalizedDist * normalizedDist);
                        pixels[y * spriteSize + x] = pixelColor;
                    }
                    else if (distance < radius + 2)
                    {
                        // Edge anti-aliasing
                        float alpha = 1f - (distance - radius) / 2f;
                        pixels[y * spriteSize + x] = new Color(birdColor.r, birdColor.g, birdColor.b, alpha);
                    }
                    else
                    {
                        // Outside circle - transparent
                        pixels[y * spriteSize + x] = Color.clear;
                    }
                }
            }
            
            // Add simple highlight for 3D effect
            float highlightRadius = radius * 0.3f;
            Vector2 highlightCenter = center + new Vector2(-radius * 0.2f, radius * 0.2f);
            
            for (int y = 0; y < spriteSize; y++)
            {
                for (int x = 0; x < spriteSize; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), highlightCenter);
                    
                    if (distance < highlightRadius)
                    {
                        float normalizedDist = distance / highlightRadius;
                        float alpha = (1f - normalizedDist) * 0.5f;
                        int index = y * spriteSize + x;
                        Color current = pixels[index];
                        pixels[index] = Color.Lerp(current, Color.white, alpha);
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            // Save as PNG
            byte[] bytes = texture.EncodeToPNG();
            string fileName = $"{savePath}Bird_Tier{tier:D2}_{gameConfig.GetBirdName(tier).Replace(" ", "")}.png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            
            // Import and configure texture
            AssetDatabase.ImportAsset(fileName);
            
            TextureImporter importer = AssetImporter.GetAtPath(fileName) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = spriteSize;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            
            Debug.Log($"Generated sprite for Tier {tier}: {gameConfig.GetBirdName(tier)}");
        }
    }
}
#endif
