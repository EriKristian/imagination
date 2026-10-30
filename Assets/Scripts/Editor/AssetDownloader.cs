using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Download free Kenney CC0 assets from GitHub and assign to project.
/// All assets are Public Domain (CC0) - https://kenney.nl
/// Usage: Window > Imagination > Download & Assign Free Assets
/// </summary>
public static class AssetDownloader
{
    private const string BaseUrl = "https://raw.githubusercontent.com/shorepine/kenney/main/";
    private const string TargetFolder = "Assets/Art/FreeAssets";

    private static readonly (string remotePath, string localName, string[] tags)[] AssetList = {
        ("2d/Abstract Platformer/Players/player_stand.png",      "player_stand.png",    new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_left.png",       "player_left.png",     new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_right.png",      "player_right.png",    new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_attack.png",     "player_attack.png",   new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_hit.png",        "player_hit.png",      new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_run_1.png",      "player_run1.png",     new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_run_2.png",      "player_run2.png",     new[]{"Player"}),
        ("2d/Abstract Platformer/Players/player_jump.png",       "player_jump.png",     new[]{"Player"}),
        ("2d/Abstract Platformer/Enemies/enemyFloating_1.png",   "enemy_float.png",     new[]{"Opponent","Enemy"}),
        ("2d/Abstract Platformer/Enemies/enemySpikey_1.png",     "enemy_spikey.png",    new[]{"Opponent","Enemy"}),
        ("2d/Abstract Platformer/Enemies/enemyFlying_1.png",     "enemy_fly.png",       new[]{"Opponent","Enemy"}),
        ("2d/Abstract Platformer/Enemies/enemySwimming_1.png",   "enemy_swim.png",      new[]{"Opponent","Enemy"}),
        ("2d/Abstract Platformer/Enemies/enemyWalking_1.png",    "enemy_walk.png",      new[]{"Opponent","Enemy"}),
        ("2d/Abstract Platformer/Players/player_stand.png",      "npc_default.png",     new[]{"NPC"}),
        ("2d/Abstract Platformer/Players/player_left.png",       "npc_fighter.png",     new[]{"NPC_Fighter"}),
        ("2d/Abstract Platformer/Players/player_right.png",      "npc_racer.png",       new[]{"NPC_Racer"}),
        ("2d/Abstract Platformer/Players/player_attack.png",     "npc_platformer.png",  new[]{"NPC_Platformer"}),
        ("2d/Abstract Platformer/Tiles/tile_grass.png",          "tile_grass.png",      new[]{"Ground","Tile"}),
        ("2d/Abstract Platformer/Tiles/tile_stone.png",          "tile_stone.png",      new[]{"Ground","Tile"}),
        ("2d/Abstract Platformer/Tiles/tile_hazard.png",         "tile_hazard.png",     new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0000.png","1bit_0000.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0001.png","1bit_0001.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0002.png","1bit_0002.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0010.png","1bit_0010.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0014.png","1bit_0014.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0020.png","1bit_0020.png",      new[]{"Ground","Tile"}),
        ("2d/1-Bit Platformer Pack/Tiles/Transparent/tile_0025.png","1bit_0025.png",      new[]{"Ground","Tile"}),
        ("2d/Abstract Platformer/Backgrounds/background_1.png",  "background_1.png",    new[]{"Background"}),
        ("2d/Abstract Platformer/Backgrounds/background_2.png",  "background_2.png",    new[]{"Background"}),
        ("2d/Abstract Platformer/Items/item_coin.png",           "item_coin.png",       new[]{"Item","NPC"}),
        ("2d/Abstract Platformer/Items/item_key.png",            "item_key.png",        new[]{"Item","NPC"}),
        ("2d/Abstract Platformer/Items/item_sword.png",          "item_sword.png",      new[]{"Item","NPC"}),
    };

    [MenuItem("Imagination/Download & Assign Free Assets (Kenney CC0)")]
    public static void DownloadAndAssign()
    {
        EnsureFolders();
        int downloaded = 0, failed = 0, skipped = 0;

        foreach (var (remotePath, localName, tags) in AssetList)
        {
            string fullPath = $"{TargetFolder}/{localName}";
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath) != null)
            {
                skipped++;
                continue;
            }
            if (DownloadFile($"{BaseUrl}{remotePath}", fullPath))
                downloaded++;
            else
                failed++;
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Imagination — Kenney Assets",
            $"Downloaded: {downloaded}\nSkipped: {skipped}\nFailed: {failed}\n\nAll CC0 by Kenney.nl", "OK");
    }

    [MenuItem("Imagination/Assign Free Assets to Scene")]
    public static void AssignToScene()
    {
        if (!Directory.Exists(Path.GetFullPath(TargetFolder))) { Debug.LogError("No free assets folder."); return; }
        int assigned = 0;
        var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in renderers)
        {
            if (sr.sprite != null) continue;
            var sprite = FindBestSprite(sr.gameObject.name);
            if (sprite != null) { sr.sprite = sprite; assigned++; }
        }
        Debug.Log($"Assigned {assigned} sprites");
        EditorUtility.DisplayDialog("Imagination", $"Assigned {assigned} free sprites!", "OK");
    }

    [MenuItem("Imagination/Download, Assign & Regenerate All")]
    public static void FullFlow()
    {
        DownloadAndAssign();
        SceneSetup.CreateAllScenes();
        AssignToScene();
        Debug.Log("[AssetDownloader] Full flow complete!");
    }

    private static bool DownloadFile(string url, string localPath)
    {
        try
        {
            using var www = new UnityEngine.Networking.UnityWebRequest(url, UnityEngine.Networking.UnityWebRequest.kHttpVerbGET);
            www.timeout = 30;
            www.SendWebRequest();
            while (!www.isDone) System.Threading.Thread.Sleep(50);
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(Path.GetFullPath(localPath), www.downloadHandler.data);
                return true;
            }
            Debug.LogWarning($"Failed: {url} — {www.error}");
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return false;
        }
    }

    private static Sprite FindBestSprite(string goName)
    {
        foreach (var (remotePath, localName, tags) in AssetList)
        {
            string path = $"{TargetFolder}/{localName}";
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) continue;
            bool match = tags.Any(t => goName.Contains(t) || t.Contains(goName));
            if (match)
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Art"))
            AssetDatabase.CreateFolder("Assets", "Art");
        if (!AssetDatabase.IsValidFolder(TargetFolder))
            AssetDatabase.CreateFolder("Assets/Art", "FreeAssets");
        if (!AssetDatabase.IsValidFolder("Assets/Art/Placeholders"))
            AssetDatabase.CreateFolder("Assets/Art", "Placeholders");
    }
}
