using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public ParticleSystem system;
    private ParticleSystem.Particle[] particles = null;

    public CustomPerlinGenerator generator;

    public int CloudAmount;

    public int textureSize;
    public Gradient grad;

    public Texture2D cloudmap;
    public float Size;

    public NoiseType type;

    public void Generate()
    {
        cloudmap = generator.GetCloudBase(grad, textureSize, type);

        system.Emit(CloudAmount);

        if (particles == null)
        {
            particles = new ParticleSystem.Particle[CloudAmount];
        }

        CloudAmount = system.GetParticles(particles);
        PlaceClouds(cloudmap);
        system.SetParticles(particles, CloudAmount);
    }

    void PlaceClouds(Texture2D cloudmap)
    {
        for (int i = 0; i < CloudAmount; i++)
        {
            particles[i].position = FindCloudPosition(cloudmap);
        }
    }

    Vector3 FindCloudPosition(Texture2D cloudmap)
    {
        int it = 0;
        bool isInMap = false;
        Vector2 texPos = Vector2.zero;

        while(!isInMap)
        {
            texPos = new Vector2(
                Random.Range(0, cloudmap.width),
                Random.Range(0, cloudmap.height));

            if (cloudmap.GetPixel((int)texPos.x, (int)texPos.y) == Color.white)
            {
                isInMap = true;
            }

            it++;

            if (it > 50)
            {
                isInMap = true;
            }
        }

        return CoordinatesProjector.InverseMercatorProjector(
            (cloudmap.width / texPos.x) * 360f * Mathf.Deg2Rad,
            (cloudmap.height / texPos.y) * 180f * Mathf.Deg2Rad,
            Size);
    }
}
