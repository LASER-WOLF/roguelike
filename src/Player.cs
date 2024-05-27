using System.Numerics;
using Raylib_cs;

namespace Core;

/// <summary>
/// Player controlled character.
/// </summary>
public class Player
{
    public Vector3 pos { get; private set; }
    private int visionRange = 32;

    public Player()
    {
        Spawn();
        Fov();
    }

    public void Render()
    {
        //Raylib.DrawSphereEx(pos + new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 4, 4, Color.Red);
        Raylib.DrawSphereWires(pos + new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 4, 4, Color.Pink);
    }

    private void Spawn()
    {
        Room room = Game.map.tree.FindLeftLeaf(Game.map.tree.root).room;
        pos = new Vector3((float)(room.x) + 2.0f, 0.0f, (float)(room.y) + 2.0f);
    }

    private void Fov()
    {
        for (int y = 0; y < Game.map.height; y++)
        {
            for (int x = 0; x < Game.map.width; x++)
            {
                Game.map.mapVisible[x, y] = false;
            }
        }

        Shadowcast.Run(Game.map, new Vec2((int)pos.X, (int)pos.Z), visionRange);
        //ShadowcastAlt.Run(map, new Vec2(x, y));
    }

    public bool MoveUp()
    {
        if (Game.map.pathGraph.HasLocation(Game.map.MapCoord((int)pos.X, (int)pos.Z - 1))) { pos -= new Vector3(0.0f, 0.0f, 1.0f); Fov(); return true; }
        return false;
    }

    public bool MoveDown()
    {
        if (Game.map.pathGraph.HasLocation(Game.map.MapCoord((int)pos.X, (int)pos.Z + 1))) { pos += new Vector3(0.0f, 0.0f, 1.0f); Fov(); return true; }
        return false;
    }
    
    public bool MoveLeft()
    {
        if (Game.map.pathGraph.HasLocation(Game.map.MapCoord((int)pos.X - 1, (int)pos.Z))) { pos -= new Vector3(1.0f, 0.0f, 0.0f); Fov(); return true; }
        return false;
    }
    
    public bool MoveRight()
    {
        if (Game.map.pathGraph.HasLocation(Game.map.MapCoord((int)pos.X + 1, (int)pos.Z))) { pos += new Vector3(1.0f, 0.0f, 0.0f); Fov(); return true; }
        return false;
    }
}
