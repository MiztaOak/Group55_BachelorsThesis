using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Simple class that handles the calculation of the ligand consentation when there is only one ligand present
**/
public class BasicEnvironment : AbstractEnvironment
{

    private float d; //governs the slope of the function must be != 0
    
    private float i_0; //governs the min value of the consentation;

    public BasicEnvironment(float d, float i_0, float x, float y) : base(x,y) 
    {
        this.d = d == 0 ? 0.00001f : d;
        this.i_0 = i_0;
    }

    public BasicEnvironment(float d, float i_0) : this(d, i_0, 0, 0) { }


    public BasicEnvironment() : this(1, 0.01f) { } //basic general case constructor
    
    override
    public float getConsentration(float x, float y) //"based" on the model from the article that Gustav sent us
    {
        float distPow2 = Mathf.Pow(x - xCord, 2) + Mathf.Pow(y - yCord, 2); //calculates the dist^2 just to make the next row more readable
        float c = i_0 + Mathf.Exp(-Mathf.Sqrt(distPow2/d)); //calculatates c
        return c <= 1 ? c : 1; //makes sure that c is not greater than 1
    }
}
