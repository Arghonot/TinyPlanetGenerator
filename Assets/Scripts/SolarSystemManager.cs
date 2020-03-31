using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : Singleton<SolarSystemManager>
{
    public bool _Regenerate;
    public int PlanetAmount;
    public int PlanetSpacing;

    public Sphere PlanetPrefab;

    public List<PlanetProfile> profiles;
    public List<Sphere> planets;

    private void Start()
    {
        SetupSolarSytem();

        Regenerate();
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
        for (int i = 0; i < PlanetAmount; i++)
        {
            planets.Add(Instantiate(PlanetPrefab));

            var anchor = new GameObject();
            var rotator = anchor.AddComponent<Rotator>();
            rotator.rotation = Vector3.up * Random.Range(0.5f, 10f);
            anchor.transform.SetParent(transform);
            anchor.transform.position = Vector3.zero;
            anchor.name = "Planet_" + i;

            // USEFULL ?
            planets[i].transform.position = Vector3.zero;

            planets[i].transform.SetParent(anchor.transform);
            planets[i].transform.position = new Vector3(1f, 0f, 1f) * (PlanetSpacing * i);
            planets[i].Generate();


            planets[i].profile = RandomizePlanet();
            planets[i].Regenerate();
        }
    }

    public void Regenerate()
    {
        // we clear the UI
        UIMapManager.Instance.FlushMaps();
        planets.ForEach(x => PlanetGeneration(x));
    }

    void    PlanetGeneration(Sphere sphere)
    {
        PlanetProfile profile = RandomizePlanet();
        sphere.profile = profile;
        sphere.Regenerate();
    }

    PlanetProfile RandomizePlanet()
    {
        return profiles[(int)Random.Range(0, profiles.Count)];
    }

}
