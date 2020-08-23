using System;

[BlackboardType("QualityMode")]
public class Quality : Variable
{
    public override string GetDefaultName()
    {
        return "QualityMode";
    }

    public override Type GetValueType()
    {
        return typeof(LibNoise.QualityMode);
    }
}
