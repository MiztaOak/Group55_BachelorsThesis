using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentFactory
{
    //These methods will be used to set/create the environment based on the input data in the ui they serve as a interface between the view and the model
    //Abstracing the actual creation and lessening the impact of any possible changes to the environmental models
    public static void CreateBasicEnvionment(float d, float i_0, float x, float z)
    {
        Model.GetInstance().environment = new Environment(d, i_0, x, z);
    }

    public static void CreateBasicEnvionment(float d, float i_0)
    {
        CreateBasicEnvionment(d, i_0, 0, 0);
    }

    public static void CreateTimeDependentEnvionment(float d, float i_0, float maxTime, float k, float x, float z)
    {
        Model.GetInstance().environment = new TimeDependentEnvironment(d, i_0,maxTime, k, x, z);
    }

    public static void CreateTimeDependentEnvionment(float d, float i_0, float maxTime, float k)
    {
        CreateTimeDependentEnvionment(d, i_0, maxTime, k, 0, 0);
    }
}
