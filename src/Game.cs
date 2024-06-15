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

    // Private
    private static Planet planet;
    private static Font font;
    private static Camera3D camera;
    private static float cameraZoom;

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
        //map = new Map(64, 64);
        //player = new Player();
        planet = new Planet(100);
        
        // Camera setup
        camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        //camera.Target = player.pos;
        //camera.Position = camera.Target + new Vector3(0.0f, 150.0f, 18.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;
        cameraZoom = 2.5f;
    }

    // Get input from the user
    private static void Input()
    {
        // System
        if (Raylib.IsKeyPressed(KeyboardKey.Tab)) { debug = !debug; }
        if (Raylib.IsKeyPressed(KeyboardKey.F)) { Raylib.ToggleFullscreen(); }
        
        // Movement
        // if (Raylib.IsKeyPressed(KeyboardKey.Up)) { player.MoveUp(); }
        // else if (Raylib.IsKeyPressed(KeyboardKey.Down)) { player.MoveDown(); }
        // else if (Raylib.IsKeyPressed(KeyboardKey.Left)) { player.MoveLeft(); }
        // else if (Raylib.IsKeyPressed(KeyboardKey.Right)) { player.MoveRight(); }
        int charPressed = Raylib.GetCharPressed();
        if (charPressed == 45) { cameraZoom += 0.2f; }
        if (charPressed == 43) { cameraZoom -= 0.2f; }

        if (Raylib.IsKeyDown(KeyboardKey.Up)) { planet.Rotate(new Vector3(-0.1f, 0.0f, 0.0f)); }
        if (Raylib.IsKeyDown(KeyboardKey.Down)) { planet.Rotate(new Vector3(0.1f, 0.0f, 0.0f)); }
        //if (Raylib.IsKeyDown(KeyboardKey.Left)) { planet.Rotate(new Vector3(0.0f, 0.0f, 0.1f)); }
        //if (Raylib.IsKeyDown(KeyboardKey.Right)) { planet.Rotate(new Vector3(0.0f, 0.0f, -0.1f)); }
        if (Raylib.IsKeyDown(KeyboardKey.Left)) { planet.Rotate(new Vector3(0.0f, 0.1f, 0.0f)); }
        if (Raylib.IsKeyDown(KeyboardKey.Right)) { planet.Rotate(new Vector3(0.0f, -0.1f, 0.0f)); }
    }

    // Update things
    private static void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Camera
        //Raylib.UpdateCamera(ref camera, CameraMode.Free);
        //Vector3 cameraTargetGoal = player.pos;
        Vector3 cameraTargetGoal = planet.pos;
        camera.Target = Raymath.Vector3Distance(camera.Target, cameraTargetGoal) > 0.1f ? Raymath.Vector3Lerp(camera.Target, cameraTargetGoal, 0.05f) : camera.Target;
        camera.Position = camera.Target + new Vector3(0.0f, planet.size * cameraZoom, 18.0f);
        //camera.Position = camera.Target + new Vector3(0.0f, planet.size * 1.5f, 18.0f);
        
        //camera.Position = camera.Target + new Vector3(0f, 16.0f, 12.0f);

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
        // Raylib.ClearBackground(Color.Black);
        Raylib.ClearBackground(Color.Red);

        // 3D
        Raylib.BeginMode3D(camera);
        if (debug) { Raylib.DrawGrid(300, 1.0f); }
        
        //map.Render();
        //player.Render();
        planet.Render3D();
        
        Raylib.EndMode3D();

        planet.Render2D();

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
        planet.Exit();
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
