using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorComparison : MonoBehaviour
{
    public Color color1;
    public Color color2;

    public Gradient Gradient;

    public bool _CompareColors;
    public bool _FindInGrad;

    void Update()
    {
        if (_CompareColors)
        {
            CompareColors();
            _CompareColors = false;
        }
        if (_FindInGrad)
        {
            _FindInGrad = false;
            FindInGrad();
        }
    }

    void FindInGrad()
    {
        float redval = (float)(color1.r * 0.33);
        float greenval = (float)(color1.g * 0.66);
        float blueval = (float)(color1.b);

        Debug.Log(redval + greenval + blueval);
    }

    void CompareColors()
    {
        float D = Mathf.Sqrt(
            (Mathf.Pow(color2.r - color1.r, 2)) +
            (Mathf.Pow(color2.g - color1.g, 2)) +
            (Mathf.Pow(color2.b - color1.b, 2)));
        float d =
            (Mathf.Pow((color2.r - color1.r * 0.2f), 2)) +
            (Mathf.Pow((color2.g - color1.g * 0.2f), 2)) +
            (Mathf.Pow((color2.b - color1.b * 0.2f), 2));

        Debug.Log("D : " + D);
        Debug.Log("d : " + d);
        Debug.Log("grayscale2 : " + color2.grayscale);
    }
}
