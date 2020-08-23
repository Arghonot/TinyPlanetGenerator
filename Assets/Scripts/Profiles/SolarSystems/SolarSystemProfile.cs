using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SolarSystem", menuName = "SolarSystem/Profile", order = 1)]
public class SolarSystemProfile : ScriptableObject
{
    public PlanetProfile Sun;

    public Material Skybox;
}
