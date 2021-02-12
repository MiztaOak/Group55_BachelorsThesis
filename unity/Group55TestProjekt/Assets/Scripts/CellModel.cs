using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

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
    private static float kOn = 0.5f; //mM
    private static float kOff = 0.02f; //mM

    private bool run = true;
    private float c = 0;
    // public float deltaC { get; set; }
    
    //calculated values of energy and phosphorylated concentrations
    private float m; //methylation
    private float energy; // F
    private float phi; //receptor activity
    //phosphorylated concentrations
    private float ap = a/3;
    private float bp = b/3;
    private float yp = y/3;

    public void SetConcentration(float c)
    {
        // deltaC = c - this.c;
        this.c = c;
    }

    private void CalculatePhi(){
        float dm;
        dm = gr*r*(1-phi)-gb*Mathf.Pow(bp,2)*phi;
        m = m + dm;
        float x = (1 + c/kOff) / (1 + c/kOn);
        energy = n * (1 - m/2 + Mathf.Log(x));
        Debug.Log("F: " + energy);
        phi = 1 / (1 + Mathf.Exp(energy));
    }

    private void CalculateA(){
        float dA;
        dA = phi*k1*(a-ap)-k2*ap*(y-yp)-k3*ap*(b-bp);
        ap = ap + dA;
    }

    private void CalculateY(){
        float dY;
        dY = k2*ap*(y-yp)-k4*yp*z-k6*yp;
        yp = yp + dY;
    }

    private void CalculateB(){
        float dB;
        dB = k3*ap*(b-bp)-k5*bp;
        bp = bp + dB;
    }

    public bool IsRun()
    {
        Debug.Log("IsRun");
        DecideState();
        bool state = this.run;
        if(!state){ // Will reverse from tumble when accessed
           this.run = true;
        }
        return state;
    }

    private void DecideState()
    {
        Debug.Log("DecideState");
        Debug.Log("Ap: " + ap);
        Debug.Log("Bp: " + bp);
        Debug.Log("Yp: " + yp);
        Debug.Log("Phi: " + phi);
        CalculatePhi();
        CalculateA();
        CalculateY();
        CalculateB();
        Debug.Log("Ap: " + ap);
        Debug.Log("Bp: " + bp);
        Debug.Log("Yp: " + yp);
        Debug.Log("Phi: " + phi);
        float tumbleBias = yp/y;
        float rand = Random.Range(0.0f,1.0f);
        if(tumbleBias > rand)
            this.run = false;
        else
            this.run = true;
        // float rand = Random.Range(0.0f,1.0f);
        // if(rand >= this.c && deltaC >= 0)
        //     this.run = true;
        // else 
        //     this.run = false;      
    }
}
