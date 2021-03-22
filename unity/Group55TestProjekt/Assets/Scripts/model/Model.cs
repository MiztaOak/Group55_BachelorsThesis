using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that will manage the overarching data and operations that are needed by all of the program, sort of like the hub of the program
public class Model
{
    private static Model instance;
    public AbstractEnvironment environment { get; set; } //allows for the environment to be changed by the program if needed
    private float timeScaleFactor; //variable that scales the "time" of the simulation might be better to place this in a repo class later

    private Model()
    {
        //add code as it is needed
        environment = new Environment(); //super base case just to prevent any scary null pointers
        timeScaleFactor = 1;
    }

    public static Model GetInstance()
    {
        if (instance == null)
            instance = new Model();
        return instance;
    }

    //Code bellow is a very dumb implementation but will be usefull before the ui is implemented

    //creates a basic enironment
    public void SetEnvironment(float d, float i_0, float x, float y)
    {
        environment = new Environment(d, i_0, x, y);
    }

    //creates a time dependent environment
    public void SetEnvironment(float d, float i_0, float x, float y, float maxTime, float k)
    {
        environment = new TimeDependentEnvironment(d, i_0, x, y, maxTime, k);
    }

    public void SetTimeScaleFactor(float timeScaleFactor)
    {
        this.timeScaleFactor = timeScaleFactor;
    }

    public float GetTimeScaleFactor()
    {
        return timeScaleFactor;
    }
}
