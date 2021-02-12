using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private bool run = true;
    private float c = 0;
    public float deltaC { get; set; }
    
    //calculated values of energy and phosphorylated concentrations
    private float m; //methylation
    private float energy; // F
    private float phi; //receptor activity
    //phosphorylated concentrations
    private float ap;
    private float bp;
    private float yp;

    //Static values of total concentrations and chemical properties
    //Taken from table 1 in ODEs in Chemotaxis.pdf
    private static int n = 1; //Number of receptors (1?)
    private static float a = 7.9f; // uM
    private static float b = 0.28f; // uM
    private static float r = 0.16f; // uM
    private static float y = 9.7f; // uM
    private static float z = 3.8f; // uM
    private static float k1 = 34.0f; //per second
    private static float k2 = 100.0f; //per uM per second
    private static float k3 = 15.0f; //per uM per second
    private static float k4 = 1.6f; //per uM per second
    private static float k5 = 0.7f; //per second
    private static float k6 = 0.085f; //per second
    private static float gr = 0.0375f; //per uM per second
    private static float gb = 3.14f; //per uM^2 per second
    private static float kaOn = 0.5f; //mM
    private static float kaOff = 0.02f; //mM

    public void SetConcentration(float c)
    {
        deltaC = c - this.c;
        this.c = c;
    }

    private void CalculatePhi(){
        float dm;
        dm = gr*r*(1-phi)-gb*Mathf.pow(bp,2)*phi;
        m = m + dm;
        float x = (1 + c/kOff) / (1 + c/kOn);
        energy = n * (1 - m/2 + Mathf.Log(x));
        phi = 1 / (1 + Mathf.Exp(energy));
    }

    public bool IsRun()
    {
        DecideState();
        bool state = this.run;
        //if(!state){ // Will reverse from tumble when accessed
        //    this.run = true;
        //}
        
        return state;
    }

    private void DecideState()
    {
        CalculatePhi();
        float rand = Random.Range(0.0f,1.0f);
        if(rand >= this.c && deltaC >= 0)
            this.run = true;
        else 
            this.run = false;      
    }
}
