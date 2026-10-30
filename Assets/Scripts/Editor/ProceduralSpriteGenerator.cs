using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Generate placeholder sprites (colored squares) automatically.
/// No external assets needed. Run: Window > Imagination > Generate Placeholder Sprites
/// </summary>
public static class ProceduralSpriteGenerator
{
    public const string SpriteFolder = "Assets/Art/Placeholders";

    private static readonly Dictionary<string, Color> ColorScheme = new()
    {
        { "Player", new Color(0.2f, 0.6f, 1.0f) },
        { "Opponent", new Color(1.0f, 0.3f, 0.3f) },
        { "NPC_Fighter", new Color(1.0f, 0.6f, 0.2f) },
        { "NPC_Racer", new Color(0.6f, 0.2f, 1.0f) },
        { "NPC_Platformer", new Color(1.0f, 0.9f, 0.2f) },
        { "NPC", new Color(0.3f, 1.0f, 0.5f) },
        { "Ground", new Color(0.4f, 0.4f, 0.4f) },
        { "Wall", new Color(0.3f, 0.3f, 0.3f) },
        { "Default", new Color(0.8f, 0.8f, 0.8f) },
        { "Panel", new Color(0.15f, 0.15f, 0.2f) },
        { "Button", new Color(0.3f, 0.5f, 0.8f) },
        { "HealthBar", new Color(0.2f, 1.0f, 0.2f) },
    };

    [MenuItem("Imagination/Generate Placeholder Sprites")]
    public static void GenerateAll()
    {
        if (!AssetDatabase.IsValidFolder(SpriteFolder))
            AssetDatabase.CreateFolder("Assets/Art", "Placeholders");

        int count = 0;
        foreach (var (name, color) in ColorScheme)
        {
            string path = $"{SpriteFolder}/{name}.png";
            if (!AssetDatabase.LoadAssetAtPath<Texture2D>(path))
            {
                var tex = CreatePlaceholderTexture(color, 64);
                var bytes = tex.EncodeToPNG();
                string fullPath = Path.GetFullPath(path);
                File.WriteAllBytes(fullPath, bytes);
                AssetDatabase.ImportAsset(path);
                count++;
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[ProceduralSprite] Generated {count} placeholder sprites");
        EditorUtility.DisplayDialog("Imagination", $"Generated {count} placeholder sprites!", "OK");
    }

    /// <summary>
    /// Called by SceneSetup after creating scenes to auto-assign sprites
    /// </summary>
    public static void AssignPlaceholdersToScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath,OpenSceneMode.Single);
        int assigned = 0;
        var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in renderers)
        {
            if (sr.sprite == null)
            {
                var sprite = GetOrCreateSpriteFor(sr.gameObject.name);
                if (sprite != null)
                {
                    sr.sprite = sprite;
                    assigned++;
                }
            }
        }
        if (assigned > 0)
        {
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[ProceduralSprite] Assigned {assigned} sprites in {scene.name}");
        }
    }

    public static Sprite GetOrCreateSpriteFor(string name)
    {
        // First try FreeAssets (downloaded Kenney sprites)
        string freeAssetPath = FindMatchingAsset(name);
        if (freeAssetPath != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(freeAssetPath);
            if (tex) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        // Fallback to procedural placeholders
        foreach (var (key, color) in ColorScheme)
        {
            if (name.Contains(key))
            {
                string path = $"{SpriteFolder}/{key}.png";
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        string pathDefault = $"{SpriteFolder}/Default.png";
        var texDefault = AssetDatabase.LoadAssetAtPath<Texture2D>(pathDefault);
        if (texDefault) return Sprite.Create(texDefault, new Rect(0, 0, texDefault.width, texDefault.height), new Vector2(0.5f, 0.5f));
        return null;
    }

    private static string FindMatchingAsset(string goName)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Art/FreeAssets")) return null;

        // Mapping: GameObject name -> filename
        // Player -> player_stand.png
        // Opponent -> enemy_float_1.png
        // NPC_Fighter -> npc_fighter.png
        // NPC_Racer -> npc_racer.png
        // NPC_Platformer -> npc_platformer.png
        // NPC -> npc_fighter.png (fallback)
        // Background -> background_1.png

        if (goName.Contains("Player")) return "Assets/Art/FreeAssets/player_stand.png";
        if (goName.Contains("Opponent")) return "Assets/Art/FreeAssets/enemy_float_1.png";
        if (goName.Contains("NPC_Fighter")) return "Assets/Art/FreeAssets/npc_fighter.png";
        if (goName.Contains("NPC_Racer")) return "Assets/Art/FreeAssets/npc_racer.png";
        if (goName.Contains("NPC_Platformer")) return "Assets/Art/FreeAssets/npc_platformer.png";
        if (goName.Contains("NPC")) return "Assets/Art/FreeAssets/npc_fighter.png";
        if (goName.Contains("Background")) return "Assets/Art/FreeAssets/background_1.png";

        return null;
    }

    private static Texture2D CreatePlaceholderTexture(Color color, int size = 64)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        tex.filterMode = FilterMode.Point;
        var pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        tex.SetPixels(pixels);

        // Border
        Color borderColor = color * 0.6f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                if (x < 2 || x >= size - 2 || y < 2 || y >= size - 2)
                    pixels[y * size + x] = borderColor;

        // Cross hair indicator
        int cx = size / 2, cy = size / 2;
        for (int i = -3; i <= 3; i++)
        {
            if (cy + i >= 0 && cy + i < size) pixels[(cy + i) * size + cx] = new Color(1, 1, 1, 0.5f);
            if (cx + i >= 0 && cx + i < size) pixels[cy * size + (cx + i)] = new Color(1, 1, 1, 0.5f);
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
