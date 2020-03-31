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
        for (int i = 0; i < PlanetAmount; i++)
        {
            planets.Add(Instantiate(PlanetPrefab));

            // USEFULL ?
            planets[i].transform.position = Vector3.zero;

            planets[i].transform.SetParent(transform);
            planets[i].Generate();
            planets[i].transform.position = new Vector3(1f, 0f, 1f) * (PlanetSpacing * i);


            planets[i].profile = RandomizePlanet();
            planets[i].Regenerate();
        }
        //planets.ForEach(x=> x.Generate());
        //PlanetGeneration();
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
