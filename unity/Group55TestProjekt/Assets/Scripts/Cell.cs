using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private bool run = true;
    private float c;

    public void SetConcentration(float c)
    {
        this.c = c;
        DecideState();
    }

    public bool IsRun()
    {
        bool state = this.run;
        if(!state){ // Will reverse from tumble when accessed
            this.run = true;
        }
        return state;
    }

    private void DecideState()
    {
        float rand = Random.Range(0.0f,1.0f);
        if(rand <= this.c)
            this.run = true;
        else 
            this.run = false;      
    }
}
