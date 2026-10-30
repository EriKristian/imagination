using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Imagination.Core;
using Imagination.Managers;
using Imagination.UI;
using Imagination.Gameplay;

public static class SceneSetup
{
    [MenuItem("Imagination/Setup/Create All Scenes")]
    public static void CreateAllScenes()
    {
        EnsureScenesFolder();

        CreateBootstrapScene();
        CreateMenuScene();
        CreateClassroomScene();
        CreateFightingScene();

        SetBuildScenes();

        EditorUtility.DisplayDialog("Imagination",
            "Semua scene berhasil dibuat!\n\nScene:\n- Bootstrap.unity\n- MainMenu.unity\n- Classroom.unity\n- Fighting_Level1.unity\n\nBuild settings sudah di-update.", "OK");
    }

    private static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
    }

    private static void SetBuildScenes()
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Bootstrap.unity", true));
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true));
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Classroom.unity", true));
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Fighting_Level1.unity", true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void CreateBootstrapScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var go = new GameObject("Bootstrap");
        go.AddComponent<GameManager>();
        go.AddComponent<SceneLoader>();
        go.AddComponent<SaveManager>();
        go.AddComponent<ProgressionManager>();
        go.AddComponent<QuestManager>();
        go.AddComponent<DialogueManager>();
        go.AddComponent<AudioManager>();

        var uiGO = new GameObject("UIManager");
        uiGO.transform.SetParent(go.transform);
        var uiManager = uiGO.AddComponent<UIManager>();
        SetupUIManager(uiManager);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Bootstrap.unity", false);
        Debug.Log("[SceneSetup] Created Bootstrap.unity");
    }

    private static void CreateMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var canvasGO = new GameObject("UIRoot");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var menuGO = new GameObject("MenuPanel");
        menuGO.transform.SetParent(canvasGO.transform);
        var menuRect = menuGO.AddComponent<RectTransform>();
        menuRect.anchorMin = Vector2.zero;
        menuRect.anchorMax = Vector2.one;
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;

        CreateMenuButtons(menuGO);
        menuGO.AddComponent<MenuController>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity", false);
        Debug.Log("[SceneSetup] Created MainMenu.unity");
    }

    private static void CreateClassroomScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5;

        var playerGO = new GameObject("Player");
        playerGO.transform.position = Vector2.zero;
        var playerRb = playerGO.AddComponent<Rigidbody2D>();
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerGO.AddComponent<PlayerController>();
        playerGO.AddComponent<BoxCollider2D>();
        playerGO.tag = "Player";

        CreateNPC("NPC_Fighter", new Vector2(-3, 2), "Fight King", QuestType.Fighting);
        CreateNPC("NPC_Racer", new Vector2(3, 2), "Speed Demon", QuestType.Racing);
        CreateNPC("NPC_Platformer", new Vector2(0, -3), "Jump Master", QuestType.Platformer);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Classroom.unity", false);
        Debug.Log("[SceneSetup] Created Classroom.unity");
    }

    private static void CreateFightingScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5;

        var playerGO = new GameObject("Player");
        playerGO.transform.position = new Vector2(-2, 0);
        playerGO.AddComponent<SpriteRenderer>();
        playerGO.AddComponent<BoxCollider2D>();
        playerGO.AddComponent<Animator>();
        var playerRb = playerGO.AddComponent<Rigidbody2D>();
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerGO.tag = "Player";

        var opponentGO = new GameObject("Opponent");
        opponentGO.transform.position = new Vector2(2, 0);
        opponentGO.AddComponent<SpriteRenderer>();
        opponentGO.AddComponent<BoxCollider2D>();
        opponentGO.AddComponent<Animator>();
        var opponentRb = opponentGO.AddComponent<Rigidbody2D>();
        opponentRb.bodyType = RigidbodyType2D.Kinematic;
        opponentGO.tag = "Opponent";

        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var fightMgrGO = new GameObject("FightingManager");
        var fightMgr = fightMgrGO.AddComponent<FightingManager>();
        fightMgr.player = playerGO.transform;
        fightMgr.opponent = opponentGO.transform;

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Fighting_Level1.unity", false);
        Debug.Log("[SceneSetup] Created Fighting_Level1.unity");
    }

    private static void SetupUIManager(UIManager uiManager)
    {
        uiManager.menuScreen = CreateUIGameObject("MenuScreen");
        uiManager.dialoguePanel = CreateUIGameObject("DialoguePanel");
        uiManager.loadingScreen = CreateUIGameObject("LoadingScreen");
        uiManager.victoryScreen = CreateUIGameObject("VictoryScreen");
        uiManager.touchControls = CreateUIGameObject("TouchControls");
        uiManager.promptPanel = CreateUIGameObject("PromptPanel");
    }

    private static GameObject CreateUIGameObject(string name)
    {
        var go = new GameObject(name);
        go.AddComponent<RectTransform>();
        go.SetActive(false);
        return go;
    }

    private static void CreateMenuButtons(GameObject parent)
    {
        string[] btnNames = { "NewGameButton", "ContinueButton", "SettingsButton", "QuitButton" };
        foreach (var btnName in btnNames)
        {
            var btnGO = new GameObject(btnName);
            btnGO.transform.SetParent(parent.transform);
            btnGO.AddComponent<RectTransform>();
            var btn = btnGO.AddComponent<Button>();
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(btnGO.transform);
            textGO.AddComponent<RectTransform>();
            textGO.AddComponent<Text>();
        }
    }

    private static void CreateNPC(string name, Vector2 position, string speakerName, QuestType questType)
    {
        var npcGO = new GameObject(name);
        npcGO.transform.position = position;
        npcGO.AddComponent<BoxCollider2D>().isTrigger = true;
        npcGO.AddComponent<SpriteRenderer>();
        npcGO.AddComponent<Animator>();
        var interaction = npcGO.AddComponent<NPCInteraction>();
        interaction.npcName = speakerName;
        interaction.questType = questType;
    }
}
