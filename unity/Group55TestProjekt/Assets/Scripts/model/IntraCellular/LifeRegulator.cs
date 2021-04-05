using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeRegulator 
{
    private float BLife;
    private float ULife;

    private float BDeath;
    private float UDeath;

    private float h(float c, float x0, float l, float k)
    { //Basic logistic function with x0 = 0.5, L = 1, k = 1.
        return (l / (1 + Mathf.Exp(-k*(c - x0))));
    }

    public bool Split(float c)
    {
        if (ULife == 0)
        { //Step 1, initialize
            ULife = Random.Range(0.5f, 1.0f);
            BLife = 0;
        }
        float BNext = BLife + h(c,15,0.05f,0.2f) * (1 - BLife); //Step 2
        if (BNext > ULife)
        { //Tumble and return to step 1
            ULife = 0;
            BLife = 0;
            return true;
        }
        else
        { //Keep running, and return to step 2
            BLife = BNext;
            return false;
        }
    }

    public bool Die(float c)
    {
        if (UDeath == 0)
        { //Step 1, initialize
            UDeath = Random.Range(0.0f, 1.0f);
            BDeath = 0;
        }
        float BNext = BDeath + (1-h(c,15, 0.8f, 0.3f)) * (1 - BDeath); //Step 2
        if (BNext > UDeath)
        { //Tumble and return to step 1
            UDeath = 0;
            BDeath = 0;
            return true;
        }
        else
        { //Keep running, and return to step 2
            BDeath = BNext;
            return false; 
        }
    }
}
