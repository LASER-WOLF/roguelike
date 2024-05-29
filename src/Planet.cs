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
    private Model model;
    private float rotation;

    public Planet(int size)
    {
        this.size = size;
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        this.rotation = 0.0f;
        Generate();
    }

    private unsafe void Generate()
    {
        model = Raylib.LoadModelFromMesh(GenMeshCube());
        
        Texture2D texture = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");

        //model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = texture;
    }
    
    private Vector3 Transform(int face, Vector2 pos)
    {
        float height = (float)Rand.random.Next(0, 10) * 0.1f;
        //height = 0;
        //Vector3 result = new Vector3(pos.X, height, pos.Y);
        Vector3 result = TransformToFlat(face, pos);
        result = new Vector3(result.X, height, result.Z);
        //result = TransformToCube(face, result);
        //result = CubeToSphere(result / (size / 2f)) * size;
        return result;
    }
    
    public void Update(float deltaTime)
    {
        //Raymath.Wrap(rotation += deltaTime * 5.0f, 0f, 360f);
    }

    public void Render()
    {
        Raylib.DrawModelWiresEx(model, pos, new Vector3(1.0f, 0.0f, 1.0f), rotation, Vector3.One, Color.White);
    }

    private Mesh GenMeshCube()
    {
        int numTiles = (6 * (size * size));
        int numVerts = numTiles + 6 * (size * 3);
        int numTris = 2 * numTiles;

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

        ushort vertIndex = 0;
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
                    // switch (x % 4)
                    // {
                    //     case 0: color = Color.Red; break;
                    //     case 1: color = Color.Green; break;
                    //     case 2: color = Color.Blue; break;
                    //     case 3: color = Color.Yellow; break;
                    // }
                    
                    // float texCoordLeft = x * (1f / size);
                    // float texCoordRight = (x + 1f) * (1f / size);
                    // float texCoordTop = y * (1f / size);
                    // float texCoordBottom = (y + 1f) * (1f / size);

                    // switch (face)
                    // {
                    //     case 0:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.00f, 0.33f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.00f, 0.33f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.00f, 0.50f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.00f, 0.50f);
                    //         break;
                    //     case 1:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.33f, 0.66f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.33f, 0.66f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.00f, 0.50f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.00f, 0.50f);
                    //         break;
                    //     case 2:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.66f, 1.00f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.66f, 1.00f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.00f, 0.50f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.00f, 0.50f);
                    //         break;
                    //     case 3:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.00f, 0.33f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.00f, 0.33f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.50f, 1.00f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.50f, 1.00f);
                    //         break;
                    //     case 4:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.33f, 0.66f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.33f, 0.66f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.50f, 1.00f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.50f, 1.00f);
                    //         break;
                    //     case 5:
                    //         texCoordLeft = Raymath.Remap(texCoordLeft,     0f, 1f, 0.66f, 1.00f);
                    //         texCoordRight = Raymath.Remap(texCoordRight,   0f, 1f, 0.66f, 1.00f);
                    //         texCoordTop = Raymath.Remap(texCoordTop,       0f, 1f, 0.50f, 1.00f);
                    //         texCoordBottom = Raymath.Remap(texCoordBottom, 0f, 1f, 0.50f, 1.00f);
                    //         break;
                    // }

                    float texCoordXStart = 0f;
                    float texCoordYStart = 0f;
                    float texCoordXSize = 1f / 3f;
                    float texCoordYSize = 1f / 2f;
                    switch (face)
                    {
                        case 0:
                            texCoordXStart = 0f;
                            texCoordYStart = 0f;
                            break;
                        case 1:
                            texCoordXStart = texCoordXSize;
                            texCoordYStart = 0f;
                            break;
                        case 2:
                            texCoordXStart = texCoordXSize * 2;
                            texCoordYStart = 0f;
                            break;
                        case 3:
                            texCoordXStart = 0f;
                            texCoordYStart = texCoordYSize;
                            break;
                        case 4:
                            texCoordXStart = texCoordXSize;
                            texCoordYStart = texCoordYSize;
                            break;
                        case 5:
                            texCoordXStart = texCoordXSize * 2;
                            texCoordYStart = texCoordYSize;
                            break;
                    }  

                    float texCoordLeft   = texCoordXStart + ((float)x * (texCoordXSize / (float)size));
                    float texCoordRight  = texCoordXStart + (((float)x + 1f) * (texCoordXSize / (float)size));
                    float texCoordTop    = texCoordYStart + ((float)y * (texCoordYSize / (float)size));
                    float texCoordBottom = texCoordYStart + (((float)y + 1f) * (texCoordYSize / (float)size));
                    
                    ushort vertTopLeft = (ushort)(vertIndex - (size + (x == 0 ? 1 : 2)));
                    ushort vertTopRight = (ushort)(vertTopLeft + 1);
                    ushort vertBottomLeft = (ushort)(vertIndex - 1);
                    if (y == 0)
                    {
                        vertTopLeft = (ushort)(vertIndex - (x == 1 ? 3 : 2));
                    }
                    if (y == 1)
                    {
                        vertTopLeft += (ushort)((x == 0 ? x + 1 : x) - size);
                        vertTopRight = (ushort)(vertTopLeft + (x == 0 ? 1 : 2));
                    }

                    // Top-left vertex
                    if (y == 0 && x == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x + 0, y + 0));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Top-right vertex
                    if (y == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x + 1, y + 0));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordRight, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopRight = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Bottom-left vertex
                    if (x == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x + 0, y + 1));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordBottom);
                        colors[vertIndex] = color;
                        vertBottomLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }
                    
                    // Bottom-right vertex
                    vertices[vertIndex] = Transform(face, new Vector2(x + 1, y + 1));
                    normals[vertIndex] = new(0, 1, 0);
                    texcoords[vertIndex] = new(texCoordRight, texCoordBottom);
                    colors[vertIndex] = color;
                    ushort vertBottomRight = (ushort)(vertIndex);
                    vertIndex++;

                    // Triangle 1
                    indices[triIndex + 0] = vertTopLeft;
                    indices[triIndex + 1] = vertBottomRight;
                    indices[triIndex + 2] = vertTopRight;

                    // Triangle 2
                    indices[triIndex + 3] = vertTopLeft;
                    indices[triIndex + 4] = vertBottomLeft;
                    indices[triIndex + 5] = vertBottomRight;
                    
                    triIndex += 6;
                }
            }
        }

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        Raylib.UploadMesh(ref mesh, false);

        return mesh;
    }


    private Vector3 TransformToCube(int face, Vector3 pos)
    {
        Vector3 result = Vector3.Zero;
        Vector3 offset = new Vector3(-(float)size / 2, (float)size / 2, -(float)size / 2);
        switch (face)
        {
            case 0: result = offset + new Vector3(pos.Y, pos.X - (float)size, pos.Z); break;
            case 1: result = offset + new Vector3(pos.X, pos.Y, pos.Z); break;
            case 2: result = offset + new Vector3((float)size - pos.Y, ((float)size - pos.X) - (float)size, pos.Z); break;
            case 3: result = offset + new Vector3((float)size - pos.X, -(float)size + pos.Y, pos.Z); break;
            case 4: result = offset + new Vector3(pos.X, (pos.Z - (float)size), pos.Y); break;
            case 5: result = offset + new Vector3(pos.X, ((float)size - pos.Z) - (float)size, (float)size - pos.Y); break;
        }
        //result = CubeToSphere(result / (size / 2f)) * size;
        //result = Vector3.Normalize(result) * size;
        return result;
    }

    private Vector3 CubeToSphere(Vector3 p)
    {
        float x = p.X * (float)Math.Sqrt(1f - (((p.Y * p.Y) + (p.Z * p.Z)) / (2f)) + (((p.Y * p.Y) * (p.Z * p.Z)) / (3f)));
        float y = p.Y * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Z * p.Z)) / (2f)) + (((p.X * p.X) * (p.Z * p.Z)) / (3f)));
        float z = p.Z * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Y * p.Y)) / (2f)) + (((p.X * p.X) * (p.Y * p.Y)) / (3f)));
        return new Vector3(x, y, z);
    }

    private Vector3 TransformToFlat(int face, Vector2 pos)
    {
        Vector3 result = new Vector3(pos.X - (float)size * 2, 0.0f, pos.Y - (float)size);
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
