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
    public static bool debug { get; private set; } = true;
    public static Map map { get; private set; }
    public static Player player { get; private set; }
    
    public static float time { get; private set;}
    public static float timeSpeed {get; private set; } = 1f * 60f * 60f;
    public static int timeDays { get; private set; }
    public static int timeHours { get; private set; }
    public static int timeMinutes { get; private set; }
    public static int timeSeconds { get; private set; }
    public static float timeDayPhase { get; private set; }
    public static float timeMoonPhase { get; private set; }
    public static float timeYearPhase { get; private set; }

    // Private
    private static PlanetarySystem planetarySystem;
    public static Shader shader {get; private set;}
    private static Light[] lights;
    
    // Raylib & ImGui
    private static Font raylibFont;
    private static ImFontPtr imguiFont;
    private static bool imguiShowMainMenubar  =  true;
    private static bool imguiShowCameraWindow = true;
    private static bool imguiShowLogWindow    = true;
    private static bool imguiShowPlanetWindow = true;
    private static bool imguiShowMouseWindow  = true;
    private static bool imguiShowDemoWindow   = false;
    
    // ImGui styling, Set color palette
    private static Vector4 colorImgui0    = new Vector4( 28f,  28f,  28f, 255f) / 255f; // Normal, Black
    private static Vector4 colorImgui1    = new Vector4(175f,  95f,  95f, 255f) / 255f; // Normal, Red
    private static Vector4 colorImgui2    = new Vector4( 95f, 135f,  95f, 255f) / 255f; // Normal, Green
    private static Vector4 colorImgui3    = new Vector4(135f, 135f,  95f, 255f) / 255f; // Normal, Yellow
    private static Vector4 colorImgui4    = new Vector4( 95f, 135f, 175f, 255f) / 255f; // Normal, Blue
    private static Vector4 colorImgui5    = new Vector4( 95f,  95f, 135f, 255f) / 255f; // Normal, Magenta
    private static Vector4 colorImgui6    = new Vector4( 95f, 135f, 135f, 255f) / 255f; // Normal, Cyan
    private static Vector4 colorImgui7    = new Vector4(108f, 108f, 108f, 255f) / 255f; // Normal, White
    private static Vector4 colorImgui8    = new Vector4( 68f,  68f,  68f, 255f) / 255f; // Bright, Black
    private static Vector4 colorImgui9    = new Vector4(255f, 135f,   0f, 255f) / 255f; // Bright, Red
    private static Vector4 colorImgui10   = new Vector4(135f, 175f, 135f, 255f) / 255f; // Bright, Green
    private static Vector4 colorImgui11   = new Vector4(255f, 255f, 175f, 255f) / 255f; // Bright, Yellow
    private static Vector4 colorImgui12   = new Vector4(135f, 175f, 215f, 255f) / 255f; // Bright, Blue
    private static Vector4 colorImgui13   = new Vector4(135f, 135f, 175f, 255f) / 255f; // Bright, Magenta
    private static Vector4 colorImgui14   = new Vector4( 95f, 175f, 175f, 255f) / 255f; // Bright, Cyan
    private static Vector4 colorImgui15   = new Vector4(255f, 255f, 255f, 255f) / 255f; // Bright, White
    private static Vector4 colorImguiFg   = new Vector4(188f, 188f, 188f, 255f) / 255f; // Foreground
    private static Vector4 colorImguiBg   = new Vector4( 38f,  38f,  38f, 255f) / 255f; // Background
    private static Vector4 colorImguiNone = new Vector4(  0f,   0f,   0f,   0f);        // None
    
    // Camera
    private static Camera3D camera;
    private static Vector2 cameraRotation = new Vector2(0f, MathF.PI * 0.1f);
    private static Vector3 cameraPosition = new Vector3(0f);
    private static Vector3 cameraUp = Vector3.UnitY;
    private static int cameraTargetDistance = 100;
    private static float cameraDistance = 100f;
    private static float cameraDistanceMultiplier { get { return Smoothstep.QuarticPolynomial(cameraDistance / 100f); } }
    private static float cameraSpeedMultiplier { get { return 0.2f + cameraDistanceMultiplier * 0.8f; } }
    private static float cameraFov = 45f;
    private static bool cameraNorthUp;
    private static float cameraSpeedPan = 0.05f;
    private static float cameraSpeedDistance = 0.1f;
    private static float cameraSpeedInactive = 0f; 

    // Mouse
    private static Ray mouseRay = new Ray(Vector3.Zero, Vector3.Zero);
    private static RayCollision mouseRayCollision = new RayCollision();
    private static Vector3 mouseRayCollisionPointSphere;
    private static (int Face, int X, int Y) mouseRayCollisionPointGrid;
    private static (int XDeg, float Xmin, int Ydeg, float Ymin) mouseRayCollisionPointGcs;

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
        Logger.Log("Initializing game");
        // Raylib setup
        Raylib.InitWindow(1280, 720, "roguelike-v0.0.0");
        Raylib.SetTargetFPS(30);
        raylibFont = Raylib.LoadFont("./assets/fonts/Px437_IBM_VGA_8x16.ttf");

        // ImGui setup
        rlImGui.Setup(true);
        ImGuiStylePtr style = ImGui.GetStyle();

        // ImGui font setup
        unsafe
        {
            ImFontGlyphRangesBuilderPtr builder = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
            builder.AddText(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂ ¡¢£¤¥¦§¨©ª«¬-®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƒơƷǺǻǼǽǾǿȘșȚțɑɸˆˇˉ˘˙˚˛˜˝;΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϐϴЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюяѐёђѓєѕіїјљњћќѝўџҐґ־אבגדהוזחטיךכלםמןנסעףפץצקרשתװױײ׳״ᴛᴦᴨẀẁẂẃẄẅẟỲỳ‐‒–—―‗‘’‚‛“”„‟†‡•…‧‰′″‵‹›‼‾‿⁀⁄⁔⁴⁵⁶⁷⁸⁹⁺⁻ⁿ₁₂₃₄₅₆₇₈₉₊₋₣₤₧₪€℅ℓ№™Ω℮⅐⅑⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞←↑→↓↔↕↨∂∅∆∈∏∑−∕∙√∞∟∩∫≈≠≡≤≥⊙⌀⌂⌐⌠⌡─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬▀▁▄█▌▐░▒▓■□▪▫▬▲►▼◄◊○●◘◙◦☺☻☼♀♂♠♣♥♦♪♫✓ﬁﬂ�");
            builder.BuildRanges(out ImVector ranges);
            imguiFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("./assets/fonts/Px437_IBM_VGA_8x16.ttf", 16, null, ranges.Data);
        }
        rlImGui.ReloadFonts();

        // ImGui styling, Sizes
        style.WindowPadding            = new Vector2(8f, 6f);
        style.FramePadding             = new Vector2(8f, 4f);
        style.ItemSpacing              = new Vector2(16f, 8f);
        style.ScrollbarSize            = 16f;
        style.GrabMinSize              = 16f;
        style.WindowBorderSize         = 0f;
        style.ChildBorderSize          = 0f;
        style.PopupBorderSize          = 1f;
        style.FrameBorderSize          = 1f;
        style.TabBorderSize            = 1f;
        style.TabBarBorderSize         = 1f;
        style.WindowRounding           = 2f;
        style.ChildRounding            = 0f;
        style.FrameRounding            = 1f;
        style.ScrollbarRounding        = 0f;
        style.PopupRounding            = 1f;
        style.GrabRounding             = 0f;
        style.TabRounding              = 6f;
        style.WindowMenuButtonPosition = ImGuiDir.None;

        // ImGui styling, Set colors
        style.Colors[(int)ImGuiCol.Text]                  = colorImguiFg;
        style.Colors[(int)ImGuiCol.TextDisabled]          = colorImgui1;
        style.Colors[(int)ImGuiCol.WindowBg]              = colorImguiBg;
        style.Colors[(int)ImGuiCol.ChildBg]               = colorImguiBg;
        style.Colors[(int)ImGuiCol.PopupBg]               = colorImguiBg;
        style.Colors[(int)ImGuiCol.Border]                = colorImgui8;
        style.Colors[(int)ImGuiCol.BorderShadow]          = colorImguiNone;
        style.Colors[(int)ImGuiCol.FrameBg]               = colorImguiBg;
        style.Colors[(int)ImGuiCol.FrameBgHovered]        = colorImguiBg;
        style.Colors[(int)ImGuiCol.FrameBgActive]         = colorImguiBg;
        style.Colors[(int)ImGuiCol.TitleBg]               = colorImguiBg;
        style.Colors[(int)ImGuiCol.TitleBgActive]         = colorImguiBg;
        style.Colors[(int)ImGuiCol.TitleBgCollapsed]      = colorImguiBg;
        style.Colors[(int)ImGuiCol.MenuBarBg]             = colorImguiBg;
        style.Colors[(int)ImGuiCol.ScrollbarBg]           = colorImguiBg;
        style.Colors[(int)ImGuiCol.ScrollbarGrab]         = colorImgui8;
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]  = colorImgui7;
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]   = colorImgui8;
        style.Colors[(int)ImGuiCol.CheckMark]             = colorImguiFg;
        style.Colors[(int)ImGuiCol.SliderGrab]            = colorImgui8;
        style.Colors[(int)ImGuiCol.SliderGrabActive]      = colorImgui7;
        style.Colors[(int)ImGuiCol.Button]                = colorImgui8;
        style.Colors[(int)ImGuiCol.ButtonHovered]         = colorImgui7;
        style.Colors[(int)ImGuiCol.ButtonActive]          = colorImgui8;
        style.Colors[(int)ImGuiCol.Header]                = colorImgui8;
        style.Colors[(int)ImGuiCol.HeaderHovered]         = colorImgui7;
        style.Colors[(int)ImGuiCol.HeaderActive]          = colorImgui8;
        style.Colors[(int)ImGuiCol.Separator]             = colorImgui8;
        style.Colors[(int)ImGuiCol.SeparatorHovered]      = colorImgui8;
        style.Colors[(int)ImGuiCol.SeparatorActive]       = colorImgui8;
        style.Colors[(int)ImGuiCol.ResizeGrip]            = colorImgui8;
        style.Colors[(int)ImGuiCol.ResizeGripHovered]     = colorImgui7;
        style.Colors[(int)ImGuiCol.ResizeGripActive]      = colorImgui8;
        style.Colors[(int)ImGuiCol.Tab]                   = colorImgui8;
        style.Colors[(int)ImGuiCol.TabHovered]            = colorImguiBg;
        // style.Colors[(int)ImGuiCol.TabActive]             = colorImguiBg;
        // style.Colors[(int)ImGuiCol.TabUnfocused]          = colorImguiBg;
        // style.Colors[(int)ImGuiCol.TabUnfocusedActive]    = colorImguiBg;
        style.Colors[(int)ImGuiCol.DockingPreview]        = colorImguiBg;
        style.Colors[(int)ImGuiCol.DockingEmptyBg]        = colorImguiBg;
        style.Colors[(int)ImGuiCol.PlotLines]             = colorImgui8;
        style.Colors[(int)ImGuiCol.PlotLinesHovered]      = colorImgui7;
        style.Colors[(int)ImGuiCol.PlotHistogram]         = colorImgui8;
        style.Colors[(int)ImGuiCol.PlotHistogramHovered]  = colorImgui7;
        style.Colors[(int)ImGuiCol.TableHeaderBg]         = colorImguiBg;
        style.Colors[(int)ImGuiCol.TableBorderStrong]     = colorImgui8;
        style.Colors[(int)ImGuiCol.TableBorderLight]      = colorImgui8;
        style.Colors[(int)ImGuiCol.TableRowBg]            = colorImguiBg;
        style.Colors[(int)ImGuiCol.TableRowBgAlt]         = colorImgui8;
        style.Colors[(int)ImGuiCol.TextSelectedBg]        = colorImguiBg;
        style.Colors[(int)ImGuiCol.DragDropTarget]        = colorImguiBg;
        style.Colors[(int)ImGuiCol.NavHighlight]          = colorImguiBg;
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = colorImguiBg;
        style.Colors[(int)ImGuiCol.NavWindowingDimBg]     = colorImguiBg;
        style.Colors[(int)ImGuiCol.ModalWindowDimBg]      = colorImguiBg;
        
        // Camera setup
        camera.Position = Vector3.Zero;
        camera.Target = Vector3.Zero;
        camera.Up = cameraUp;
        camera.FovY = cameraFov;
        camera.Projection = CameraProjection.Perspective;

        // Planet setup
        //seed = 0B_00000010_00000001_00000011_00000001;
        planetarySystem = new PlanetarySystem(seed: (uint)DateTime.Now.Ticks, planetSize: 400);

        // Shader setup
        shader = Raylib.LoadShader(
            "./src/shaders/lighting.vs",
            "./src/shaders/lighting.fs"
        );
        unsafe { shader.Locs[(int)ShaderLocationIndex.VectorView] = Raylib.GetShaderLocation(shader, "viewPos"); }
        Raylib.SetShaderValue(shader, Raylib.GetShaderLocation(shader, "ambient"), new Vector4(0.05f, 0.05f, 0.05f, 1.0f), ShaderUniformDataType.Vec4);
        lights = new Light[1];
        lights[0] = new Light(
            LightType.Point,
            Vector3.Zero,
            //new Vector3 (50f, 50f, 50f),
            Vector3.Zero,
            new Color(242, 235, 220, 255),
            shader
        );
        
        unsafe {
            planetarySystem.planetMat.Shader = shader;
            planetarySystem.moonMat.Shader = shader;
        }
    }
    
    // Main game loop
    private static void Run()
    {
        Logger.Log("Starting game loop");
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
        Logger.Log("Exiting game");
        planetarySystem.Exit();
        Raylib.UnloadShader(shader);
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    // Get input from the user
    private static void Input()
    {
        int charPressed = Raylib.GetCharPressed();

        // System
        if (Raylib.IsKeyPressed(KeyboardKey.Tab)) { debug = !debug; }
        if (Raylib.IsKeyPressed(KeyboardKey.F)) { Raylib.ToggleFullscreen(); }
       
        // Interaction
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            mouseRay = Raylib.GetMouseRay(Raylib.GetMousePosition(), camera); 
            mouseRayCollision = Raylib.GetRayCollisionSphere(mouseRay, planetarySystem.planetPos, 1f);
            mouseRayCollisionPointSphere = Raymath.Vector3Transform(mouseRayCollision.Point - planetarySystem.planetPos, Raymath.MatrixInvert(planetarySystem.planetRotationMatrix));
            mouseRayCollisionPointGrid = planetarySystem.TransformCubeToGrid(planetarySystem.TransformSphereToCube(mouseRayCollisionPointSphere));
            mouseRayCollisionPointGcs = planetarySystem.GridToGcs(mouseRayCollisionPointGrid);
        }

        // Camera
        if (Raylib.IsMouseButtonDown(MouseButton.Right))
        {
            cameraSpeedInactive = 0f;
            Vector2 mouseDelta = Raylib.GetMouseDelta() * 0.005f * cameraSpeedMultiplier * cameraSpeedMultiplier;
            cameraRotation.X += (cameraNorthUp ? -mouseDelta.X : mouseDelta.X);
            cameraRotation.Y += mouseDelta.Y;
        }
        if (charPressed == 45 && cameraTargetDistance < 100) { cameraTargetDistance++; }
        if (charPressed == 43 && cameraTargetDistance > 0) { cameraTargetDistance--; }
        if (Raylib.IsKeyDown(KeyboardKey.Up)) { cameraRotation.Y += cameraSpeedPan * cameraSpeedMultiplier; cameraSpeedInactive = 0f; }
        if (Raylib.IsKeyDown(KeyboardKey.Down)) { cameraRotation.Y -= cameraSpeedPan * cameraSpeedMultiplier; cameraSpeedInactive = 0f; }
        if (Raylib.IsKeyDown(KeyboardKey.Left)) { cameraRotation.X += (cameraNorthUp ? -cameraSpeedPan : cameraSpeedPan) * cameraSpeedMultiplier; cameraSpeedInactive = 0f; }
        if (Raylib.IsKeyDown(KeyboardKey.Right)) { cameraRotation.X += (cameraNorthUp ? cameraSpeedPan : -cameraSpeedPan) * cameraSpeedMultiplier; cameraSpeedInactive = 0f; }
        if (Raylib.GetMouseWheelMove() > 0f && cameraTargetDistance >= 5) { cameraTargetDistance -= 5; }
        if (Raylib.GetMouseWheelMove() < 0f && cameraTargetDistance <= 95) { cameraTargetDistance += 5; }
    }

    // Update everything
    private static void Update()
    {
        float deltaTime = Raylib.GetFrameTime();
        UpdateTime(deltaTime);
        UpdateCamera(deltaTime);
        UpdateShaders(deltaTime);
        planetarySystem.Update(deltaTime);
    }

    private static void UpdateTime(float deltaTime)
    {
        time += deltaTime * timeSpeed;
        timeDays = (int)MathF.Floor((float)time / (60f * 60f * 24f));
        timeHours = (int)MathF.Floor((float)time % (60f * 60f * 24f) / (60f * 60f));
        timeMinutes = (int)MathF.Floor((float)time % (60f * 60f) / 60f);
        timeSeconds = (int)MathF.Floor((float)time % 60f);
        timeDayPhase = (float)time % (60f * 60f * 24f) / (60f * 60f * 24f);
        timeMoonPhase = (float)time % (60f * 60f * 24f * 27.3f) / (60f * 60f * 24f * 27.3f);
        timeYearPhase = (float)time % (60f * 60f * 24f * 365f) / (60f * 60f * 24f * 365f);
    }

    private static unsafe void UpdateShaders(float deltaTime)
    {
        //foreach (Light light in lights) { Light.UpdateLightValues(shader, light); }
        Raylib.SetShaderValue(
                shader,
                shader.Locs[(int)ShaderLocationIndex.VectorView],
                camera.Position,
                ShaderUniformDataType.Vec3
            );
    }

    // Update camera
    private static void UpdateCamera(float deltaTime)
    {
        camera.Target = planetarySystem.planetPos;
        float cameraTargetDifference = MathF.Abs(cameraDistance - (float)cameraTargetDistance);
        if (cameraTargetDifference > 0.01f) { cameraDistance += (cameraDistance < (float)cameraTargetDistance ? cameraSpeedDistance : -cameraSpeedDistance ) * cameraTargetDifference; }
        // float newRotation = deltaTime * 0.025f * cameraSpeedMultiplier;
        // if (cameraSpeedInactive < 1f) {
        //     cameraSpeedInactive += deltaTime;
        //     newRotation *= Smoothstep.QuarticPolynomial(cameraSpeedInactive);
        // }
        // cameraRotation.X += newRotation;
        cameraRotation.X %= MathF.PI * 2;
        cameraRotation.Y %= MathF.PI * 2;
        cameraNorthUp = (MathF.Cos(cameraRotation.Y) > 0f) ? true : false;
        cameraPosition.X = MathF.Sin(cameraRotation.X) * MathF.Cos(cameraRotation.Y);
        cameraPosition.Y = MathF.Sin(cameraRotation.Y);
        cameraPosition.Z = MathF.Cos(cameraRotation.X) * MathF.Cos(cameraRotation.Y);
        cameraUp.X = MathF.Sin(cameraRotation.X) * MathF.Cos(cameraRotation.Y + 0.1f);
        cameraUp.Y = MathF.Sin(cameraRotation.Y + 0.1f);
        cameraUp.Z = MathF.Cos(cameraRotation.X) * MathF.Cos(cameraRotation.Y + 0.1f);
        camera.Up = Raymath.Vector3Transform(cameraUp, planetarySystem.planetRotationMatrix);
        camera.Position = Raymath.Vector3Transform(cameraPosition * (1.25f + cameraDistanceMultiplier * 5f), planetarySystem.planetMatrix);
    }
    
    // Render things
    private static void Render()
    {
        // Start render
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        // 3D
        Raylib.BeginMode3D(camera);
        planetarySystem.Render3D();
        if (debug) 
        {
            Raylib.DrawRay(mouseRay, Color.Violet); 
            if (mouseRayCollision.Hit) { 
                Vector3 collisionPointA = Raymath.Vector3Transform(mouseRayCollisionPointSphere, planetarySystem.planetMatrix); 
                Vector3 collisionPointB = Raymath.Vector3Transform(planetarySystem.TransformSphereToCube(mouseRayCollisionPointSphere) * 2f, planetarySystem.planetMatrix);
                Raylib.DrawSphere(collisionPointA, 0.01f, Color.Orange);
                Raylib.DrawSphere(collisionPointB, 0.01f, Color.Orange);
                Raylib.DrawLine3D(collisionPointA, collisionPointB, Color.Orange);
            }
        }
        Raylib.EndMode3D();

        // 2D
        planetarySystem.Render2D();
        //map.RenderMinimap();
        Raylib.DrawFPS(2, debug ? 40 : 2);
        if (debug) { Raylib.DrawTextEx(raylibFont, "DEBUG MODE", new Vector2(2, debug ? 24 : 2), 16, 2, Color.White); }
        Raylib.DrawTextEx(raylibFont,  "SPEED:" + timeSpeed.ToString() + "x " + "YEAR%" + MathF.Floor(timeYearPhase * 100f).ToString("00") + " MOON%" + MathF.Floor(timeMoonPhase * 100f).ToString("00") + " DAY%" + MathF.Floor(timeDayPhase * 100f).ToString("00"), new Vector2(2, 80), 16, 2, Color.White);
        Raylib.DrawTextEx(raylibFont,  "DAY:" + timeDays.ToString("000"), new Vector2(2, 100), 16, 2, Color.White);
        Raylib.DrawTextEx(raylibFont,  "TIME:" + timeHours.ToString("00") + ":" + timeMinutes.ToString("00") + ":" + timeSeconds.ToString("00"), new Vector2(2, 120), 16, 2, Color.White);

        // ImGui
        if (debug) { RenderImGui(); }

        // End render
        Raylib.EndDrawing();
    }
    
    // Render ImGui
    private static void RenderImGui()
    {
        rlImGui.Begin();
        ImGui.PushFont(imguiFont);
        if (imguiShowMainMenubar) { ImGuiShowMainMenubar(); }
        if (imguiShowDemoWindow) { ImGui.ShowDemoWindow(); }
        if (imguiShowLogWindow) { ImGuiShowLogWindow(); }
        if (imguiShowCameraWindow) { ImGuiShowCameraWindow(); }
        if (imguiShowMouseWindow) { ImGuiShowMouseWindow(); }
        if (imguiShowPlanetWindow) { planetarySystem.RenderImGui(); }
        ImGui.End();
        rlImGui.End();
    }

    // Show ImGui main menubar
    private static void ImGuiShowMainMenubar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("View"))
            {
                if (ImGui.MenuItem("Camera window", null, imguiShowCameraWindow)) { imguiShowCameraWindow = !imguiShowCameraWindow; }
                if (ImGui.MenuItem("Mouse window",  null, imguiShowMouseWindow)) { imguiShowMouseWindow = !imguiShowMouseWindow; }
                if (ImGui.MenuItem("Planet window", null, imguiShowPlanetWindow)) { imguiShowPlanetWindow = !imguiShowPlanetWindow; }
                if (ImGui.MenuItem("Log window",    null, imguiShowLogWindow)) { imguiShowLogWindow = !imguiShowLogWindow; }
                if (ImGui.MenuItem("Demo window",   null, imguiShowDemoWindow)) { imguiShowDemoWindow = !imguiShowDemoWindow; }
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }
    
    // Show ImGui log window
    private static void ImGuiShowLogWindow()
    {
        if (ImGui.Begin("Log"))
        {
            for (int i = 0; i < Logger.log.Count; i++)
            {
                LogEntry logEntry = Logger.log[i];
                DateTime logDate = new DateTime(logEntry.time);
                ImGui.TextColored(logEntry.error ? colorImgui1 : colorImguiFg, logDate.ToString("HH:mm:ss") + ": " + logEntry.message);
            }
        }
    }
   
    // Show ImGui mouse window
    private static void ImGuiShowMouseWindow()
    {
        if (ImGui.Begin("Mouse"))
        {
            ImGui.Text("Hit:          " + mouseRayCollision.Hit.ToString());
            //ImGui.Text("Distance:     " + mouseRayCollision.Distance.ToString("0.00"));
            //ImGui.Text("Point:       " + mouseRayCollision.Point.ToString("0.00"));
            //ImGui.Text("Normal:       " + mouseRayCollision.Normal.ToString("0.00"));
            ImGui.Text("Sphere point: " + mouseRayCollisionPointSphere.ToString("0.00"));
            ImGui.Text("Cube point:   " + planetarySystem.TransformSphereToCube(mouseRayCollisionPointSphere).ToString("0.00"));
            ImGui.Text("Grid point:   " + mouseRayCollisionPointGrid.ToString());
            ImGui.Text("GCS point:    " + mouseRayCollisionPointGcs.ToString());
        }
    }
    
    // Show ImGui camera window
    private static void ImGuiShowCameraWindow()
    {
        if (ImGui.Begin("Camera"))
        {
            ImGui.Text("Rotation: " + ((cameraRotation.X < 0f ? cameraRotation.X + (MathF.PI * 2f) : cameraRotation.X) / (MathF.PI * 2f) * 360f).ToString("000.0°") + ", " + ((cameraRotation.Y < 0f ? cameraRotation.Y + (MathF.PI * 2f) : cameraRotation.Y) / (MathF.PI * 2f) * 360f).ToString("000.0°")) ;
            ImGui.Text("Position: " + cameraPosition.X.ToString("+0.00;-0.00; 0.00") + ", " + cameraPosition.Y.ToString("+0.00;-0.00; 0.00") + ", " + cameraPosition.Z.ToString("+0.00;-0.00; 0.00"));
            ImGui.Text("Distance: " + cameraDistance.ToString("0.0") + " (" + cameraTargetDistance.ToString() + ")");
            ImGui.Text("North:    " + (cameraNorthUp ? "↑" : "↓" ));
        }
    }
}
