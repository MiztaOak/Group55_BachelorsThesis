using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentFactory
{
    //Method used to inject a new environment into the model
    //To be used by the ui of the program based on the user inserted values for the different parameters (this is just a temp solution that should work in the begining)
    public static void CreateEnvironment(EnvironmentType type, float d, float i_0, float x, float y, float maxTime, float k)
    {
        switch (type)
        {
            case EnvironmentType.Basic:
                Model.GetInstance().environment = new BasicEnvironment(d, i_0, x, y);
                break;
            case EnvironmentType.Time:
                Model.GetInstance().environment = new TimeDependentEnvironment(d, i_0, x, y, maxTime, k);
                break;
        }
    }
}
