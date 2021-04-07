using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Simple class that handles the calculation of the ligand consentation when there is only one ligand present
**/
public class Environment : AbstractEnvironment
{

    private float d; //governs the slope of the function must be != 0
    
    private float i_0; //governs the min value of the consentation;

    private float max; //max value for the consentration

    public Environment(float d, float i_0, float x, float z, float max) : base(x,z)
    {
        this.d = d == 0 ? 0.00001f : d;
        this.i_0 = i_0;
        this.max = max;
    }

    public Environment(float d, float i_0, float x, float z) : this(d, i_0, x, z, 49){}

    public Environment(float d, float i_0) : this(d, i_0, 0, 0) { }


    public Environment() : this(1, 0.01f) { } //basic general case constructor
    
    override
    public float getConcentration(float x, float z) //"based" on the model from the article that Gustav sent us
    {
        float distPow2 = Mathf.Pow(x - xCord, 2) + Mathf.Pow(z - zCord, 2); //calculates the dist^2 just to make the next row more readable
        float c = i_0 + max*Mathf.Exp(-distPow2/d); //calculatates c
        return c;
        //return c <= max ? c : max; //makes sure that c is not greater than 1
        // return (7 * ( 0.001f + Mathf.Exp(-Mathf.Sqrt(Mathf.Pow(xCord+x,2)+Mathf.Pow(z,2))) ));
    }

    override
    public float GradX(float x, float z)
    {
        float dist = -Mathf.Pow(x - xCord, 2) - Mathf.Pow(z - zCord, 2);
        return -2 * Mathf.Exp(dist / d) * (x - xCord) / d;
    }

    override
    public float GradZ(float x, float z)
    {
        float dist = -Mathf.Pow(x - xCord, 2) - Mathf.Pow(z - zCord, 2);
        return -2 * Mathf.Exp(dist / d) * (z - zCord) / d;
    }

    public override float GetMaxVal()
    {
        return max;
    }
}
