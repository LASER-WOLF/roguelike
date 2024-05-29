using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;

namespace Core;

/// <summary>
/// Main class, contains program entry point.
/// </summary>
static class Game
{
    // Public
    public static bool debug { get; private set; } = false;
    public static Map map { get; private set; }
    public static Player player { get; private set; }

    private static Planet planet;
    
    // Private
    private static Font font;
    private static Camera3D camera;

    // Entry point
    static void Main(string[] args)
    {
        Init();
        Run();
        Exit();
    }

    // Initialize
    private static void Init()
    {
        Raylib.InitWindow(1280, 720, "Roguelike");
        Raylib.SetTargetFPS(30);
        rlImGui.Setup(true);
        
        font = Raylib.LoadFont("./assets/fonts/Px437_IBM_VGA_8x16.ttf");
        map = new Map(64, 64);
        player = new Player();
        planet = new Planet(10);
        
        // Camera setup
        camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        //camera.Target = player.pos;
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;
    }

    // Get input from the user
    private static void Input()
    {
        // System
        if (Raylib.IsKeyPressed(KeyboardKey.Tab)) { debug = !debug; }
        if (Raylib.IsKeyPressed(KeyboardKey.F)) { Raylib.ToggleFullscreen(); }
        
        // Movement
        if (Raylib.IsKeyPressed(KeyboardKey.Up)) { player.MoveUp(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Down)) { player.MoveDown(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Left)) { player.MoveLeft(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Right)) { player.MoveRight(); }
    }

    // Update things
    private static void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Camera
        //Raylib.UpdateCamera(ref camera, CameraMode.Free);
        //Vector3 cameraTargetGoal = player.pos;
        Vector3 cameraTargetGoal = planet.pos;
        Vector3 cameraTarget = Raymath.Vector3Distance(camera.Target, cameraTargetGoal) > 0.1f ? Raymath.Vector3Lerp(camera.Target, cameraTargetGoal, 0.05f) : camera.Target;
        Vector3 cameraPosition = cameraTarget + new Vector3(0.0f, 25.0f, 18.0f);
        camera.Target = cameraTarget;
        camera.Position = cameraPosition;

        planet.Update(deltaTime);
    }

    // Render ImGui
    private static void RenderImGui()
    {
        rlImGui.Begin();
        //ImGui.ShowDemoWindow();
        //if (ImGui.Begin("Debug window", ImGuiWindowFlags.MenuBar))
        if (ImGui.Begin("Debug window"))
        {
            // if (ImGui.BeginMenuBar())
            // {
            //     if (ImGui.BeginMenu("Options"))
            //     {
            //         if (ImGui.MenuItem("Item 1")) 
            //         {
            //             // do something..
            //         }
            //         ImGui.EndMenu();
            //     }
            //     ImGui.EndMenuBar();
            // }
            ImGui.Text("Log:");
            ImGui.BeginChild("Log");
            for (int i = 0; i < Logger.log.Count; i++)
            {
                LogEntry logEntry = Logger.log[i];
                ImGui.Text(logEntry.message);
            }
            ImGui.EndChild();
        }
        ImGui.End();
        rlImGui.End();
    }

    // Render things
    private static void Render()
    {
        // Start render
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        // 3D
        Raylib.BeginMode3D(camera);
        if (debug) { Raylib.DrawGrid(300, 1.0f); }
        
        //map.Render();
        //player.Render();
        planet.Render();
        
        Raylib.EndMode3D();

        // 2D
        //map.RenderMinimap();
        Raylib.DrawFPS(2,2);
        //Raylib.DrawTextEx(font, "Position: " + player.pos.X.ToString() + "x" + player.pos.Z.ToString(), new Vector2(2, Raylib.GetRenderHeight() - 16), 16, 2, Color.White);
        if (debug) { Raylib.DrawTextEx(font, "DEBUG MODE", new Vector2(2, 20), 16, 2, Color.White); }

        // ImGui
        if (debug) { RenderImGui(); }

        // End render
        Raylib.EndDrawing();
    }

    // Main game loop
    private static void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Input();
            Update();
            Render();
        }
    }

    // Exit game
    private static void Exit()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
