using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;

using System.Numerics;

namespace Main;

class Game
{
    public static bool debug { get; private set; } = false;
    private Map map;

    public Game()
    {
        map = new Map(128, 48);

        // Raylib/ImGui init
        //Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 720, "Roguelike");
        Raylib.SetTargetFPS(30);
        rlImGui.Setup(true);
        Font font = Raylib.LoadFont("./assets/fonts/Px437_IBM_VGA_8x16.ttf");

        Camera3D camera;
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        Vector3 cubePosition = new Vector3(0.0f, 1.0f, 0.0f);

        // Raylib loop
        while (!Raylib.WindowShouldClose())
        {
            // INPPUT:
            if (Raylib.IsKeyDown(KeyboardKey.Tab)) { debug = !debug; }

            // UPDATE:
            Raylib.UpdateCamera(ref camera, CameraMode.Free);

            // DRAW:
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            // 3D:
            Raylib.BeginMode3D(camera);
            Raylib.DrawGrid(100, 1.0f);
            Raylib.DrawCube(cubePosition, 2.0f, 2.0f, 2.0f, Color.Red);
            Raylib.DrawCubeWires(cubePosition, 2.0f, 2.0f, 2.0f, Color.Maroon);
            Raylib.EndMode3D();
            // 2D:
            Raylib.DrawFPS(2,2);
            Raylib.DrawTextEx(font, "Roguelike", new Vector2(2, 20), 16, 2, Color.White);
            if (debug) { Raylib.DrawTextEx(font, "DEBUG MODE", new Vector2(2, 36), 16, 2, Color.White); }

            if (debug)
            {
                // ImGui start
                rlImGui.Begin();
                // ImGui content
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
                    //foreach (LogEntry logEntry in Main.Logger.log)
                    for (int i = 0; i < Main.Logger.log.Count; i++)
                    {
                        LogEntry logEntry = Logger.log[i];
                        ImGui.Text(logEntry.message);
                    }
                    ImGui.EndChild();

                //    ImGui.TextUnformatted("Icon text " + IconFonts.FontAwesome6.Book);
                }

                // ImGui end
                ImGui.End();
                rlImGui.End();
            }

            // Raylib end
            Raylib.EndDrawing();
        }

        // Raylib/Imgui exit
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
