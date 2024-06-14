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
    private Texture2D heightmapTex;
    private long seed;

    // Constructor
    public Planet(int size)
    {
        this.seed = (long)0;
        this.size = size;
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        Generate();
    }

    // Generate the planet
    private unsafe void Generate()
    {
        // Generate heightmap
        Image heightmapImage = MakeHeightmap();
        heightmapTex = Raylib.LoadTextureFromImage(heightmapImage);
        Color* heightmap = Raylib.LoadImageColors(heightmapImage);
        Raylib.UnloadImage(heightmapImage);
        // Generate mesh
        model = Raylib.LoadModelFromMesh(MakeMesh(heightmap, flat: true));
        Raylib.UnloadImageColors(heightmap);
        // Set model texture
        texture = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");
        //Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref heightmapTex);
    }
    
    // TODO: Implement this in the mesh generation
    // Find normal for a vertex
    // vN = Vector3Normalize(Vector3CrossProduct(Vector3Subtract(vB, vA), Vector3Subtract(vC, vA)));

    // TODO: Move this somewhere else!
    private float Noise3(long seed, Vector3 pos)
    {
        return ((OpenSimplex2.Noise3_Fallback(seed, (double)pos.X, (double)pos.Y, (double)pos.Z) + 1f) * 0.5f);
    }
   
    // Called every frame
    public void Update(float deltaTime)
    {
        //Rotate(new Vector3(0.1f, 0f, 0.1f) * deltaTime);
    }

    // Render 3D graphics
    public void Render3D()
    {
        float offsetX = size * 0.7f;
        Raylib.DrawModel(model, pos + new Vector3((float)size * 1f, -((float)size * 0.7f), ((float)size * 1f)), 1.0f, Color.White);
    }
    
    // Render 2D graphics
    public void Render2D()
    {
        float multiplier = 0.175f;
        Raylib.DrawTextureEx(texture, new Vector2(0f, 0f), 0f, multiplier, Color.White);
        Raylib.DrawTextureEx(heightmapTex, new Vector2(0f, Raylib.GetRenderHeight() - ((1024f * 2f) * multiplier)), 0f, (1024f / ((float) size + 1f)) * multiplier, Color.White);
        //Raylib.DrawTextEx(font, "", new Vector2(2, 20), 16, 2, Color.White);
    }

    // Free allocated memory
    public void Exit()
    {
        Raylib.UnloadTexture(texture);
        Raylib.UnloadTexture(heightmapTex);
        Raylib.UnloadModel(model);
    }

    // Rotate the planet model
    public unsafe void Rotate(Vector3 newRotation)
    {
        rotation += newRotation;
        model.Transform = Raymath.MatrixRotateXYZ(rotation);
    }

    // Returns the planet surface coordinate for a given cube face index & grid position
    private Vector2 GetCoordinate(int face, int x, int y)
    {
        float faceX = (float)x;
        float faceY = (float)y + (((float)size + 1f) * 0.5f);
        int equator = face;
        // Poles
        if (face > 3)
        {
            // Check the angle to the center of the pole
            Vector2 polePos = new Vector2(x + 0.5f, y + 0.5f);
            Vector2 poleCenter = new Vector2(((float)size + 1f) / 2f, ((float)size + 1f) / 2f);
            double radian = Math.Atan2((poleCenter.Y - polePos.Y), (poleCenter.X - polePos.X));
            float angle = (float)((radian * (180 / Math.PI) + 360) % 360);
            // North pole
            if (face == 4)
            {
                // Bottom side
                if (angle >= 225f && angle < 315f)
                { 
                    equator = 1;
                    faceX = (float)x;
                    faceY = 1f + (float)y - (((float)size + 1f) * 0.5f);
                }
                // Right side
                else if (angle >= 135f && angle < 225f)
                {
                    equator = 2;
                    faceX = (float)size - (float)y;
                    faceY = 1f + (float)x - (((float)size + 1f) * 0.5f);
                }
                // Top side
                else if (angle >= 45f && angle < 135f)
                {
                    equator = 3;
                    faceX = (float)size - (float)x;
                    faceY = (((float)size + 1f) * 0.5f) - (float)y;
                }
                // Left side
                else 
                {
                    equator = 0;
                    faceX = (float)y;
                    faceY = (((float)size + 1f) * 0.5f) - (float)x;
                }
            }
            // South pole
            else if (face == 5)
            {
                faceY = (((float)size + 1f) * 1.5f) - 1f;
                // Bottom side
                if (angle >= 225f && angle < 315f)
                { 
                    equator = 3;
                    faceX = (float)size - (float)x;
                    faceY += ((float)size - (float)y);
                }
                // Right side
                else if (angle >= 135f && angle < 225f)
                {
                    equator = 2;
                    faceX = (float)y;
                    faceY += ((float)size - (float)x);
                }
                // Top side
                else if (angle >= 45f && angle < 135f)
                {
                    equator = 1;
                    faceX = (float)x;
                    faceY += (float)y;
                }
                // Left side
                else 
                {
                    equator = 0;
                    faceX = (float)size - (float)y;
                    faceY += (float)x;
                }
            }
        }
        // Handle equator sides
        switch (equator)
        {
            case 1: faceX += (float)size; break;
            case 2: faceX += (float)size * 2f; break;
            case 3: faceX += (float)size * 3f; break;
        }
        return new Vector2(faceX, faceY);
    }

    // Returns the heightmap colorbuffer index for a given cube face index & grid position
    private int GetHeightmapIndex(int face, int x, int y)
    {
        int indexOffset = (size + 1) * (face % 3) + (face > 2 ? ((size + 1) * 3) * (size + 1) : 0);
        return indexOffset + (((size + 1) * 3) * y) + x;
    }
    
    // Procedural generation of an 8-bit/1 channel grayscale heightmap image for the planet
    private unsafe Image MakeHeightmap()
    {
        int imgWidth = (size + 1) * 3;
        int imgHeight = (size + 1) * 2;
        byte* pixels = (byte*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(byte));
        //Color* pixels = (Color*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(Color));
        for (int face = 0; face < 6; face++)
        {
            //bool[,] dlaMap = DiffusionLimitedAggregation(size + 1, size + 1, (int)(size * (size / 2f)));
            bool[,] dlaMap = DiffusionLimitedAggregation.Run(size + 1, size + 1);
            bool[,] dwMap = DrunkardsWalk.Run(size + 1, size + 1);
            bool[] caMap = CellularAutomata.Run(size + 1, size + 1);

            for (int y = 0; y < size + 1; y++)
            {
                for (int x = 0; x < size + 1; x++)
                {
                    float noiseWeight = 0.75f;
                    float squareBumpWeight = 0.25f;
                    float frequency = 1.0f; // Noise scale
                    float steepness = 0.0f; // Mountain steepness (0.1f - 10f)
                    float centerWeight = 0.5f; // 0f-1f
                    float dlaWeight = 1f;
                    
                    dlaWeight = 0f;
                    noiseWeight = 0f;
                    centerWeight = 0f; // 0f-1f
                    squareBumpWeight = 0f;
                   
                    // Set 3D position
                    Vector3 pos = TransformCubeToSphere(Transform2DToCube(face, new Vector2((float)x, (float)y))) / (size / frequency);
                    
                    // Set height to 1
                    float height = 0f;
                   
                    if (face == 0)
                    {
                        height += dlaMap[x, y] ? 1f : 0f;
                    }

                    // DLA value
                    if (dlaWeight > 0f)
                    {
                        float dla = dlaMap[x, y] ? 1f : 0f;
                        height *= (1f - dlaWeight) + (dla * dlaWeight);
                    }
                    
                    // Add noise
                    if (noiseWeight > 0f)
                    {
                        float noise = 
                            (
                                  (1.00f * Noise3(seed + (long)1, 1f * pos))
                                + (0.50f * Noise3(seed + (long)2, 2f * pos))
                                + (0.25f * Noise3(seed + (long)4, 4f * pos))
                                + (0.13f * Noise3(seed + (long)8, 8f * pos))
                                + (0.06f * Noise3(seed + (long)8, 8f * pos))
                                + (0.03f * Noise3(seed + (long)8, 8f * pos))
                            ) / (1.00f + 0.50f + 0.25f + 0.13f + 0.06f + 0.03f);
                        if (steepness > 0f) { noise = (float)Math.Pow(noise, steepness); }
                        height *= (1f - noiseWeight) + (noise * noiseWeight);
                    }
                    
                    // Multiply by distance to center (diagonal)
                    if (centerWeight > 0f) 
                    {
                        float centerDist = Math.Abs((((float)size + 1f) / 2f) - (float)x) + Math.Abs((((float)size + 1f) / 2f) - (float)y);
                        float center = (1f / ((float)size + 1f)) * ((((float)size + 1f) * 0.5f) - centerDist);
                        center = center > 0.0f ? center : 0.0f;
                        height *= (1f - centerWeight) + (center * centerWeight);
                    }
                    
                    // Square bump
                    if (squareBumpWeight > 0f){
                        float xNormalized = (((1f / (float)size) * (float)x) * 2f) - 1f;
                        float yNormalized = (((1f / (float)size) * (float)y) * 2f) - 1f;
                        float squareBump = (1f - (xNormalized * xNormalized)) * ( 1f - (yNormalized * yNormalized));
                        height *= (1f - squareBumpWeight) + (squareBumpWeight * squareBump);
                    }

                    pixels[GetHeightmapIndex(face, x, y)] = (byte)Math.Floor(height * 255f);
                    //pixels[GetHeightmapIndex(face, x, y)] = new Color(height, 0, 0, 255);
                }
            }
        }
        // Make image
        Image image = new Image
        {
            Data = pixels,
            Width = imgWidth,
            Height = imgHeight,
            Format = PixelFormat.UncompressedGrayscale,
            //Format = PixelFormat.UncompressedR8G8B8A8,
            Mipmaps = 1,
        };
        //Raylib.MemFree(pixels);
        return image;
    }

    // Transform a given cube face index & grid position to a 3D point in local space
    private unsafe Vector3 Transform2Dto3D(int face, Vector2 pos, Color* heightmap, bool flat = false)
    {
        // Get height from heightmap
        //float maxHeight = size / 4f;
        float maxHeight = size;
        int heightmapIndex = GetHeightmapIndex(face, (int)pos.X, (int)pos.Y);
        float height = Raymath.Remap((float)heightmap[heightmapIndex].R, 0f, 255f, 0f, maxHeight);
        // Flat mode
        if (flat)
        {
            return Transform2DToFlat(face, pos) + (new Vector3(0f, 1f, 0f) * height);
        }
        // Sphere mode
        Vector3 result = Transform2DToCube(face, pos);
        Vector3 normal = Vector3.Normalize(result);
        result = TransformCubeToSphere(result);
        result = result + (normal * height);
        return result;
    }

    // Project the planet as a 3D unfolded cube
    private Vector3 Transform2DToFlat(int face, Vector2 pos)
    {
        Vector3 result = new Vector3(pos.X - (float)size * 2, 0f, pos.Y - (float)size);
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

    // Project the planet as a 3D cube
    private Vector3 Transform2DToCube(int face, Vector2 pos)
    {
        Vector3 result = Vector3.Zero;
        Vector3 offset = new Vector3(-(float)size / 2, (float)size / 2, -(float)size / 2);
        switch (face)
        {
            case 0: result = offset + new Vector3(0f, pos.X - (float)size, pos.Y); break;
            case 1: result = offset + new Vector3(pos.X, 0f, pos.Y); break;
            case 2: result = offset + new Vector3((float)size, ((float)size - pos.X) - (float)size, pos.Y); break;
            case 3: result = offset + new Vector3((float)size - pos.X, -(float)size, pos.Y); break;
            case 4: result = offset + new Vector3(pos.X, (pos.Y - (float)size), 0f); break;
            case 5: result = offset + new Vector3(pos.X, ((float)size - pos.Y) - (float)size, (float)size); break;
        }
        return result;
    }

    // Cube to sphere projection
    private Vector3 TransformCubeToSphere(Vector3 p)
    {
        p /= size / 2f;
        float x = p.X * (float)Math.Sqrt(1f - (((p.Y * p.Y) + (p.Z * p.Z)) / (2f)) + (((p.Y * p.Y) * (p.Z * p.Z)) / (3f)));
        float y = p.Y * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Z * p.Z)) / (2f)) + (((p.X * p.X) * (p.Z * p.Z)) / (3f)));
        float z = p.Z * (float)Math.Sqrt(1f - (((p.X * p.X) + (p.Y * p.Y)) / (2f)) + (((p.X * p.X) * (p.Y * p.Y)) / (3f)));
        return new Vector3(x, y, z) * (size * 0.75f);
    }

    // Generate the 3D mesh for the planet
    private unsafe Mesh MakeMesh(Color* heightmap, bool flat = true)
    {
        // Set mesh specs
        int faceNumVerts = (size + 1) * (size + 1);
        int numVerts = 6 * faceNumVerts;
        int numTris = 2 * (6 * (size * size));
        
        // Allocate memory for the mesh
        Mesh mesh = new(numVerts, numTris);
        mesh.AllocVertices();
        mesh.AllocTexCoords();
        mesh.AllocNormals();
        mesh.AllocColors();
        mesh.AllocIndices();
        
        // Contigous regions of memory set aside for mesh data
        Span<Vector3> vertices = mesh.VerticesAs<Vector3>();
        Span<Vector2> texcoords = mesh.TexCoordsAs<Vector2>();
        Span<Color> colors = mesh.ColorsAs<Color>();
        Span<ushort> indices = mesh.IndicesAs<ushort>();
        Span<Vector3> normals = mesh.NormalsAs<Vector3>();
        
        // Make lookup table for cube face vert index start position
        int[] faceVertIndex = new int[6];
        for (int face = 0; face < 6; face++){
            faceVertIndex[face] = faceNumVerts * face;
        }

        // Loop through all cube faces
        ushort vertIndex = 0;
        int triIndex = 0;
        Color color = Color.White;
        for (int face = 0; face < 6; face++)
        {
            
            // switch (face)
            // {
            //     case 0: color = Color.SkyBlue; break;
            //     case 1: color = Color.Red; break;
            //     case 2: color = Color.Yellow; break;
            //     case 3: color = Color.Green; break;
            //     case 4: color = Color.Purple; break;
            //     case 5: color = Color.Beige; break;
            // }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Set UV cordinates for vertices on the current grid position
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
                    
                    // Set vertex indexes
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
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x, y), heightmap, flat: flat);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make top-right vertex
                    if (y == 0)
                    {
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x + 1, y), heightmap, flat: flat);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordRight, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopRight = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make bottom-left vertex
                    if (x == 0)
                    {
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x, y + 1), heightmap, flat: flat);
                        normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordBottom);
                        colors[vertIndex] = color;
                        vertBottomLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }
                    
                    // Make bottom-right vertex
                    vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x + 1, y + 1), heightmap, flat: flat);
                    normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                    texcoords[vertIndex] = new(texCoordRight, texCoordBottom);
                    colors[vertIndex] = color;
                    ushort vertBottomRight = (ushort)(vertIndex);
                    vertIndex++;

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

        // Return the finished mesh
        return mesh;
    }
}
