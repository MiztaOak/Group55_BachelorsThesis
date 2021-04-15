using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentFactory
{
    //These methods will be used to set/create the environment based on the input data in the ui they serve as a interface between the view and the model
    //Abstracing the actual creation and lessening the impact of any possible changes to the environmental models
    public static void CreateBasicEnvionment(float d, float i_0, float x, float z, bool isDynamic)
    {
        Model.GetInstance().environment = new Environment(d, i_0, x, z, 49,isDynamic);
    }

    public static void CreateBasicEnvionment(float d, float i_0, float x, float z)
    {
        CreateBasicEnvionment(d, i_0, x, z, false);
    }

    public static void CreateBasicEnvionment(float d, float i_0)
    {
        CreateBasicEnvionment(d, i_0, 0, 0);
    }

    public static void CreateMultiEnvironment(float d, float i_0, int n, bool isDynamic)
    {
        if (n <= 1)
        {
            CreateBasicEnvionment(d, i_0,0,0,isDynamic);
            return;
        }
        float x = 10, y = 10;

        if (n > 5)
            n = 5;
        float[] xs = { 10, -10, 10, -10, 0 };
        float[]ys = { 10, -10, -10, 10, 0 };
        
        
        Model.GetInstance().environment = new MultiLigandEnvironment(d, i_0, xs, ys, 49f,n,isDynamic);
    }

    public static void CreateMultiEnvironment(float d, float i_0)
    {
        Model.GetInstance().environment = new MultiLigandEnvironment(new float[] { d, d }, new float[] { i_0, i_0 }, new float[] { 10, -10 }, new float[] { 10, -10 }, new float[] { 49f, 49f }, 2);
    }

    public static void CreateMultiEnvironment(float[] d, float[] i_0)
    {
        Model.GetInstance().environment = new MultiLigandEnvironment(d, i_0, new float[] { 10, -10 }, new float[] { 10, -10 }, new float[] { 49f, 49f }, 2);
    }
}
