using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Auto-assigns sprites from Assets/Art/FreeAssets to scene objects on scene load.
/// No placeholder generation — uses only downloaded Kenney CC0 assets.
/// </summary>
public static class SpriteAssigner
{
    private const string FreeAssetsFolder = "Assets/Art/FreeAssets";

    private static readonly Dictionary<string, string> NameToSprite = new()
    {
        { "Player",          "player_stand.png" },
        { "Opponent",        "enemy_float_1.png" },
        { "NPC_Fighter",     "npc_fighter.png"  },
        { "NPC_Racer",       "npc_racer.png"    },
        { "NPC_Platformer",  "npc_platformer.png" },
        { "Background",      "background_1.png" },
    };

    [MenuItem("Imagination/Assign Sprites to Current Scene")]
    public static void AssignToScene()
    {
        int assigned = 0;
        var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in renderers)
        {
            if (sr.sprite != null) continue;
            var sprite = GetSpriteFor(sr.gameObject.name);
            if (sprite != null)
            {
                sr.sprite = sprite;
                assigned++;
            }
        }
        Debug.Log($"[SpriteAssigner] Assigned {assigned} sprites");
        EditorUtility.DisplayDialog("Imagination", $"Assigned {assigned} sprites from FreeAssets!", "OK");
    }

    [MenuItem("Imagination/Assign Sprites to All Scenes")]
    public static void AssignToAllScenes()
    {
        string[] scenes = {
            "Assets/Scenes/Classroom.unity",
            "Assets/Scenes/Fighting_Level1.unity"
        };
        int total = 0;
        foreach (var path in scenes)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            int assigned = 0;
            var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            foreach (var sr in renderers)
            {
                if (sr.sprite != null) continue;
                var sprite = GetSpriteFor(sr.gameObject.name);
                if (sprite != null) { sr.sprite = sprite; assigned++; }
            }
            if (assigned > 0)
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[SpriteAssigner] Assigned {assigned} sprites in {scene.name}");
            }
            total += assigned;
        }
        EditorUtility.DisplayDialog("Imagination", $"Assigned {total} sprites across all scenes.", "OK");
    }

    public static Sprite GetSpriteFor(string goName)
    {
        foreach (var (key, filename) in NameToSprite)
        {
            if (goName.Contains(key))
            {
                string path = $"{FreeAssetsFolder}/{filename}";
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        return null;
    }
}
