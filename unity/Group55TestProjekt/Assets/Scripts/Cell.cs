﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool run = true;
    private float c;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

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
