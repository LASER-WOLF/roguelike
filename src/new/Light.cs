using System.Numerics;
using Raylib_cs;

namespace Core;

public enum LightType
{
    Directional,
    Point
}

public class Light
{
    private static int count = 0;
    private int id;
    private bool enabled = true;
    private LightType type;
    public Vector3 pos;
    public Vector3 target;
    public Color color;
    private int enabledLoc;
    private int typeLoc;
    private int posLoc;
    private int targetLoc;
    private int colorLoc;

    // Constructor
    public Light(LightType type, Vector3 pos, Vector3 target, Color color, Shader shader)
    {
        this.id = count;
        count++;
        string enabledName = "lights[" + this.id + "].enabled";
        string typeName = "lights[" + this.id + "].type";
        string posName = "lights[" + this.id + "].position";
        string targetName = "lights[" + this.id + "].target";
        string colorName = "lights[" + this.id + "].color";
        this.type = type;
        this.pos = pos;
        this.target = target;
        this.color = color;
        this.enabledLoc = Raylib.GetShaderLocation(shader, enabledName);
        this.typeLoc = Raylib.GetShaderLocation(shader, typeName);
        this.posLoc = Raylib.GetShaderLocation(shader, posName);
        this.targetLoc = Raylib.GetShaderLocation(shader, targetName);
        this.colorLoc = Raylib.GetShaderLocation(shader, colorName);
        UpdateLightValues(shader, this);
    }

    public static void UpdateLightValues(Shader shader, Light light)
    {
        float[] color = new [] { (float)light.color.R / 255f, (float)light.color.G / 255f, (float)light.color.B / 255f, (float)light.color.A / 255f };
        Raylib.SetShaderValue(shader, light.enabledLoc, light.enabled ? 1 : 0, ShaderUniformDataType.Int);
        Raylib.SetShaderValue(shader, light.typeLoc, (int)light.type, ShaderUniformDataType.Int);
        Raylib.SetShaderValue(shader, light.posLoc, light.pos, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(shader, light.targetLoc, light.target, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(shader, light.colorLoc, color, ShaderUniformDataType.Vec4);
    }
}
