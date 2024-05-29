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
    
    public void Update(float deltaTime)
    {
        Raymath.Wrap(rotation += deltaTime * 5.0f, 0f, 360f);
    }

    public void Render()
    {
        Raylib.DrawModelWiresEx(model, pos, new Vector3(1.0f, 0.0f, 1.0f), rotation, Vector3.One, Color.White);
    }

    private Mesh GenMeshCube()
    {
        int numTiles = (6 * (size * size));
        int numVerts = 2 * numTiles + 6 * (size * 2);
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
            //ushort faceVertIndex = vertIndex;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    switch (x % 4)
                    {
                        case 0: color = Color.Red; break;
                        case 1: color = Color.Green; break;
                        case 2: color = Color.Blue; break;
                        case 3: color = Color.Yellow; break;

                    }

                    float texCoordLeft = x * (x / size);
                    float texCoordRight = (x + 1) * (x / size);
                    float texCoordTop = y * (y / size);
                    float texCoordBottom = (y + 1) * (y / size);

                    //ushort vertTopLeft =  (ushort)(faceVertIndex + (((y * size) + x) + (x == 0 ? 2 : 1)));
                    
                    ushort vertTopLeft =  (ushort)(vertIndex - (size + (x == 0 ? 1 : 2)));
                    
                    ushort vertBottomLeft =  (ushort)(vertIndex - 1);
                    
                    //ushort vertBottomLeft =  (ushort)(vertIndex - ((x + (size - x)) * 1) + 2);

                    if (y == 0)
                    {
                        // Vertex 1 - Top-left
                        vertices[vertIndex] = Transform(face, new Vector2(x + 0, y + 0));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopLeft = (ushort)(vertIndex);
                        vertIndex++;
                        
                        // Vertex 2 - Top-right
                        vertices[vertIndex] = Transform(face, new Vector2(x + 1, y + 0));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordRight, texCoordTop);
                        colors[vertIndex] = color;
                        vertIndex++;
                    }
                    
                    ushort vertTopRight = (ushort)(vertTopLeft + 1);
                    
                    if (y == 1)
                    {
                        //vertTopLeft = (ushort)((faceVertIndex + (x * 4 + 2)));
                        //vertTopLeft = (ushort)(vertIndex - (((x * 2) + ((size - x) * 4) - 2)));
                        //vertTopLeft =  (ushort)(vertIndex - ((x + (size - x) + (x == 0 ? 1 : 2) ))); 
                        
                        //vertTopLeft =  (ushort)(vertIndex - (x + ((size - x) * 3)  - (x == 0 ? 1 : 0)));
                        //vertTopLeft =  (ushort)(faceVertIndex + (x * 2) + (x == 0 ? 2 : 3));
                        //vertTopRight = (ushort)(vertTopLeft + 2);
                        vertTopLeft -= (ushort)((size - x) * 2 - (x == 0 ? 2 : 0));
                        vertTopRight = (ushort)(vertTopLeft + 3 - (x == 0 ? 2 : 0));
                    }
                    


                    if (x == 0)
                    {
                        // Vertex 3 - Bottom-left
                        vertices[vertIndex] = Transform(face, new Vector2(x + 0, y + 1));
                        normals[vertIndex] = new(0, 1, 0);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordBottom);
                        colors[vertIndex] = color;
                        vertBottomLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }
                    
                    // Vertex 4 - Bottom-right
                    vertices[vertIndex] = Transform(face, new Vector2(x + 1, y + 1));
                    normals[vertIndex] = new(0, 1, 0);
                    texcoords[vertIndex] = new(texCoordRight, texCoordBottom);
                    colors[vertIndex] = color;

                    // Triangle 1
                    indices[triIndex + 0] = vertTopLeft;
                    indices[triIndex + 1] = (ushort)(vertIndex);
                    indices[triIndex + 2] = vertTopRight;

                    // Triangle 2
                    indices[triIndex + 3] = vertTopLeft;
                    indices[triIndex + 4] = vertBottomLeft;
                    indices[triIndex + 5] = (ushort)(vertIndex);
                    
                    vertIndex += 1;
                    triIndex += 6;
                }
            }
        }

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        Raylib.UploadMesh(ref mesh, false);

        return mesh;
    }

    private Vector3 Transform(int face, Vector2 pos)
    {
        return TransformCube(face, pos);
    }

    private Vector3 TransformCube(int face, Vector2 pos)
    {
        Vector3 result = Vector3.Zero;
        Vector3 offset = new Vector3(-(float)size / 2, (float)size / 2, -(float)size / 2);
        switch (face)
        {
            case 0: result = offset + new Vector3(0.0f, pos.X - (float)size, pos.Y); break;
            case 1: result = offset + new Vector3(pos.X, 0.0f, pos.Y); break;
            case 2: result = offset + new Vector3((float)size, ((float)size - pos.X) - (float)size, pos.Y); break;
            case 3: result = offset + new Vector3((float)size - pos.X, -(float)size, pos.Y); break;
            case 4: result = offset + new Vector3(pos.X, (pos.Y - (float)size), 0.0f); break;
            case 5: result = offset + new Vector3(pos.X, ((float)size - pos.Y) - (float)size, (float)size); break;
        }
        result = CubeToSphere(result / (size / 2f)) * size;
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

    private Vector3 TransformFlat(int face, Vector2 pos)
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
