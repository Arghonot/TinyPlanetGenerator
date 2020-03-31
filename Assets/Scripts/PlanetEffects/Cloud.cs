using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public ParticleSystem system;
    private ParticleSystem.Particle[] particles = null;

    public CustomPerlinGenerator generator;

    public int CloudAmount;
    public int PerCloud;
    public Vector2 CloudSize;

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
        // cloud's points of interest
        List<Vector2> POIs = new List<Vector2>();

        for (int i = 0; i < CloudAmount / PerCloud; i++)
        {
            POIs.Add(FindPOILonLat(cloudmap));
        }

        for (int i = 0; i < CloudAmount; i++)
        {
            // particle pos relative to it's POI
            Vector2 particlePositionFromPOI = new Vector2(
                Random.Range(-CloudSize.x, CloudSize.x),
                Random.Range(-CloudSize.y, CloudSize.y));

            Vector2 particlePositionLonLat = new Vector2(
                Mathf.Clamp(
                    (POIs[i / PerCloud].x + particlePositionFromPOI.x),
                    0,
                    cloudmap.width),
                Mathf.Clamp(
                    (POIs[i / PerCloud].y + particlePositionFromPOI.y),
                    0,
                    cloudmap.height));

            Vector2 particlePosition = new Vector2(
                ((particlePositionLonLat.x / cloudmap.width) * 360f) - 180f,
                ((particlePositionLonLat.y / cloudmap.height) * 180f) - 90f);

            particles[i].position = CoordinatesProjector.InverseMercatorProjector(
                particlePosition.x * Mathf.Deg2Rad,
                particlePosition.y * Mathf.Deg2Rad,
                Size);
        }
    }

    Vector2 FindPOILonLat(Texture2D cloudmap)
    {
        // amount of iteration
        int it = 0;
        bool isInMap = false;
        Vector2 texPos = Vector2.zero;

        while (!isInMap)
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

        return new Vector2(
            ((texPos.x / cloudmap.width) * 360f),
            ((texPos.y / cloudmap.height) * 180f));
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
