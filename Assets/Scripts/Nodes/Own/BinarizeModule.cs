using System.Collections;
using System.Collections.Generic;
using LibNoise;
using UnityEngine;

public class BinarizeModule : SerializableModuleBase
{
    #region Constructors

    public BinarizeModule() : base(1)
    {

    }

    /// <summary>
    /// Initializes a new instance of Invert.
    /// </summary>
    /// <param name="input">The input module.</param>
    public BinarizeModule(ModuleBase input)
        : base(1)
    {
        Modules[0] = input;
    }

    #endregion

    #region ModuleBase Members

    public override double GetValue(double x, double y, double z)
    {
        double val = Modules[0].GetValue(x, y, z);

        return (Mathf.Abs((float)val - 1) > Mathf.Abs((float)val + 1)) ? 1d : -1d;

        //return (double)((int)Modules[0].GetValue(x, y, z));
    }

    #endregion
}
