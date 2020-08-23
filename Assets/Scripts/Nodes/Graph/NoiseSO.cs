using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Graph", menuName = "Planets/Graph", order = 2)]
public class NoiseSO : ScriptableObject
{
    public double Frequency;
    public double Lacunarity;
    public double Persistence;
    public int Octaves;
    public int Seed;
    public int Min;
    public int Max;
    public LibNoise.QualityMode Quality;
}
