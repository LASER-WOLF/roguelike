using Raylib_cs;
using System.Numerics;

namespace Core;

/// <summary>
/// A round astronomical body.
/// </summary>
public class Planet
{

    public Vector3 pos { get; private set; }
    public readonly int size;
    private Model model;
    private Vector3 rotation = new Vector3(0f, 0f, 0f);
    private Texture2D texture;

    public Planet(int size)
    {
        this.size = size;
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        Generate();
    }

    private unsafe void Generate()
    {
        // Load heightmap
        Image heightmapImage = Raylib.LoadImage("./assets/textures/heightmap_11x11_test.png");
        Color* heightmap = Raylib.LoadImageColors(heightmapImage);
        Raylib.UnloadImage(heightmapImage);
        // Generate model
        model = Raylib.LoadModelFromMesh(MakeMesh(heightmap));
        Raylib.UnloadImageColors(heightmap);
        // Set texture
        texture = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);
    }

    public void Exit()
    {
        Raylib.UnloadTexture(texture);
        Raylib.UnloadModel(model);
    }
    
    private unsafe Vector3 Transform(int face, Vector2 pos, Color* heightmap, bool sphere = true)
    {
        // Giet height from heightmap
        float hmMaxHeight = 1f;
        int hmIndexOffset = (size + 1) * (face % 3) + (face > 2 ? ((size + 1) * 3) * (size + 1) : 0);
        int hmIndex = hmIndexOffset + (((size + 1) * 3) * (int)pos.Y) + (int)pos.X;
        float height = Raymath.Remap((float)((heightmap[hmIndex].R + heightmap[hmIndex].G + heightmap[hmIndex].B) / 3f), 0f, 255f, hmMaxHeight, 0f);
        Vector3 newPos = new Vector3(pos.X, height, pos.Y);

        // Sphere mode
        if (sphere)
        {
            return TransformFaceToCube(face, newPos);
            //return TransformCubeToSphere(TransformFaceToCube(face, newPos));
        }
        
        // Flat mode
        return TransformFaceToFlat(face, newPos);
    }
   
    public unsafe void Rotate(Vector3 newRotation)
    {
        rotation += newRotation;
        model.Transform = Raymath.MatrixRotateXYZ(rotation);
    }

    public void Update(float deltaTime)
    {
        // Rotate(new Vector3(0.1f, 0f, 0.1f) * deltaTime);
    }

    public void Render()
    {
        Raylib.DrawModel(model, pos, 1.0f, Color.White);
    }

    private unsafe Mesh MakeMesh(Color* heightmap, bool sphere = true)
    {
        int faceNumVerts = (size + 1) * (size + 1);
        int numVerts = 6 * faceNumVerts;
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

        ushort vertIndex = 0;
        int triIndex = 0;
        Color color = Color.White;
        
        int[] faceVertIndex = new int[6];
        for (int face = 0; face < 6; face++){
            faceVertIndex[face] = faceNumVerts * face;
        }

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
                    // Set UV corrdinates for verts
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
                    
                    // Set verts (by index)
                    int vertIndexStart = vertIndex;
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

                    // Make top-left vertex
                    if (y == 0 && x == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x, y), heightmap, sphere);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make top-right vertex
                    if (y == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x + 1, y), heightmap, sphere);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordRight, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopRight = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make bottom-left vertex
                    if (x == 0)
                    {
                        vertices[vertIndex] = Transform(face, new Vector2(x, y + 1), heightmap, sphere: sphere);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordBottom);
                        colors[vertIndex] = color;
                        vertBottomLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }
                    
                    // Make bottom-right vertex
                    vertices[vertIndex] = Transform(face, new Vector2(x + 1, y + 1), heightmap, sphere: sphere);
                    normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                    texcoords[vertIndex] = new(texCoordRight, texCoordBottom);
                    colors[vertIndex] = color;
                    ushort vertBottomRight = (ushort)(vertIndex);
                    vertIndex++;

                    /*
                    // Set rules for the faces
                    bool makeTop = false;
                    bool makeRight = false;
                    bool makeBottom = false;
                    bool makeLeft = false;
                    bool makeCorners = false;
                    switch (face)
                    {
                        // Face 0
                        case 0:
                            // Make top and bottom edges
                            makeTop = true;
                            makeBottom = true;
                            // Get left edge from right edge of face 3
                            if (x == 0)
                            {
                                int targetVertIndex = faceVertIndex[3] + (vertIndexStart - faceVertIndex[face]);
                                int targetVertOffsetTop = -1;
                                int targetVertOffsetBottom = size;
                                if (y == 0)
                                { 
                                    targetVertOffsetTop = size + size; 
                                    targetVertOffsetBottom = size + size + 1;
                                }
                                vertTopLeft = (ushort)(targetVertIndex + targetVertOffsetTop);
                                vertBottomLeft = (ushort)(targetVertIndex + targetVertOffsetBottom);
                            }
                            // Get right edge from left edge of face 1
                            if (x == size - 1)
                            {
                                vertTopRight = (ushort)(faceVertIndex[1] + (y == 1 ? 2 : ((size + 1) * y)));
                                vertBottomRight = (ushort)(vertTopRight + (y == 0 ? 2 : y == 1 ? (size * 2) : (size + 1)));
                            }
                            // Top-left corner from top-right corner of face 3
                            if (y == 0 && x == 0) { vertTopLeft = (ushort)(faceVertIndex[3] + (size * 2)); }
                            // Top-right corner from top-left corner of face 1
                            if (y == 0 && x == size -1) { vertTopRight = (ushort)faceVertIndex[1]; }
                            // Bottom-left corner from bottom-right corner of face 3
                            if (y == size - 1 && x == 0) { vertBottomLeft = (ushort)(faceVertIndex[3] + ((size + 1) * (size + 1)) - 1); }
                            // Bottom-right corner from bottom-left corner of face 1
                            if (y == size - 1 && x == size -1) { vertBottomRight = (ushort)(faceVertIndex[1] + ((size + 1) * (size + 1)) - (size + 1)); }
                            break;

                        // Face 1
                        case 1:
                            // Make left and right edges
                            makeLeft = true;
                            makeRight = true;
                            // Make all corners
                            makeCorners = true;
                            // Get the top edge from bottom edge of face 4
                            if (y == 0)
                            {
                                int targetVertIndex = faceVertIndex[4] + (vertIndexStart - faceVertIndex[face]);
                                int targetVertOffsetLeft = (size * (size + 1)) - (x == 0 ? 0 : x + 2);
                                int targetVertOffsetRight = targetVertOffsetLeft + 1;
                                vertTopLeft = (ushort)(targetVertIndex + targetVertOffsetLeft);
                                vertTopRight = (ushort)(targetVertIndex + targetVertOffsetRight);
                            }
                            // Get bottom edge from top edge of face 5
                            if (y == size - 1)
                            {
                                vertBottomLeft = (ushort)(faceVertIndex[5] + (x * (x == 1 ? 1 : 2)));
                                vertBottomRight = (ushort)(vertBottomLeft + (x == 0 ? 0 : 1) + (x == 1 ? 2 : 1));
                            }
                            break;

                        // Face 2
                        case 2:
                            // Make top and bottom edges
                            makeTop = true;
                            makeBottom = true;
                            // Get left edge from right edge of face 1
                            if (x == 0)
                            {
                                int targetVertIndex = faceVertIndex[1] + (vertIndexStart - faceVertIndex[face]);
                                int targetVertOffsetTop = -1;
                                int targetVertOffsetBottom = size;
                                if (y == 0)
                                { 
                                    targetVertOffsetTop = size + size; 
                                    targetVertOffsetBottom = size + size + 1;
                                }
                                vertTopLeft = (ushort)(targetVertIndex + targetVertOffsetTop);
                                vertBottomLeft = (ushort)(targetVertIndex + targetVertOffsetBottom);
                            }
                            // Get right edge from left edge of face 3
                            if (x == size - 1)
                            {
                                vertTopRight = (ushort)(faceVertIndex[3] + (y == 1 ? 2 : ((size + 1) * y)));
                                vertBottomRight = (ushort)(vertTopRight + (y == 0 ? 2 : y == 1 ? (size * 2) : (size + 1)));
                            }
                            // Top-left corner from top-right corner of face 1
                            if (y == 0 && x == 0) { vertTopLeft = (ushort)(faceVertIndex[1] + (size * 2)); }
                            // Top-right corner from top-left corner of face 3
                            if (y == 0 && x == size -1) { vertTopRight = (ushort)faceVertIndex[3]; }
                            // Bottom-left corner from bottom-right corner of face 1
                            if (y == size - 1 && x == 0) { vertBottomLeft = (ushort)(faceVertIndex[1] + ((size + 1) * (size + 1)) - 1); }
                            // Bottom-right corner from bottom-left corner of face 3
                            if (y == size - 1 && x == size -1) { vertBottomRight = (ushort)(faceVertIndex[3] + ((size + 1) * (size + 1)) - (size + 1)); }
                            break;

                        case 3:
                            // Make left and right edgees
                            makeLeft = true;
                            makeRight = true;
                            // Make all corners
                            makeCorners = true;
                            // Get top edge from top edge of face 4
                            if (y == 0)
                            {
                                vertTopLeft = (ushort)(faceVertIndex[4] + (2 * size) - (x * 2) - (x == size - 1 ? 1 : 0));
                                vertTopRight = (ushort)(vertTopLeft - (x == size - 1 ? 0 : 1) - (x == size - 2 ? 2 : 1));
                            }
                            // Get bottom edge from bottom edge of face 5
                            if (y == size - 1)
                            {
                                vertBottomLeft = (ushort)(faceVertIndex[5] + ((size + 1) * (size + 1)) - 1 - x);
                                vertBottomRight = (ushort)(vertBottomLeft - 1);
                            }
                            break;

                        case 4:
                            // Make top and bottom edges
                            makeTop = true;
                            makeBottom = true;
                            // Get left edge from top edge of face 0
                            if (x == 0)
                            {
                                vertTopLeft = (ushort)(faceVertIndex[0] + (y * (y == 1 ? 1 : 2)));
                                vertBottomLeft = (ushort)(vertTopLeft + (y == 0 ? 0 : 1) + (y == 1 ? 2 : 1));
                            }
                            // Get right edge from top edge of face 2
                            if (x == size - 1)
                            {
                                vertTopRight = (ushort)(faceVertIndex[2] + (2 * size) - (y * 2) - (y == size - 1 ? 1 : 0));
                                vertBottomRight = (ushort)(vertTopRight - (y == size - 1 ? 0 : 1) - (y == size - 2 ? 2 : 1));
                            }
                            // Top-left corner from top-right corner of face 3
                            if (y == 0 && x == 0) { vertTopLeft = (ushort)(faceVertIndex[3] + (size * 2)); }
                            // Top-right corner from top-left corner of face 3
                            if (y == 0 && x == size -1) { vertTopRight = (ushort)faceVertIndex[3]; }
                            // Bottom-left corner from top-left corner of face 1
                            if (y == size - 1 && x == 0) { vertBottomLeft = (ushort)(faceVertIndex[1]); }
                            // Bottom-right corner from top-right corner of face 1
                            if (y == size - 1 && x == size -1) { vertBottomRight = (ushort)(faceVertIndex[1] + (size * 2)); }
                            break;

                        case 5:
                            // Make top and bottom edges
                            makeTop = true;
                            makeBottom = true;
                            // Get left edge from bottom edge of face 0
                            if (x == 0)
                            {
                                vertTopLeft = (ushort)(faceVertIndex[0] + ((size + 1) * (size + 1)) - 1 - y);
                                vertBottomLeft = (ushort)(vertTopLeft - 1);
                            }
                            // Get right edge from bottom edge of face 2
                            else if (x == size -1)
                            {
                                vertTopRight = (ushort)(faceVertIndex[2] + ((size + 1) * (size + 1)) - (size + 1) + y);
                                vertBottomRight = (ushort)(vertTopRight + 1);
                            }
                            // Top-left corner from bottom-left corner of face 1
                            if (y == 0 && x == 0) { vertTopLeft = (ushort)(faceVertIndex[1] + ((size + 1) * (size + 1)) - (size + 1)); }
                            // Top-right corner from bottom-right corner of face 1
                            if (y == 0 && x == size -1) { vertTopRight = (ushort)(faceVertIndex[1] + ((size + 1) * (size + 1)) - 1); }
                            // Bottom-left corner from bottom-right corner of face 3
                            if (y == size - 1 && x == 0) { vertBottomLeft = (ushort)(faceVertIndex[3] + ((size + 1) * (size + 1)) - 1); }
                            // Bottom-right corner from bottom-left corner of face 3
                            if (y == size - 1 && x == size -1) { vertBottomRight = (ushort)(faceVertIndex[3] + ((size + 1) * (size + 1)) - (size + 1)); }
                            break;
                    }
                    */

                    // Triangle 1 (Counter-clockwise winding order)
                    indices[triIndex]     = vertTopLeft;
                    indices[triIndex + 1] = vertBottomRight;
                    indices[triIndex + 2] = vertTopRight;
                    triIndex += 3;

                    // Triangle 2 (Counter-clockwise winding order)
                    indices[triIndex]     = vertTopLeft;
                    indices[triIndex + 1] = vertBottomLeft;
                    indices[triIndex + 2] = vertBottomRight;
                    triIndex += 3;
                }
            }
        }

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        Raylib.UploadMesh(ref mesh, false);

        return mesh;
    }

    private Vector3 TransformFaceToFlat(int face, Vector3 pos)
    {
        Vector3 result = new Vector3(pos.X - (float)size * 2, pos.Y, pos.Z - (float)size);
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


    private Vector3 TransformFaceToCube(int face, Vector3 pos)
    {
        Vector3 result = Vector3.Zero;
        Vector3 offset = new Vector3(-(float)size / 2, (float)size / 2, -(float)size / 2);
        switch (face)
        {
            case 0: result = offset + new Vector3(-pos.Y, pos.X - (float)size, pos.Z); break;
            case 1: result = offset + new Vector3(pos.X, pos.Y, pos.Z); break;
            case 2: result = offset + new Vector3((float)size + pos.Y, ((float)size - pos.X) - (float)size, pos.Z); break;
            case 3: result = offset + new Vector3((float)size - pos.X, -(float)size - pos.Y, pos.Z); break;
            case 4: result = offset + new Vector3(pos.X, (pos.Z - (float)size), -pos.Y); break;
            case 5: result = offset + new Vector3(pos.X, ((float)size - pos.Z) - (float)size, (float)size + pos.Y); break;
        }
        return result;
    }

    private Vector3 TransformCubeToSphere(Vector3 p)
    {
        p /= size / 2f;
        float x = p.X * (float)Math.Sqrt(1f - (((p.Y * p.Y) + (p.Z * p.Z)) / (2f)) + (((p.Y * p.Y) * (p.Z * p.Z)) / (3f)));
        float y = p.Y * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Z * p.Z)) / (2f)) + (((p.X * p.X) * (p.Z * p.Z)) / (3f)));
        float z = p.Z * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Y * p.Y)) / (2f)) + (((p.X * p.X) * (p.Y * p.Y)) / (3f)));
        return new Vector3(x, y, z) * (size * 0.75f);
    }
}
