using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : Singleton<SolarSystemManager>
{
    public bool _Regenerate;
    public int PlanetAmount;
    public int PlanetSpacing;
    public Vector2 PlanetsSize;

    public Planet PlanetPrefab;

    public List<PlanetProfile> Suns;
    public List<PlanetProfile> PlanetProfiles;
    public List<Planet> planets;

    float planetSizes;

    private void Start()
    {
        SetupSolarSytem();
    }

    private void Update()
    {
        if (_Regenerate)
        {
            _Regenerate = false;
            Regenerate();
        }
    }

    void SetupSolarSytem()
    {
        // the width of all the previous planets
        planetSizes = 0;

        for (int i = 0; i < PlanetAmount; i++)
        {
            planets.Add(Instantiate(PlanetPrefab));
            // we setup the planet's rotation
            var anchor = new GameObject();
            var rotator = anchor.AddComponent<Rotator>();
            rotator.rotation = Vector3.up * Random.Range(0.5f, 1f);
            anchor.transform.SetParent(transform);
            anchor.transform.position = Vector3.zero;
            anchor.name = "Planet_" + i;
            // we setup the planet's orbital datas
            planets[i].transform.SetParent(anchor.transform);
            planets[i].Anchor = rotator;
            // real planet generation
            planets[i].Initialize();
            planetSizes += GeneratePlanet(i, planetSizes);

            // then set the sun at the origin of solar system.
            if (i == 0)
            {
                planets[0].transform.position = Vector3.zero;
            }

            rotator.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
        }
    }

    /// <summary>
    /// Generate the planet[index], set it a proper profile and return it's size.
    /// </summary>
    /// <param name="i">The index of the planet.</param>
    /// <param name="planetsize">The width occupied by the previous planets.</param>
    /// <returns>The size of the planet (x, y, z)</returns>
    float GeneratePlanet(int i, float planetsize)
    {
        // the new position = 0 + the space occupied by previous planets + own width * spacing
        planets[i].transform.localScale = Vector3.one * Random.Range(PlanetsSize.x, PlanetsSize.y);
        planets[i].transform.position =
            new Vector3(1f, 0f, 1f) *
            (planetsize + (PlanetSpacing * i) + planets[i].transform.localScale.x);

        // planet profile generation
        if (i == 0)
        {
            planets[i].profile = RandomizePlanet(false);
        }
        else
        {
            planets[i].profile = RandomizePlanet();
        }
        planets[i].Regenerate();

        return planets[i].transform.localScale.x;
    }

    public void Regenerate()
    {
        // we clear the UI
        planets.ForEach(x => PlanetGeneration(x));
    }

    void    PlanetGeneration(Planet sphere)
    {
        PlanetProfile profile = RandomizePlanet();
        sphere.profile = profile;
        sphere.Regenerate();
    }

    PlanetProfile RandomizePlanet(bool planet = true)
    {
        if (planet)
        {
            return PlanetProfiles[(int)Random.Range(0, PlanetProfiles.Count)];
        }
        else
        {
            return Suns[(int)Random.Range(0, Suns.Count)];
        }
    }

}
