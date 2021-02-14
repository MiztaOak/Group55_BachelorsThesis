using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class Cell
{

    private float x,z;
    private Model model;


    //Static values of total concentrations and chemical properties
    //Taken from table 1 in ODEs in Chemotaxis.pdf
    // private static float at = 7.9f; // uM
    // private static float bt = 0.28f; // uM
    // private static float rt = 0.16f; // uM
    // private static float yt = 9.7f; // uM
    // private static float zt = 3.8f; // uM
    // private static float k1 = 34.0f; //per second
    // private static float k2 = 100.0f; //per uM per second
    // private static float k3 = 15.0f; //per uM per second
    // private static float k4 = 1.6f; //per uM per second
    // private static float k5 = 0.7f; //per second
    // private static float k6 = 0.085f; //per second
    // private static float gr = 0.0375f; //per uM per second
    // private static float gb = 3.14f; //per uM^2 per second


    // private static float kOn = 0.5f; //mM
    // private static float kOff = 0.02f; //mM

    // private static float kbar1 = 48.571f;
    // private static float kbar2 = 1385.0f;
    // private static float kbar3 = 6.0f;
    // private static float kbar4 = 8.686f;
    // private static float kbar5 = 1.0f;
    // private static float kbar6 = 0.121f; 
    // private static float alpha1 = 0.814f;
    // private static float alpha2 = 28.214f;
    // private static float gammaR = 0.00857f;
    // private static float gammaB = 0.352f;


    //Probabilistic

    //Probabilities of reactions
    private float pA = 0.5f;
    private float pY = 0.5f;
    private float pB = 0.5f;
    private float pZ = 0.4f;
    private float pR = 0.3f;

    //Total concentrations
    private int cheA = 100;
    private int cheY = 100;
    private int cheB = 100;
    private int cheZ = 100;
    private int cheR = 100;

    //Phosphorylated concentrations
    private int cheAP = 30;
    private int cheYP = 30;
    private int cheBP = 30;

    private bool run = true;
    private float c = 0;
    // public float deltaC { get; set; }
    
    // //calculated values of energy and phosphorylated concentrations
    // private float m; //methylation
    // private float energy; // F
    // private float phi; //receptor activity
    // //phosphorylated concentrations
    // private float ap = 0;
    // private float bp = 0;
    // private float yp = 0;

    public Cell()
    {
        this.model = Model.GetInstance();
    }

    public void SetConcentration(float c)
    {
        // deltaC = c - this.c;
        this.c = c;
    }

    //Values run away or become NaN very quickly without bounding.

    // private void CalculatePhi()
    // {
    //     float dm;
    //     dm = (gammaR*(1-phi)-gammaB*Mathf.Pow(bp,2)*phi)*k5;
    //     m = m + dm;
    //     float l = c * 0.0000001f; //Absolute concentration? mM
    //     float x = (1 + l/kOff) / (1 + l/kOn);
    //     energy = n * (1 - m/2 + Mathf.Log(x));
    //     phi = 1 / (1 + Mathf.Exp(energy));
    // }

    // private void CalculateA()
    // {
    //     float da;
    //     da = (phi*kbar1*(1-ap)-kbar2*ap*(1-yp)-kbar3*ap*(1-bp))*k5;
    //     ap = Mathf.Min(Mathf.Max(ap + da, 0), a); //Should be balanced without
    // }

    // private void CalculateY()
    // {
    //     float dy;
    //     dy = (alpha1*kbar2*ap*(1-yp)-(kbar4+kbar6)*yp)*k5;
    //     yp = Mathf.Min(Mathf.Max(yp + dy, 0), y);
    // }

    // private void CalculateB()
    // {
    //     float db;
    //     db = (alpha2*kbar3*ap*(1-bp)-kbar5*bp)*k5;
    //     bp = Mathf.Min(Mathf.Max(bp + db, 0), b);
    // }

    private void MCP()
    { //Determine 'activity' of A based on ligands, cheR and cheB
        //c and cheB decrease activity, cheR increases (methylation)
        int nR = Binomial.Sample(pR, cheR);
        int nB = Binomial.Sample(pB, cheBP);
        float r = (float) nR / cheR;
        float b = (float) nB / cheBP;
        pA = (1-c)*(1-b)*r; 
        Debug.Log("nR: " + r);
        Debug.Log("nB: " + b);
        Debug.Log("pA: " + pA);
    }

    private void CheA()
    { //Phosphorylation from A to B and Y
        if(pA > 0 && cheAP > 0)
        {
            int nB = cheB - cheBP;
            int nY = cheY-cheYP;
            int n = Mathf.Min(cheAP, nY + nB); //Available particles
            int nP = Binomial.Sample(pA,n); //transfers
            Debug.Log("nP: " + nP);
            int shareB = (int) (nB / (nB+nY)) * nP;
            cheBP += shareB; //portion to B
            cheYP += nP-shareB; //portion to Y
            cheAP -= nP; //Subtract from A
        }
    }

    private void Flagella()
    {
        int nP = Binomial.Sample(pY,cheYP);
        if(nP > 20)
            this.run = false;
        else
            this.run = true;
    }

    private void CheZ()
    { //Phosphorylated Y are dephosphorylated by Z
        int nP = Binomial.Sample(pZ,cheYP);
        cheY -= nP;
    }

    public bool IsRun()
    {
        this.c = model.environment.getConcentration(x, z);
        DecideState();
        bool state = this.run;
        return state;
    }

    public void setPos(float x, float z)
    {
        this.x = x;
        this.z = z;
    }

    private void DecideState()
    {
        //Probabilistic approach:
        MCP();
        CheA();
        Flagella();
        CheZ();
        Debug.Log("CheY-P: " + cheYP);
        //Try solving diff. equations sequentially and produce output.
        //Only either maxed out phosphorylated, or 0. Completely unbalanced, probably
        //completely incorrectly handled.

        // CalculatePhi();
        // CalculateA();
        // CalculateY();
        // CalculateB();
        // float tumbleBias = yp/y; //Always 1 or 0.
        // if(tumbleBias > 0.5f)
        //     this.run = false;
        // else
        //     this.run = true;
        // float rand = Random.Range(0.0f,1.0f);
        // if(rand >= this.c && deltaC >= 0)
        //     this.run = true;
        // else 
        //     this.run = false;      
    }
}
