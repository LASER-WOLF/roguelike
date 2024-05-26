using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;

using System.Numerics;

namespace Main;

class Game
{
    public static bool debug { get; private set; } = false;
    private Map map;
    private Font font;
    private Camera3D camera;
    private Vector3 position = new Vector3(0.0f, 0.5f, 0.0f);

    public Game()
    {
        Init();
        Run();
        Exit();
    }

    private void Init()
    {
        Raylib.InitWindow(1280, 720, "Roguelike");
        Raylib.SetTargetFPS(30);
        rlImGui.Setup(true);
        
        font = Raylib.LoadFont("./assets/fonts/Px437_IBM_VGA_8x16.ttf");
        map = new Map(64, 64);
        
        // Camera setup
        camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;
    }

    private void Input()
    {
        if (Raylib.IsKeyDown(KeyboardKey.Tab)) { debug = !debug; }
        if (Raylib.IsKeyDown(KeyboardKey.Up)) { position.Z -= (float)0.25; }
        if (Raylib.IsKeyDown(KeyboardKey.Down)) { position.Z += (float)0.25; }
        if (Raylib.IsKeyDown(KeyboardKey.Left)) { position.X -= (float)0.25; }
        if (Raylib.IsKeyDown(KeyboardKey.Right)) { position.X += (float)0.25; }
    }

    private void Update()
    {
        //Raylib.UpdateCamera(ref camera, CameraMode.Free);
    }

    private void RenderImGui()
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
            for (int i = 0; i < Main.Logger.log.Count; i++)
            {
                LogEntry logEntry = Logger.log[i];
                ImGui.Text(logEntry.message);
            }
            ImGui.EndChild();
        }
        ImGui.End();
        rlImGui.End();
    }

    private void Render()
    {
        // Start render
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
       
        // Camera
        camera.Target = position;
        camera.Position = position + new Vector3(0.0f, 10.0f, 10.0f);

        // 3D
        Raylib.BeginMode3D(camera);
        Raylib.DrawGrid(100, 1.0f);
        Raylib.DrawSphereEx(position, 0.5f, 4, 4, Color.Red);
        Raylib.DrawSphereWires(position, 0.5f, 4, 4, Color.White);
        Raylib.EndMode3D();

        map.Render();

        // 2D
        Raylib.DrawFPS(2,2);
        Raylib.DrawTextEx(font, "Roguelike", new Vector2(2, 20), 16, 2, Color.White);
        if (debug) { Raylib.DrawTextEx(font, "DEBUG MODE", new Vector2(2, 36), 16, 2, Color.White); }

        // ImGui
        if (debug) { RenderImGui(); }

        // End render
        Raylib.EndDrawing();
    }

    private void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Input();
            Update();
            Render();
        }
    }

    private void Exit()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
