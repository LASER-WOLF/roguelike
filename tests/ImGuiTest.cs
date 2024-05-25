using System;
using Raylib_cs;
using ImGuiNET;

namespace Test;

class ImGuiTest
{
    public ImGuiTest()
    {
        Raylib.InitWindow(1280, 720, "Story Creator");
        Raylib.SetTargetFPS(60);

        var context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        var controller = new ImGuiController();
        
        while (!Raylib.WindowShouldClose())
        {
            controller.NewFrame();
            controller.ProcessEvent();
            ImGui.NewFrame();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKGRAY);

            if (ImGui.Button("Cick me!"))
            {
                Console.WriteLine("Button clicked.");
            }

            ImGui.Render();
            controller.Render(ImGui.GetDrawData());
            Raylib.EndDrawing();
        }

        controller.Shutdown();
    }
}

