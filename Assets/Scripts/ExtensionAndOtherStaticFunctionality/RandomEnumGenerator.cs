using System;
using System.Collections.Generic;
using System.Linq;

public static class RandomEnumGenerator
{
    private static Random s_m_RandomObject;

    static RandomEnumGenerator()
    {
        s_m_RandomObject = new();
    }

    public static T RandomEnumValue<T>(IEnumerable<T> valuesWhichShouldBeExcluded) where T : Enum
    {
        Array AllValuesOfT = Enum.GetValues(typeof(T));
        IEnumerable<T> PossibleValues = AllValuesOfT.Cast<T>().Except(valuesWhichShouldBeExcluded);
        if (PossibleValues.Any())
        {
            return PossibleValues.ElementAt(s_m_RandomObject.Next(PossibleValues.Count()));
        }
        else
        {
            throw new InvalidOperationException("Cannot get random enum value because all values are marked as excluded!");
        }
    }

    public static T RandomEnumValue<T>() where T : Enum => RandomEnumValue(new List<T>());
}