﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private bool run = true;
    private float c = 0;
    public float deltaC { get; set; }

    public void SetConcentration(float c)
    {
        deltaC = c - this.c;
        this.c = c;
        DecideState();
    }

    public bool IsRun()
    {
        bool state = this.run;
        //if(!state){ // Will reverse from tumble when accessed
        //    this.run = true;
        //}
        
        return state;
    }

    private void DecideState()
    {
        float rand = Random.Range(0.0f,1.0f);
        if(rand >= this.c && deltaC >= 0)
            this.run = true;
        else 
            this.run = false;      
    }
}