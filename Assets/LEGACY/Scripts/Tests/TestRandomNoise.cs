using UnityEngine;
using System.Collections;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;


public class TestRandomNoise : MonoBehaviour
{
    public UnityEngine.Gradient grad;

    public int mapSizeX = 256; // for heightmaps, this would be 2^n +1
    public int mapSizeY = 256; // for heightmaps, this would be 2^n +1

    public float sampleSizeX = 4.0f; // perlin sample size
    public float sampleSizeY = 4.0f; // perlin sample size

    public float sampleOffsetX = 2.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
    public float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f


    public Renderer cubeRenderer; // renderer texture set for testing

    private Texture2D texture; // texture created for testing


    //  Persistant Functions
    //    ----------------------------------------------------------------------------


    void Start()
    {
        Generate();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Generate();
    }


    //  Other Functions
    //    ----------------------------------------------------------------------------


    void Generate()
    {
        Perlin myPerlin = new Perlin();

        ModuleBase myModule = myPerlin;



        // ------------------------------------------------------------------------------------------

        // - Generate -

        // this part generates the heightmap to a texture, 
        // and sets the renderer material texture of a cube to the generated texture


        Noise2D heightMap;

        heightMap = new Noise2D(mapSizeX, mapSizeY, myModule);

        heightMap.GeneratePlanar(
            sampleOffsetX,
            sampleOffsetX + sampleSizeX,
            sampleOffsetY,
            sampleOffsetY + sampleSizeY
            );

        texture = heightMap.GetTexture(grad);
        texture.Apply();

        cubeRenderer.material.SetTexture("_BaseMap", texture);
    }
}
