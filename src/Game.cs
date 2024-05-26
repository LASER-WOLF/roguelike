using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;

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
            //ImGui.ShowDemoWindow();
            if (ImGui.Begin("Debug window", ImGuiWindowFlags.MenuBar))
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Options"))
                    {
                        if (ImGui.MenuItem("Item 1")) 
                        {
                            // do something..
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                
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

            // Raylib end
            Raylib.EndDrawing();
        }

        // Raylib/Imgui exit
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
