using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDependentEnvironment: AbstractEnvironment
{
    private float d; //governs the slope of the function must be != 0

    private float i_0; //governs the min value of the consentation;

    protected float cMax; //sets the max consentration 

    private float time;
    private float k; //just a constant that will be used to lower the max c to simulate it disapating or something
    private float maxTime;

    public TimeDependentEnvironment(float d, float i_0, float x, float z, float maxTime, float k) : base(x, z)
    {
        this.d = d == 0 ? 0.00001f : d;
        this.i_0 = i_0;
        this.maxTime = maxTime;
        this.k = k;

        cMax = 1;
        time = 0;
    }

    override
    public float getConcentration(float x, float z) //"based" on the model from the article that Gustav sent us
    {
        float distPow2 = Mathf.Pow(x - xCord, 2) + Mathf.Pow(z - zCord, 2); //calculates the dist^2 just to make the next row more readable
        float c = i_0 + cMax*Mathf.Exp(-distPow2 / d); //calculatates c
        return c <= cMax ? c : cMax; //makes sure that c is not greater than cMax
    }

    public void updateTime(float deltaTime) //method that simulates time moving formward making the slope of the graph lower and less steep not to sure if it is correct or stupid
    {
        time += deltaTime;
        cMax = 1 - k * (time < maxTime ? time : maxTime); //lowers the max value
        d = 1 + k * (time < maxTime ? time : maxTime); //max the slope less step
    }

    public void setTime(float time)
    {
        this.time = time;
    }

    public override float GradX(float x, float z)
    {
        throw new System.NotImplementedException();
    }

    public override float GradZ(float x, float z)
    {
        throw new System.NotImplementedException();
    }

    public override float GetMaxVal()
    {
        throw new System.NotImplementedException();
    }
}

