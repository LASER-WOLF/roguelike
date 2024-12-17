using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace Core;

/// <summary>
/// Region on a planet
/// </summary>
public class Region
{
    // Fields
    public readonly int id; 
    
    // Properties
    public static int count { get; private set; } = 0;
    public Vector3 pos { get; private set; }
    public int height { get; private set; }
    public int size { get; private set; }

    // Constructor
    public Region(Vector3 pos, int height = 0, int size = 0)
    {
        this.id = count;
        count++;
        //Logger.Log("Making new region (" + id.ToString() + ") on position " + pos.X.ToString() + ", " + pos.Y.ToString() + ", " + pos.Z.ToString());
        this.pos = pos;
        this.height = height;
        this.size = size;
    }
}

/// <summary>
/// A round astronomical body.
/// </summary>
public class Planet
{
    public static readonly float sizeRatio = 0.01f;
    public Vector3 pos { get; private set; }
    public readonly int size;
    public float renderSize { get { return (float)size * sizeRatio; } }
    private Model model;
    private Vector3 rotation = new Vector3(0f, 0f, 0f);
    //private Texture2D texture;
    private Texture2D heightmapTex;
    private uint initialSeed, seed;
    private Region[] regions;
    private Region[] subregions;
    private Model skyboxModel;
    private Texture2D skyboxTex;

    // Constructor
    public Planet(int size)
    {
        //this.seed = 0B_00000010_00000001_00000011_00000001;
        this.initialSeed = this.seed = (uint)DateTime.Now.Ticks;
        this.size = size;
        this.pos = new Vector3(0.0f, 0.0f, 0.0f);
        Generate();
    }

    // Generate the planet
    private unsafe void Generate()
    {
        Logger.Log("Generating planet (" + seed.ToString() + ")");

        GenerateRegions();

        // Generate planet heightmap
        Image heightmapImage = MakeHeightmap();
        heightmapTex = Raylib.LoadTextureFromImage(heightmapImage);
        Color* heightmap = Raylib.LoadImageColors(heightmapImage);
        Raylib.UnloadImage(heightmapImage);
        
        // Generate planet mesh
        model = Raylib.LoadModelFromMesh(MakeMesh(heightmap, renderSize: 100, flat: false));
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref heightmapTex);
        // for (int face = 0; face < 6; face++)
        // {
        //     models[face] = Raylib.LoadModelFromMesh(MakeMeshFace(face, heightmap, flat: false));
        //     Raylib.SetMaterialTexture(ref models[face], 0, MaterialMapIndex.Albedo, ref heightmapTex);
        // }
        Raylib.UnloadImageColors(heightmap);
        
        // Set planet texture
        //texture = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");
        //Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        MakeSkyboxImg(out Image skyboxImg);
        skyboxTex = Raylib.LoadTextureFromImage(skyboxImg);
        Raylib.UnloadImage(skyboxImg);
        //skyboxTex = Raylib.LoadTexture("./assets/textures/uv_checker_cubemap_1024.png");

        skyboxModel = Raylib.LoadModelFromMesh(MakeSkyboxMesh());
        Raylib.SetMaterialTexture(ref skyboxModel, 0, MaterialMapIndex.Albedo, ref skyboxTex);
    }
    
    // TODO: Implement this in the mesh generation
    // Find normal for a vertex
    // vN = Vector3Normalize(Vector3CrossProduct(Vector3Subtract(vB, vA), Vector3Subtract(vC, vA)));
   
    // Called every frame
    public void Update(float deltaTime)
    {
    }

    // Render 3D graphics
    public void Render3D()
    {
        Raylib.DrawModel(model, pos, sizeRatio, Color.White);
        Raylib.DrawModel(skyboxModel, pos, 100f, Color.White);
        if (Game.debug)
        {
            Raylib.DrawLine3D(new Vector3(-(float)(renderSize * 2f), 0f, 0f), new Vector3(renderSize * 2f, 0f, 0f), Color.Red);
            Raylib.DrawLine3D(new Vector3(0f, -(float)(renderSize * 2f), 0f), new Vector3(0f, renderSize * 2f, 0f), Color.Blue);
            Raylib.DrawLine3D(new Vector3(0f, 0f, -(float)(renderSize * 2f)), new Vector3(0f, 0f, renderSize * 2f), Color.Green);
            foreach (Region region in regions)    { Raylib.DrawSphereEx((region.pos * 0.82f * (float)size * sizeRatio), 0.1f, 10, 10, Color.Maroon); }
            foreach (Region region in subregions) { Raylib.DrawSphereEx((region.pos * 0.82f * (float)size * sizeRatio), 0.05f, 10, 10, Color.Violet); }
        }
        //Raylib.DrawSphereEx(pos + new Vector3(offsetX, 0f, 0f), size * 0.775f, 64, 64, new Color(0, 82, 172, 60));
    }
    
    // Render 2D graphics
    public void Render2D()
    {
        //float multiplier = 0.175f;
        //Raylib.DrawTextureEx(texture, new Vector2(0f, 0f), 0f, multiplier, Color.White);
        //Raylib.DrawTextureEx(heightmapTex, new Vector2(0f, Raylib.GetRenderHeight() - ((1024f * 2f) * multiplier)), 0f, (1024f / ((float) size + 1f)) * multiplier, Color.White);
        //Raylib.DrawTextEx(font, "", new Vector2(2, 20), 16, 2, Color.White);
    }

    public void RenderImGui()
    {
        if (ImGui.Begin("Planet"))
        {
            ImGui.Text("Seed:       " + initialSeed.ToString()) ;
            ImGui.Text("Size:       " + size.ToString()) ;
            ImGui.Text("Regions:    " + regions.Length.ToString()) ;
            ImGui.Text("Subregions: " + subregions.Length.ToString()) ;
        }
    }

    // Free allocated memory
    public void Exit()
    {
        //Raylib.UnloadTexture(texture);
        Raylib.UnloadTexture(skyboxTex);
        Raylib.UnloadTexture(heightmapTex);
        Raylib.UnloadModel(model);
        Raylib.UnloadModel(skyboxModel);
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

    // Find the three closest regions from a given point on the planet surface
    private void FindRegions(Vector3 pos, ref Region[] source, ref Region[] result, ref float[] resultDist, ref float[] resultRatio)
    {
        bool[] found = new bool[3];
        for (int i = 0; i < source.Length; i++)
        {
            float dist = Vector3.Distance(pos, source[i].pos);
            if (!found[0] || dist < resultDist[0])
            {
                if (found[0])
                {
                    if (found[1])
                    {
                        found[2] = true;
                        result[2] = result[1];
                        resultDist[2] = resultDist[1];
                    }
                    found[1] = true;
                    result[1] = result[0];
                    resultDist[1] = resultDist[0];
                }
                found[0] = true;
                result[0] = source[i];
                resultDist[0] = dist;
            }
            if (source[i] != result[0] && (!found[1] || dist < resultDist[1]))
            {
                if (found[1])
                {
                    found[2] = true;
                    result[2] = result[1];
                    resultDist[2] = resultDist[1];
                }
                found[1] = true;
                result[1] = source[i];
                resultDist[1] = dist;
            }
            if (source[i] != result[0] && source[i] != result[1] && (!found[2] || dist < resultDist[2]))
            {
                found[2] = true;
                result[2] = source[i];
                resultDist[2] = dist;
            }
        }
        float totalDist = resultDist[0] + resultDist[1] + resultDist[2];
        resultRatio[0] = resultDist[0] / totalDist;
        resultRatio[1] = resultDist[1] / totalDist;
        resultRatio[2] = resultDist[2] / totalDist;
    }

    // Place regions on the planet surface
    private void PlaceRegions(int num, ref Region[] result)
    {
        result = new Region[num];
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
                foreach (Region? region in result) { if (region != null && Vector3.Distance(pos, region.pos) < minDistance) { discard = true; Logger.Err("Planet region discarded (too close)"); break; } }
                
                // Add the region if the position is free
                if (!discard)
                {
                    placed = true;
                    result[i] = new Region(pos, Lfsr.MakeInt(ref seed, max: (uint)255));
                }
            }
        }
    }

    // Place regions and sub-regions on the planet surface
    private void GenerateRegions()
    {
        uint minNumRegions           = 4;
        uint maxNumRegions           = 32;
        uint maxNumSubregions        = 4;
        int numRegions    = Lfsr.MakeInt(ref seed, min: minNumRegions, max: maxNumRegions);
        int numSubregions = Lfsr.MakeInt(ref seed, min: (uint)numRegions, max: (uint)(numRegions * maxNumSubregions));
        //Logger.Log("Number of regions: " + numRegions.ToString());
        //Logger.Log("Number of subregions: " + numSubregions.ToString());
        PlaceRegions(numRegions, ref regions);
        PlaceRegions(numSubregions, ref subregions);
    }
    
    // Returns the heightmap colorbuffer index for a given cube face index & grid position
    private int GetCubemapIndex(int imgSize, int face, int x, int y)
    {
        int indexOffset = imgSize * (face % 3) + (face > 2 ? (imgSize * 3) * imgSize : 0);
        return indexOffset + ((imgSize * 3) * y) + x;
    }

    private unsafe void MakeSkyboxImg(out Image result)
    {
        // float threshold = 0.8f;
        // float noiseSize1 = 100f;
        // float noiseSize2 = 1f;
        // float noiseSeed1 = (float)Lfsr.MakeInt(ref seed);
        // float noiseSeed2 = (float)Lfsr.MakeInt(ref seed);
        int imgSize = 512;
        int imgWidth = imgSize * 3;
        int imgHeight = imgSize * 2;
        byte* pixels = (byte*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(byte));
        for (int face = 0; face < 6; face++)
        {
            for (int y = 0; y < imgSize; y++)
            {
                for (int x = 0; x < imgSize; x++)
                {
                    // int index = y * imgSize + x + (face * imgSize * imgSize);
                    // Vector3 pos = Vector3.Normalize(Transform2DToCube(imgSize, face, new Vector2((float)x, (float)y)));
                    // float noise1  = Perlin.Noise4(pos * noiseSize1, noiseSeed1);
                    // float noise2  = Perlin.Noise4(pos * noiseSize2, noiseSeed2);
                    // float noise = (noise1 * 1.75f + noise2 * 0.25f) / 2f;
                    // float intensity = noise > threshold ? (noise - threshold) / (1f - threshold) : 0f ;
                    // intensity = Smoothstep.CubicPolynomial(intensity);
                    float intensity = (float)x / (float)imgSize;
                    pixels[GetCubemapIndex(imgSize, face, x, y)] = (byte)(intensity * 255f);
                }
            }
        }
        // Make image
        result = new Image
        {
            Data = pixels,
            Width = imgWidth,
            Height = imgHeight,
            Format = PixelFormat.UncompressedGrayscale,
            //Format = PixelFormat.UncompressedR8G8B8A8,
            Mipmaps = 1,
        };
    }

    // Procedural generation of an 8-bit/1 channel grayscale heightmap image for the planet
    private unsafe Image MakeHeightmap()
    {
        // Border sizes 
        float borderSizeRegion       = 0.15f;
        float borderSizeSubregion    = 0.45f;
        float edgeSizeRegion         = 0.3f;
        float edgeSizeSubregion      = 0.3f;
        // Noise set up
        float noiseAmountRegion      = 1f;
        float noiseSizeRegion        = 3f;
        float noiseAmountSubregion   = 1f;
        float noiseSizeSubregion     = 5f;
        float noiseSizeFractal       = 100f;
        // Height ratios
        float heightPercentNoise     = 0.025f;
        float heightPercentSubregion = 0.30f;
        float heightPercentRegion    = 1.0f - heightPercentNoise - heightPercentSubregion;

        // Create pixel array for the heightmap image
        int imgWidth = (size + 1) * 3;
        int imgHeight = (size + 1) * 2;
        byte* pixels = (byte*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(byte));
        //Color* pixels = (Color*)Raylib.MemAlloc(imgWidth * imgHeight * sizeof(Color));
        
        // Generate noise seeds
        float noiseSeedRegion     = (float)Lfsr.MakeInt(ref seed);
        float noiseSeedSubregion  = (float)Lfsr.MakeInt(ref seed);
        float noiseSeedFractal    = (float)Lfsr.MakeInt(ref seed);
        
        // Iterate through every position on the cube
        for (int face = 0; face < 6; face++)
        {
            for (int y = 0; y < size + 1; y++)
            {
                for (int x = 0; x < size + 1; x++)
                {
                    // Set 3D position
                    Vector3 pos = Vector3.Normalize(TransformCubeToSphere(Transform2DToCube(size, face, new Vector2((float)x, (float)y))));
                    
                    // Generate noise
                    float noiseRegion     = Perlin.Octave4(pos * noiseSizeRegion, noiseSeedRegion, octaves: 4);
                    float noiseSubregion  = Perlin.Octave4(pos * noiseSizeSubregion, noiseSeedSubregion, octaves: 6);
                    float noiseFractal    = Perlin.Octave4(pos * noiseSizeFractal, noiseSeedFractal, octaves: 3);
                   
                    // Set region positions
                    Vector3 posRegion    = pos + new Vector3(noiseAmountRegion * noiseRegion);
                    Vector3 posSubregion = pos + new Vector3(noiseAmountSubregion * noiseSubregion);

                    // Find the three closest regions from the current position on the planet surface
                    Region[] nearbyRegions     = new Region[3];
                    float[] nearbyRegionsDist  = new float[3];
                    float[] nearbyRegionsRatio = new float[3];
                    FindRegions(posRegion, ref regions, ref nearbyRegions, ref nearbyRegionsDist, ref nearbyRegionsRatio);
                    
                    // Find the three closest subregions from the current position on the planet surface
                    Region[] nearbySubregions     = new Region[3];
                    float[] nearbySubregionsDist  = new float[3];
                    float[] nearbySubregionsRatio = new float[3];
                    FindRegions(posSubregion, ref subregions, ref nearbySubregions, ref nearbySubregionsDist, ref nearbySubregionsRatio);
                    
                    // Set region height
                    float borderRatioRegion = ((nearbyRegionsDist[0] / nearbyRegionsDist[1]) > 1.0f - borderSizeRegion) ? Smoothstep.QuadraticRational(((nearbyRegionsDist[0] / nearbyRegionsDist[1]) - (1.0f - borderSizeRegion)) / borderSizeRegion) : 0.0f;
                    float heightRegion = ((float)nearbyRegions[0].height * (1f - borderRatioRegion)) + ((((float)nearbyRegions[0].height + (float)nearbyRegions[1].height) / 2f) * borderRatioRegion);
                    if (nearbyRegionsRatio[0] > edgeSizeRegion && nearbyRegionsRatio[1] > edgeSizeRegion && nearbyRegionsRatio[2] > edgeSizeRegion) 
                    { 
                        float edgeRatioRegion = Smoothstep.QuadraticRational((nearbyRegionsRatio[0] - edgeSizeRegion) / (0.3333334f - edgeSizeRegion));
                        heightRegion = (heightRegion * (1f - edgeRatioRegion)) + ((((float)nearbyRegions[0].height + (float)nearbyRegions[1].height + (float)nearbyRegions[2].height) / 3f) * edgeRatioRegion); 
                    }

                    // Set subregion height
                    float borderRatioSubregion = ((nearbySubregionsDist[0] / nearbySubregionsDist[1]) > 1.0f - borderSizeSubregion) ? Smoothstep.QuadraticRational(((nearbySubregionsDist[0] / nearbySubregionsDist[1]) - (1.0f - borderSizeSubregion)) / borderSizeSubregion) : 0.0f;
                    float heightSubregion = ((float)nearbySubregions[0].height * (1f - borderRatioSubregion)) + ((((float)nearbySubregions[0].height + (float)nearbySubregions[1].height) / 2f) * borderRatioSubregion);
                    if (nearbySubregionsRatio[0] > edgeSizeSubregion && nearbySubregionsRatio[1] > edgeSizeSubregion && nearbySubregionsRatio[2] > edgeSizeSubregion) 
                    { 
                        float edgeRatioSubregion = Smoothstep.QuadraticRational((nearbySubregionsRatio[0] - edgeSizeSubregion) / (0.3333334f - edgeSizeSubregion));
                        heightSubregion = (heightSubregion * (1f - edgeRatioSubregion)) + ((((float)nearbySubregions[0].height + (float)nearbySubregions[1].height + (float)nearbySubregions[2].height) / 3f) * edgeRatioSubregion); 
                    }

                    // Set height
                    //int height = (int)((heightSubregion * (heightPercentSubregion * (1f - borderRatioRegion) * (0.1f + Smoothstep.QuadraticRational((heightRegion / 255f)) * 0.9f))) + (heightRegion * heightPercentRegion) + ((noiseFractal * 255f) * heightPercentNoise));
                    int height = (int)((heightSubregion * (heightPercentSubregion * (0.1f + Smoothstep.QuadraticRational((heightRegion / 255f)) * 0.9f))) + (heightRegion * heightPercentRegion) + ((noiseFractal * 255f) * heightPercentNoise));

                    // Set height in the color array
                    pixels[GetCubemapIndex(size + 1, face, x, y)] = (byte)height;
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
        int heightmapIndex = GetCubemapIndex(size +  1, face, (int)pos.X, (int)pos.Y);
        float height = Raymath.Remap((float)heightmap[heightmapIndex].R, 0f, 255f, 0f, maxHeight);
        // Flat mode
        if (flat)
        {
            return Transform2DToFlat(face, pos) + (new Vector3(0f, 1f, 0f) * height);
        }
        // Sphere mode
        Vector3 result = Transform2DToCube(size, face, pos);
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
    private Vector3 Transform2DToCube(int cubeSize, int face, Vector2 pos)
    {
        Vector3 result = Vector3.Zero;
        Vector3 offset = new Vector3(-(float)cubeSize / 2, (float)cubeSize / 2, -(float)cubeSize / 2);
        switch (face)
        {
            case 0: result = offset + new Vector3(0f, pos.X - (float)cubeSize, pos.Y); break;
            case 1: result = offset + new Vector3(pos.X, 0f, pos.Y); break;
            case 2: result = offset + new Vector3((float)cubeSize, ((float)cubeSize - pos.X) - (float)cubeSize, pos.Y); break;
            case 3: result = offset + new Vector3((float)cubeSize - pos.X, -(float)cubeSize, pos.Y); break;
            case 4: result = offset + new Vector3(pos.X, (pos.Y - (float)cubeSize), 0f); break;
            case 5: result = offset + new Vector3(pos.X, ((float)cubeSize - pos.Y) - (float)cubeSize, (float)cubeSize); break;
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

    private unsafe Mesh MakeSkyboxMesh()
    {
        int numVerts = 24;
        int numTris = 36;

        Mesh mesh = new(numVerts, numTris);
        mesh.AllocVertices();
        mesh.AllocIndices();
        mesh.AllocTexCoords();
        Span<Vector3> vertices = mesh.VerticesAs<Vector3>();
        Span<ushort> indices = mesh.IndicesAs<ushort>();
        Span<Vector2> texcoords = mesh.TexCoordsAs<Vector2>();
        
        float texCoordX = 1f / 3f;
        float texCoordY = 1f / 2f;

        // 0: LEFT
        vertices[0]   = new Vector3(-1f,  1f,  1f);
        vertices[1]   = new Vector3(-1f,  1f, -1f);
        vertices[2]   = new Vector3(-1f, -1f,  1f);
        vertices[3]   = new Vector3(-1f, -1f, -1f);
        texcoords[0]  = new Vector2(texCoordX * 0f, texCoordY * 0f);
        texcoords[1]  = new Vector2(texCoordX * 1f, texCoordY * 0f);
        texcoords[2]  = new Vector2(texCoordX * 0f, texCoordY * 1f);
        texcoords[3]  = new Vector2(texCoordX * 1f, texCoordY * 1f);
        indices[0]    = (ushort)0;
        indices[1]    = (ushort)2;
        indices[2]    = (ushort)3;
        indices[3]    = (ushort)0;
        indices[4]    = (ushort)3;
        indices[5]    = (ushort)1;

        // 1: REAR
        vertices[4]   = new Vector3(-1f,  1f, -1f);
        vertices[5]   = new Vector3( 1f,  1f, -1f);
        vertices[6]   = new Vector3(-1f, -1f, -1f);
        vertices[7]   = new Vector3( 1f, -1f, -1f);
        texcoords[4]  = new Vector2(0f, 0f);
        texcoords[5]  = new Vector2(0f, 0f);
        texcoords[6]  = new Vector2(0f, 0f);
        texcoords[7]  = new Vector2(0f, 0f);
        texcoords[4]  = new Vector2(texCoordX * 1f, texCoordY * 0f);
        texcoords[5]  = new Vector2(texCoordX * 2f, texCoordY * 0f);
        texcoords[6]  = new Vector2(texCoordX * 1f, texCoordY * 1f);
        texcoords[7]  = new Vector2(texCoordX * 2f, texCoordY * 1f);
        indices[6]    = (ushort)4;
        indices[7]    = (ushort)6;
        indices[8]    = (ushort)7;
        indices[9]    = (ushort)4;
        indices[10]   = (ushort)7;
        indices[11]   = (ushort)5;
        
        // 2: RIGHT
        vertices[8]   = new Vector3( 1f,  1f, -1f);
        vertices[9]   = new Vector3( 1f,  1f,  1f);
        vertices[10]  = new Vector3( 1f, -1f, -1f);
        vertices[11]  = new Vector3( 1f, -1f,  1f);
        texcoords[8]  = new Vector2(texCoordX * 2f, texCoordY * 0f);
        texcoords[9]  = new Vector2(texCoordX * 3f, texCoordY * 0f);
        texcoords[10] = new Vector2(texCoordX * 2f, texCoordY * 1f);
        texcoords[11] = new Vector2(texCoordX * 3f, texCoordY * 1f);
        indices[12]   = (ushort)8;
        indices[13]   = (ushort)10;
        indices[14]   = (ushort)11;
        indices[15]   = (ushort)8;
        indices[16]   = (ushort)11;
        indices[17]   = (ushort)9;
        
        // 3: FRONT
        vertices[12]  = new Vector3( 1f,  1f,  1f);
        vertices[13]  = new Vector3(-1f,  1f,  1f);
        vertices[14]  = new Vector3( 1f, -1f,  1f);
        vertices[15]  = new Vector3(-1f, -1f,  1f);
        texcoords[12] = new Vector2(texCoordX * 0f, texCoordY * 1f);
        texcoords[13] = new Vector2(texCoordX * 1f, texCoordY * 1f);
        texcoords[14] = new Vector2(texCoordX * 0f, texCoordY * 2f);
        texcoords[15] = new Vector2(texCoordX * 1f, texCoordY * 2f);
        indices[18]   = (ushort)12;
        indices[19]   = (ushort)14;
        indices[20]   = (ushort)15;
        indices[21]   = (ushort)12;
        indices[22]   = (ushort)15;
        indices[23]   = (ushort)13;
        
        // 4: TOP
        vertices[16]  = new Vector3(-1f,  1f,  1f);
        vertices[17]  = new Vector3( 1f,  1f,  1f);
        vertices[18]  = new Vector3(-1f,  1f, -1f);
        vertices[19]  = new Vector3( 1f,  1f, -1f);
        texcoords[16] = new Vector2(texCoordX * 1f, texCoordY * 1f);
        texcoords[17] = new Vector2(texCoordX * 2f, texCoordY * 1f);
        texcoords[18] = new Vector2(texCoordX * 1f, texCoordY * 2f);
        texcoords[19] = new Vector2(texCoordX * 2f, texCoordY * 2f);
        indices[24]   = (ushort)16;
        indices[25]   = (ushort)18;
        indices[26]   = (ushort)19;
        indices[27]   = (ushort)16;
        indices[28]   = (ushort)19;
        indices[29]   = (ushort)17;
        
        // 5: BOTTOM
        vertices[20]  = new Vector3(-1f, -1f, -1f);
        vertices[21]  = new Vector3( 1f, -1f, -1f);
        vertices[22]  = new Vector3(-1f, -1f,  1f);
        vertices[23]  = new Vector3( 1f, -1f,  1f);
        texcoords[20] = new Vector2(texCoordX * 2f, texCoordY * 1f);
        texcoords[21] = new Vector2(texCoordX * 3f, texCoordY * 1f);
        texcoords[22] = new Vector2(texCoordX * 2f, texCoordY * 2f);
        texcoords[23] = new Vector2(texCoordX * 3f, texCoordY * 2f);
        indices[30]   = (ushort)20;
        indices[31]   = (ushort)22;
        indices[32]   = (ushort)23;
        indices[33]   = (ushort)20;
        indices[34]   = (ushort)23;
        indices[35]   = (ushort)21;

        Raylib.UploadMesh(ref mesh, false);
        return mesh;
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
