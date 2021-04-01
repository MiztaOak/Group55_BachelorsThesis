using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardRegulation : ICellRegulation
{

    private float B;
    private float U;

    //Don't know if we want to have time as a factor, or just work in steps. The dimensions of dt
    //are a bit unclear
    //private float dt

    private float h(float c)
    { //Basic logistic function with x0 = 0.5, L = 1, k = 1.
        return (1 / (1 + Mathf.Exp(-(c-0.5f))));
    }

    public bool DecideState(float c)
    {
        if( U == 0 )
        { //Step 1, initialize
            U = Random.Range(0.0f,1.0f);
            B = 0;
        }
        float BNext = B + h(c)*(1-B); //Step 2
        if( BNext > U )
        { //Tumble and return to step 1
            U = 0;
            B = 0;
            return false; //Tumble
        } 
        else
        { //Keep running, and return to step 2
            B = BNext;
            return true; //Run
        } 
        
    }

    public ICellRegulation Copy()
    {
        return new HazardRegulation();
    }
}
