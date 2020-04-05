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

    public float RimWidth;
    public Transform Player;
    public List<PlanetProfile> Suns;
    public List<PlanetProfile> PlanetProfiles;
    public List<Planet> planets;

    public MeshRenderer eclipticPlane;

    float planetSizes;

    private void Start()
    {
        SetupSolarSytem();
        //LightEffect.Instance.Init();
    }

    private void Update()
    {
        if (isPlayerOutsideSolarSytem() || _Regenerate)
        {
            _Regenerate = false;

            RepositionPlayer();
            Regenerate();
        }
    }

    #region PLAYER management

    /// <summary>
    /// Used to know if we shall regenerate the solar system.
    /// </summary>
    /// <returns></returns>
    bool isPlayerOutsideSolarSytem()
    {
        if (Player.position.magnitude > planets.Last().transform.position.magnitude + RimWidth)
        {
            return true;
        }

        return false;
    }

    void RepositionPlayer()
    {
        Player.position = Vector3.left * planetSizes;
    }

    #endregion

    #region Bodies management

    /// <summary>
    /// Use at executable initialization.
    /// Create X planets, setup their hierarchy and base datas then give them a
    /// </summary>
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
        }

        eclipticPlane.material.SetFloat("SolarSystemRadius", planets.Last().transform.position.magnitude + RimWidth);
    }

    /// <summary>
    /// Re set a new profile to every planet and give them a proper position.
    /// </summary>
    public void Regenerate()
    {
        planetSizes = 0f;

        for (int i = 0; i < planets.Count; i++)
        {
            planetSizes += GeneratePlanet(i, planetSizes);
        }

        print(planets.Last().transform.position.magnitude + RimWidth);

        eclipticPlane.material.SetFloat("SolarSystemRadius", planets.Last().transform.position.magnitude + RimWidth);
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

        // We keep the sun at the center
        if (i == 0)
        {
            planets[i].transform.position = Vector3.zero;
        }
        else
        {
            planets[i].transform.position =
                new Vector3(1f, 0f, 1f) *
                (planetsize + (PlanetSpacing * i) + planets[i].transform.localScale.x);
        }

        planets[i].Anchor.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));


        // planet profile generation
        if (i == 0)
        {
            planets[i].profile = GetRandomProfile(false);
        }
        else
        {
            planets[i].profile = GetRandomProfile();
        }

        planets[i].Regenerate();

        return planets[i].transform.localScale.x;
    }

    /// <summary>
    /// Get a random profile depending on if the body is a star or a planet.
    /// </summary>
    /// <param name="planet"></param>
    /// <returns></returns>
    PlanetProfile GetRandomProfile(bool planet = true)
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

    #endregion
}
