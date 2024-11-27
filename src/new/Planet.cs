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
    //private Model[] models = new Model[6];
    private Model model;
    private Vector3 rotation = new Vector3(0f, 0f, 0f);
    //private Texture2D texture;
    private Texture2D heightmapTex;
    private uint seed;

    // Constructor
    public Planet(int size)
    {
        //this.seed = 0B_00000000_00000000_00000000_00000001;
        this.seed = (uint)DateTime.Now.Ticks;
        this.size = size;
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        Generate();
    }

    // Generate the planet
    private unsafe void Generate()
    {
        Logger.Log("Generating planet with seed: " + seed.ToString());

        // Generate heightmap
        Image heightmapImage = MakeHeightmap();
        heightmapTex = Raylib.LoadTextureFromImage(heightmapImage);
        Color* heightmap = Raylib.LoadImageColors(heightmapImage);
        Raylib.UnloadImage(heightmapImage);
        
        // Generate mesh
        model = Raylib.LoadModelFromMesh(MakeMesh(heightmap, renderSize: 100, flat: false));
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref heightmapTex);
        // for (int face = 0; face < 6; face++)
        // {
        //     models[face] = Raylib.LoadModelFromMesh(MakeMeshFace(face, heightmap, flat: false));
        //     Raylib.SetMaterialTexture(ref models[face], 0, MaterialMapIndex.Albedo, ref heightmapTex);
        // }
        Raylib.UnloadImageColors(heightmap);
        
        // Set model texture
        //texture = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");
        //Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);
    }
    
    // TODO: Implement this in the mesh generation
    // Find normal for a vertex
    // vN = Vector3Normalize(Vector3CrossProduct(Vector3Subtract(vB, vA), Vector3Subtract(vC, vA)));
   
    // Called every frame
    public void Update(float deltaTime)
    {
        Rotate(new Vector3(0.1f, 0f, 0.1f) * deltaTime);
    }

    // Render 3D graphics
    public void Render3D()
    {
        float offsetX = size * 0.7f;
        Raylib.DrawModel(model, pos + new Vector3(offsetX, 0f, 0f), 1.0f, Color.White);
        //Raylib.DrawSphereEx(pos + new Vector3(offsetX, 0f, 0f), size * 0.775f, 20, 20, new Color(0, 82, 172, 60));
    }
    
    // Render 2D graphics
    public void Render2D()
    {
        float multiplier = 0.175f;
        //Raylib.DrawTextureEx(texture, new Vector2(0f, 0f), 0f, multiplier, Color.White);
        Raylib.DrawTextureEx(heightmapTex, new Vector2(0f, Raylib.GetRenderHeight() - ((1024f * 2f) * multiplier)), 0f, (1024f / ((float) size + 1f)) * multiplier, Color.White);
        //Raylib.DrawTextEx(font, "", new Vector2(2, 20), 16, 2, Color.White);
    }

    // Free allocated memory
    public void Exit()
    {
        //Raylib.UnloadTexture(texture);
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

    // Place regions on the planet surface
    private void PlaceRegions(int num, ref Vector3[] positions, ref int[] heights, uint maxHeight = 255)
    {
        float minDistance = 1f / num;
        for (int i = 0; i < num; i++)
        {
            bool placed = false;
            while (!placed)
            {
                // Select a random point
                Vector3 pos = Vector3.Normalize(new Vector3((float)Lfsr.MakeInt(ref seed, true), (float)Lfsr.MakeInt(ref seed, true), (float)Lfsr.MakeInt(ref seed, true)));
                
                // Discard the region if position already is occupied
                bool discard = false;
                foreach (Vector3 oldPos in positions) { if (Vector3.Distance(pos, oldPos) < minDistance) { discard = true; Logger.Err("Planet region discarded (too close)"); break; } }
                
                // Add the region if the position is free
                if (!discard)
                { 
                    positions[i] = pos;
                    heights[i] = Lfsr.MakeInt(ref seed, max: maxHeight);
                    placed = true; 
                }
            }
        }
    }

    // Find the three closest regions from a given point on the planet surface
    private void FindRegions(Vector3 pos, int num, ref Vector3[] positions, ref float[] distances, ref int[] regions)
    {
        bool[] found = new bool[3];
        for (int i = 0; i < num; i++)
        {
            float distance = Vector3.Distance(pos, positions[i]);
            if (!found[0] || distance < distances[0])
            {
                if (found[0])
                {
                    if (found[1])
                    {
                        found[2] = true;
                        regions[2] = regions[1];
                        distances[2] = distances[1];
                    }
                    found[1] = true;
                    regions[1] = regions[0];
                    distances[1] = distances[0];
                }
                found[0] = true;
                regions[0] = i;
                distances[0] = distance;
            }
            if (i != regions[0] && (!found[1] || distance < distances[1]))
            {
                found[1] = true;
                regions[1] = i;
                distances[1] = distance;
            }
            if (i != regions[0] && i != regions[1] && (!found[2] || distance < distances[2]))
            {
                found[2] = true;
                regions[2] = i;
                distances[2] = distance;
            }
        }
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
        // Set up planet prefs
        uint minNumContinents           = 4;
        uint maxNumContinents           = 32;
        uint minNumFragments            = 8;
        uint maxNumFragments            = 64;
        // Border widths 
        float continentBorderRatioWidth = 0.25f;
        float fragmentBorderRatioWidth  = 0.65f;
        // Noise set up
        float continentNoiseAmount      = 0.85f;
        float continentNoiseSize        = 0.50f * 0.008f * size;
        float fragmentNoiseAmount       = 0.85f;
        float fragmentNoiseSize         = 0.60f * 0.006f * size;
        float fractalNoiseSize          = 0.75f * size;
        // Height ratios
        float heightPercentNoise        = 0.50f * 0.1f;
        float heightPercentFragment     = 0.50f;
        float heightPercentContinent    = 1.0f - heightPercentNoise - heightPercentFragment;

        // Create pixel array for the heightmap image
        int imgWidth = (size + 1) * 3;
        int imgHeight = (size + 1) * 2;
        byte* pixels = (byte*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(byte));
        //Color* pixels = (Color*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(Color));
        
        // Generate noise seeds
        long continentNoiseSeed         = Lfsr.MakeInt(ref seed);
        long fragmentNoiseSeed          = Lfsr.MakeInt(ref seed);
        long fractalNoiseSeed           = Lfsr.MakeInt(ref seed);
        
        // Set up continents
        int numContinents = Lfsr.MakeInt(ref seed, min: minNumContinents, max: maxNumContinents);
        Logger.Log("Number of continents: " + numContinents.ToString());
        Vector3[] continentPositions = new Vector3[numContinents];
        int[] continentHeights = new int[numContinents];
        PlaceRegions(numContinents, ref continentPositions, ref continentHeights);

        // Set up fragments
        int numFragments = Lfsr.MakeInt(ref seed, min: minNumFragments, max: maxNumFragments);
        Logger.Log("Number of fragments: " + numFragments.ToString());
        Vector3[] fragmentPositions = new Vector3[numFragments];
        int[] fragmentHeights = new int[numFragments];
        PlaceRegions(numFragments, ref fragmentPositions, ref fragmentHeights);
        
        // Go through every position on every face of the cube one by one
        for (int face = 0; face < 6; face++)
        {
            for (int y = 0; y < size + 1; y++)
            {
                for (int x = 0; x < size + 1; x++)
                {
                    // Set 3D position
                    Vector3 pos = Vector3.Normalize(TransformCubeToSphere(Transform2DToCube(face, new Vector2((float)x, (float)y))));

                    // Set continent position
                    float continentNoise = 
                        (
                            (1.00f * Noise.Simplex3(continentNoiseSeed, 1f  * (pos * continentNoiseSize)))
                          + (0.50f * Noise.Simplex3(continentNoiseSeed, 2f  * (pos * continentNoiseSize)))
                          + (0.25f * Noise.Simplex3(continentNoiseSeed, 4f  * (pos * continentNoiseSize)))
                          + (0.13f * Noise.Simplex3(continentNoiseSeed, 8f  * (pos * continentNoiseSize)))
                          + (0.06f * Noise.Simplex3(continentNoiseSeed, 16f * (pos * continentNoiseSize)))
                          + (0.03f * Noise.Simplex3(continentNoiseSeed, 32f * (pos * continentNoiseSize)))
                        ) / (1.00f + 0.50f + 0.25f + 0.13f + 0.06f + 0.03f);
                    Vector3 continentPos = pos + (new Vector3(continentNoiseAmount) * ((continentNoise - 0.5f) * 2f));
                    
                    // Set fragment position
                    float fragmentNoise = 
                        (
                            (1.00f * Noise.Simplex3(fragmentNoiseSeed, 1f  * (pos * fragmentNoiseSize)))
                          + (0.50f * Noise.Simplex3(fragmentNoiseSeed, 2f  * (pos * fragmentNoiseSize)))
                          + (0.25f * Noise.Simplex3(fragmentNoiseSeed, 4f  * (pos * fragmentNoiseSize)))
                          + (0.13f * Noise.Simplex3(fragmentNoiseSeed, 8f  * (pos * fragmentNoiseSize)))
                          + (0.06f * Noise.Simplex3(fragmentNoiseSeed, 16f * (pos * fragmentNoiseSize)))
                          + (0.03f * Noise.Simplex3(fragmentNoiseSeed, 32f * (pos * fragmentNoiseSize)))
                        ) / (1.00f + 0.50f + 0.25f + 0.13f + 0.06f + 0.03f);
                     Vector3 fragmentPos = pos + (new Vector3(fragmentNoiseAmount) * ((fragmentNoise - 0.5f) * 2f));
                    
                    // Create fractal noise
                    float fractalNoise = 
                        (
                            (1.00f * Noise.Simplex3(fractalNoiseSeed, 1f * (pos * fractalNoiseSize)))
                          + (0.50f * Noise.Simplex3(fractalNoiseSeed, 2f * (pos * fractalNoiseSize)))
                          + (0.25f * Noise.Simplex3(fractalNoiseSeed, 4f * (pos * fractalNoiseSize)))
                        ) / (1.00f + 0.50f + 0.25f);

                    // Find the three closest continents from the current position on the planet surface
                    int[] continents = new int[3];
                    float[] continentDistances = new float[3];
                    FindRegions(continentPos, numContinents, ref continentPositions, ref continentDistances, ref continents);
                    
                    // Find the three closest fragments from the current position on the planet surface
                    int[] fragments = new int[3];
                    float[] fragmentDistances = new float[3];
                    FindRegions(fragmentPos, numFragments, ref fragmentPositions, ref fragmentDistances, ref fragments);
                    
                    // Set continent height
                    float continentBorderRatio = continentDistances[0] / continentDistances[1];
                    float continentHeightMultiplier = (continentBorderRatio > 1.0f - continentBorderRatioWidth) ? Smoothstep.QuadraticRational((continentBorderRatio - (1.0f - continentBorderRatioWidth)) / continentBorderRatioWidth) : 0.0f;
                    float continentHeight = (float)continentHeights[continents[0]] * (1f - continentHeightMultiplier) + (((float)continentHeights[continents[0]] + (float)continentHeights[continents[1]]) / 2.0f) * continentHeightMultiplier;
                    
                    // Set fragment height
                    float fragmentBorderRatio = fragmentDistances[0] / fragmentDistances[1];
                    float fragmentHeightMultiplier = (fragmentBorderRatio > 1.0f - fragmentBorderRatioWidth) ? Smoothstep.QuadraticRational((fragmentBorderRatio - (1.0f - fragmentBorderRatioWidth)) / fragmentBorderRatioWidth) : 0.0f;
                    float fragmentHeight = (float)fragmentHeights[fragments[0]] * (1f - fragmentHeightMultiplier) + (((float)fragmentHeights[fragments[0]] + (float)fragmentHeights[fragments[1]]) / 2.0f) * fragmentHeightMultiplier;
                    
                    // Set height
                    int height = (int)((fragmentHeight * (heightPercentFragment * (0.15f + Smoothstep.QuadraticRational((continentHeight / 255f)) * 0.85f))) + (continentHeight * heightPercentContinent) + ((fractalNoise * 255f) * heightPercentNoise));
                    
                    // Set height in the color array
                    pixels[GetHeightmapIndex(face, x, y)] = (byte)height;
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
        //float maxHeight = size * 0.1f;
        float maxHeight = size * 0.08f;
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
    private unsafe Mesh MakeMesh(Color* heightmap, int renderSize = 100, bool flat = true)
    {
        float sizeRatio = (float)renderSize / (float)size;
        float tileSize = 1.0f / sizeRatio;

        //renderSize = size;

        // Set mesh specs
        int faceNumVerts = (renderSize + 1) * (renderSize + 1);
        int numVerts = 6 * faceNumVerts;
        int numTris = 2 * (6 * (renderSize * renderSize));
        
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
        //Span<Vector3> normals = mesh.NormalsAs<Vector3>();
        
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

            for (int y = 0; y < renderSize; y++)
            {
                //int yPos = y / sizeRatio;
                for (int x = 0; x < renderSize; x++)
                {
                    //int xPos = x / sizeRatio;
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
                    float texCoordLeft   = texCoordXStart + ((float)x * (texCoordXSize / (float)renderSize));
                    float texCoordRight  = texCoordXStart + (((float)x + 1f) * (texCoordXSize / (float)renderSize));
                    float texCoordTop    = texCoordYStart + ((float)y * (texCoordYSize / (float)renderSize));
                    float texCoordBottom = texCoordYStart + (((float)y + 1f) * (texCoordYSize / (float)renderSize));
                    
                    // Set vertex indexes
                    int vertIndexStart = vertIndex;
                    ushort vertTopLeft = (ushort)(vertIndex - (renderSize + (x == 0 ? 1 : 2)));
                    ushort vertTopRight = (ushort)(vertTopLeft + 1);
                    ushort vertBottomLeft = (ushort)(vertIndex - 1);
                    if (y == 0)
                    {
                        vertTopLeft = (ushort)(vertIndex - (x == 1 ? 3 : 2));
                    }
                    if (y == 1)
                    {
                        vertTopLeft += (ushort)((x == 0 ? x + 1 : x) - renderSize);
                        vertTopRight = (ushort)(vertTopLeft + (x == 0 ? 1 : 2));
                    }

                    // Make top-left vertex
                    if (y == 0 && x == 0)
                    {
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x * tileSize, y * tileSize), heightmap, flat: flat);
                        //normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make top-right vertex
                    if (y == 0)
                    {
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2((x + 1) * tileSize, y * tileSize), heightmap, flat: flat);
                        //normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordRight, texCoordTop);
                        colors[vertIndex] = color;
                        vertTopRight = (ushort)(vertIndex);
                        vertIndex++;
                    }

                    // Make bottom-left vertex
                    if (x == 0)
                    {
                        vertices[vertIndex] = Transform2Dto3D(face, new Vector2(x * tileSize, (y + 1) * tileSize), heightmap, flat: flat);
                        //normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
                        texcoords[vertIndex] = new(texCoordLeft, texCoordBottom);
                        colors[vertIndex] = color;
                        vertBottomLeft = (ushort)(vertIndex);
                        vertIndex++;
                    }
                    
                    // Make bottom-right vertex
                    vertices[vertIndex] = Transform2Dto3D(face, new Vector2((x + 1) * tileSize, (y + 1) * tileSize), heightmap, flat: flat);
                    //normals[vertIndex] = Vector3.Normalize(vertices[vertIndex]);
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
