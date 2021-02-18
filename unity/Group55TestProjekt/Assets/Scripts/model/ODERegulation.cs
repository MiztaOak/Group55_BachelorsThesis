using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODERegulation : ICellRegulation
{
    private float c;

    //Static values of total concentrations and chemical properties
    //Taken from tables 1 and 2 in ODEs in Chemotaxis-article
    private static float a = 7.9f; // uM
    private static float b = 0.28f; // uM
    private static float r = 0.16f; // uM
    private static float y = 9.7f; // uM
    private static float z = 3.8f; // uM
    private static float kOn = 0.5f; //mM
    private static float kOff = 0.02f; //mM
    private static float k5 = 0.7f; //per second
    private static int n = 18; //Number of receptors

    private static float kbar1 = 48.571f;
    private static float kbar2 = 1385.0f;
    private static float kbar3 = 6.0f;
    private static float kbar4 = 8.686f;
    private static float kbar5 = 1.0f;
    private static float kbar6 = 0.121f; 
    private static float alpha1 = 0.814f;
    private static float alpha2 = 28.214f;
    private static float gammaR = 0.00857f;
    private static float gammaB = 0.352f;

    //calculated values of energy and phosphorylated concentrations
    private float m; //methylation
    private float energy; // F
    private float phi; //receptor activity

    //phosphorylated concentrations
    private float ap = 0;
    private float bp = 0;
    private float yp = 0;

    //Values run away or become NaN very quickly without bounding.
    private void CalculatePhi()
    {
        float dm;
        dm = (gammaR*(1-phi)-gammaB*Mathf.Pow(bp,2)*phi)*k5;
        m = m + dm;
        float l = c * 0.0000001f; //Absolute concentration? mM
        float x = (1 + l/kOff) / (1 + l/kOn);
        energy = n * (1 - m/2 + Mathf.Log(x));
        phi = 1 / (1 + Mathf.Exp(energy));
    }

    private void CalculateA()
    {
        float da;
        da = (phi*kbar1*(1-ap)-kbar2*ap*(1-yp)-kbar3*ap*(1-bp))*k5;
        ap = Mathf.Min(Mathf.Max(ap + da, 0), a); //Should be balanced without
    }

    private void CalculateY()
    {
        float dy;
        dy = (alpha1*kbar2*ap*(1-yp)-(kbar4+kbar6)*yp)*k5;
        yp = Mathf.Min(Mathf.Max(yp + dy, 0), y);
    }

    private void CalculateB()
    {
        float db;
        db = (alpha2*kbar3*ap*(1-bp)-kbar5*bp)*k5;
        bp = Mathf.Min(Mathf.Max(bp + db, 0), b);
    }

    public bool DecideState(float c)
    {
        this.c = c;
        //Try solving diff. equations sequentially and produce output.
        //Only either maxed out phosphorylated, or 0. Completely unbalanced, probably
        //completely incorrectly handled.
        CalculatePhi();
        CalculateA();
        CalculateY();
        CalculateB();
        float tumbleBias = yp/y; //Always 1 or 0 (for some reason).
        if(tumbleBias <= 0.5f)
            return true; //Running
        else
            return false; //Tumbling
    }
}
