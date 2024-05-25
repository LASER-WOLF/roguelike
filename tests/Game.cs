using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;

namespace Test;

class Game
{
    public Game()
    {
        // Raylib/ImGui init
        //Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 720, "Roguelike");
        //Raylib.SetTargetFPS(30);
        rlImGui.Setup(true);

        // Raylib loop
        while (!Raylib.WindowShouldClose())
        {
            // Raylib start
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);
            
            // ImGui start
            rlImGui.Begin();

            // ImGui content
            ImGui.ShowDemoWindow();
            // if (ImGui.Begin("Simple Window"))
            // {
            //     ImGui.TextUnformatted("Icon text " + IconFonts.FontAwesome6.Book);
            // }

            // ImGui end
            ImGui.End();
            rlImGui.End();

            // Raylib end
            Raylib.EndDrawing();
        }

        // Raylib/Imgui exit
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
