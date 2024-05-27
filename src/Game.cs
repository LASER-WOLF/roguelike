using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;

namespace Core;

static class Game
{
    public static bool debug { get; private set; } = false;
    public static Map map { get; private set; }
    public static Player player { get; private set; }
    
    private static Font font;
    private static Camera3D camera;

    static void Main(string[] args)
    {
        Init();
        Run();
        Exit();
    }

    private static void Init()
    {
        Raylib.InitWindow(1280, 720, "Roguelike");
        Raylib.SetTargetFPS(30);
        rlImGui.Setup(true);
        
        font = Raylib.LoadFont("./assets/fonts/Px437_IBM_VGA_8x16.ttf");
        map = new Map(64, 64);
        player = new Player();
        
        // Camera setup
        camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;
    }

    private static void Input()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Tab)) { debug = !debug; }
        
        if (Raylib.IsKeyPressed(KeyboardKey.Up)) { player.MoveUp(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Down)) { player.MoveDown(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Left)) { player.MoveLeft(); }
        else if (Raylib.IsKeyPressed(KeyboardKey.Right)) { player.MoveRight(); }
    }

    private static void Update()
    {
        //Raylib.UpdateCamera(ref camera, CameraMode.Free);
    }

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

    private static void Render()
    {
        // Start render
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
       
        // Camera
        camera.Target = player.pos;
        camera.Position = player.pos + new Vector3(0.0f, 20.0f, 20.0f);

        // 3D
        Raylib.BeginMode3D(camera);
        if (debug) { Raylib.DrawGrid(300, 1.0f); }
        map.Render();
        player.Render();
        Raylib.EndMode3D();

        // 2D
        map.RenderMinimap();
        Raylib.DrawFPS(2,2);
        Raylib.DrawTextEx(font, "Roguelike", new Vector2(2, 20), 16, 2, Color.White);
        if (debug) { Raylib.DrawTextEx(font, "DEBUG MODE", new Vector2(2, 36), 16, 2, Color.White); }

        // ImGui
        if (debug) { RenderImGui(); }

        // End render
        Raylib.EndDrawing();
    }

    private static void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Input();
            Update();
            Render();
        }
    }

    private static void Exit()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
