using Raylib_cs;
using System.Numerics;

namespace Core;

/// <summary>
/// ...
/// </summary>
public class Planet
{

    public Vector3 pos { get; private set; }
    public readonly int size;
    //private bool[,,] tilemaps;
    private Model model;
    private float rotation;

    public Planet(int size)
    {
        this.size = size;
        //this.tilemaps = new bool[6,size,size];
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        this.rotation = 0.0f;
        model = Raylib.LoadModelFromMesh(GenMeshCube());
        Generate();
    }

    private void Generate()
    {
        //...
    }

    public void Render()
    {
        Raylib.DrawModelEx(model, pos, new Vector3(1.0f, 0.0f, 1.0f), rotation, Vector3.One, Color.White);
    }

    private Mesh GenMeshCube()
    {
        int numVerts = 4 * (6 * (size * size));
        int numTris = 2 * (6 * (size * size));

        Mesh mesh = new(numVerts, numTris);
        mesh.AllocVertices();
        mesh.AllocTexCoords();
        mesh.AllocNormals();
        mesh.AllocColors();
        mesh.AllocIndices();

        Span<Vector3> vertices = mesh.VerticesAs<Vector3>();
        Span<Vector2> texcoords = mesh.TexCoordsAs<Vector2>();
        Span<Color> colors = mesh.ColorsAs<Color>();
        Span<ushort> indices = mesh.IndicesAs<ushort>();
        Span<Vector3> normals = mesh.NormalsAs<Vector3>();

        int vertIndex = 0;
        int triIndex = 0;
        Color color = Color.White;
        for (int face = 0; face < 6; face++)
        {
            switch (face)
            {
                case 0: color = Color.SkyBlue; break;
                case 1: color = Color.Red; break;
                case 2: color = Color.Yellow; break;
                case 3: color = Color.Green; break;
                case 4: color = Color.Purple; break;
                case 5: color = Color.Beige; break;
            }
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float texCoordLeft = x * (x / size);
                    float texCoordRight = (x + 1) * (x / size);
                    float texCoordTop = y * (y / size);
                    float texCoordBottom = (y + 1) * (y / size);

                    // Vertex 1 - Top-left
                    vertices[vertIndex  + 0] = Transform(face, new Vector2(x + 0, y + 0));
                    normals[vertIndex   + 0] = new(0, 1, 0);
                    texcoords[vertIndex + 0] = new(texCoordLeft, texCoordTop);
                    colors[vertIndex    + 0] = color;
                    
                    // Vertex 2 - Top-right
                    vertices[vertIndex  + 1] = Transform(face, new Vector2(x + 1, y + 0));
                    normals[vertIndex   + 1] = new(0, 1, 0);
                    texcoords[vertIndex + 1] = new(texCoordRight, texCoordTop);
                    colors[vertIndex    + 1] = color;

                    // Vertex 3 - Bottom-left
                    vertices[vertIndex  + 2] = Transform(face, new Vector2(x + 0, y + 1));
                    normals[vertIndex   + 2] = new(0, 1, 0);
                    texcoords[vertIndex + 2] = new(texCoordLeft, texCoordBottom);
                    colors[vertIndex    + 2] = color;
                    
                    // Vertex 4 - Bottom-right
                    vertices[vertIndex  + 3] = Transform(face, new Vector2(x + 1, y + 1));
                    normals[vertIndex   + 3] = new(0, 1, 0);
                    texcoords[vertIndex + 3] = new(texCoordRight, texCoordBottom);
                    colors[vertIndex    + 3] = color;

                    // Triangle 1
                    indices[triIndex + 0] = (ushort)(vertIndex + 0);
                    indices[triIndex + 1] = (ushort)(vertIndex + 3);
                    indices[triIndex + 2] = (ushort)(vertIndex + 1);

                    // Triangle 2
                    indices[triIndex + 3] = (ushort)(vertIndex + 0);
                    indices[triIndex + 4] = (ushort)(vertIndex + 2);
                    indices[triIndex + 5] = (ushort)(vertIndex + 3);
                    
                    vertIndex += 4;
                    triIndex += 6;
                }
            }
        }

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        Raylib.UploadMesh(ref mesh, false);

        return mesh;
    }
    
    public void Update()
    {
        Raymath.Wrap(rotation += 1f, 0f, 360f);
    }

    private Vector3 Transform(int face, Vector2 pos)
    {
        Vector3 offset = new Vector3(-(float)size / 2, (float)size / 2, -(float)size / 2);
        switch (face)
        {
            case 0: return offset + new Vector3(0.0f, pos.X - (float)size, pos.Y);
            case 1: return offset + new Vector3(pos.X, 0.0f, pos.Y);
            case 2: return offset + new Vector3((float)size, ((float)size - pos.X) - (float)size, pos.Y);
            case 3: return offset + new Vector3((float)size - pos.X, -(float)size, pos.Y);
            case 4: return offset + new Vector3(pos.X, (pos.Y - (float)size), 0.0f);
            case 5: return offset + new Vector3(pos.X, ((float)size - pos.Y) - (float)size, (float)size);
        }
        return new Vector3(0);
    }

    private Vector3 TransformFlat(int face, Vector2 pos)
    {
        Vector3 result = new Vector3(pos.X, 0.0f, pos.Y);
        switch (face)
        {
            case 1:
                result += new Vector3((float)size, 0.0f, 0.0f);
                break;
            case 2:
                result += new Vector3((float)size * 2, 0.0f, 0.0f);
                break;
            case 3:
                result += new Vector3((float)size * 3, 0.0f, 0.0f);
                break;
            case 4:
                result += new Vector3((float)size, 0.0f, -(float)size);
                break;
            case 5:
                result += new Vector3((float)size, 0.0f, (float)size);
                break;
        }
        return result;
    }
}
