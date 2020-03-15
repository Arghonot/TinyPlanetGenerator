using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : Singleton<SolarSystemManager>
{
    public bool _Regenerate;

    public List<PlanetProfile> profiles;
    public List<Sphere> planets;

    private void Start()
    {
        planets.ForEach(x=> x.Generate());
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
        print(sphere.gameObject.name);
        PlanetProfile profile = RandomizePlanet();
        sphere.profile = profile;
        sphere.Regenerate();
    }

    PlanetProfile RandomizePlanet()
    {
        return profiles[(int)Random.Range(0, profiles.Count)];
    }

}
