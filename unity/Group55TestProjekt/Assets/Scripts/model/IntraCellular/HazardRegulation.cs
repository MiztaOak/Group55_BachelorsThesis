using System.Collections;
using System.Collections.Generic;

public class HazardRegulation : ICellRegulation
{

    private float B;
    private float U;
    private float l;

    //Don't know if we want to have time as a factor, or just work in steps. The dimensions of dt
    //are a bit unclear
    //private float dt

    private float h(float c)
    { //Basic logistic function with x0 = 0.5, L = 1, k = 1.
        //return (0.1f / (1 + MathFloat.Exp((c-1))));
        //return (1/ (2 + MathFloat.Exp(-(c - 3))));
        //return 1 / (1 + MathFloat.Exp(-8 * (c - 0.8f)));
        return c < 0 ? 0.5f : 0.09f;
    }

    public bool DecideState(float c)
    {
        float dC = c-l;
        l = c;
        if ( U == 0 )
        { //Step 1, initialize
            U = RandomFloat.NextFloat();
            B = 0;
        }
        float BNext = B + h(dC)*(1-B); //Step 2
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

    public float GetL()
    {
        return l;
    }
}
