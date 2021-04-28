using System.Collections;
using System.Collections.Generic;
using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class DistRegulation : ICellRegulation
{

    private float c = 0;

    //Probabilities of reactions
    private float pA = 0.34f; //Autophosphorylation by A
    private float pY = 0.00085f; //Auto dephosphorylation by Y-P
    private float pB = 0.007f; //Auto dephosphorylation by B-P
    private float pAY = 0.9f; //Phosphotransfer from A-P to Y
    private float pAB = 0.15f; //Phosphotransfer from A-P to B
    private float pZ = 0.016f; //Dephosphorylation of Y-P by Z
    private float pMR = 0.000375f; //Methylation by R
    private float pMB = 0.0314f; //Demethylation by B-P
    private float pFlagella = 0.8f; //Binding of Y-P to flagella
    private float phi; //Activity of receptor

    //Total concentrations
    private int cheA = 100;
    private int cheY = 100;
    private int cheB = 100;
    private int cheZ = 100;
    private int cheR = 100;

    //Phosphorylated amount
    private int cheAP = 30;
    private int cheYP = 30;
    private int cheBP = 30;

    private void MCP()
    { //Determine 'activity' of A based on ligands, cheR and cheB
        //c and cheB decrease activity, cheR increases (methylation)
        int nR = Binomial.Sample(pMR, cheR);
        int nB = Binomial.Sample(pMB, cheBP);
        float r = (float) nR / cheR;
        float b = (float) nB / cheBP;
        phi = (1-c)*(1-b)*r; 
        int nA = Binomial.Sample(pA, (cheA-cheAP));
        cheAP += nA;
    }

    private void CheA()
    { //Phosphorylation from A to B and Y

        int nB = cheB-cheBP;
        int nY = cheY-cheYP;
        int n = Math.Min(cheAP, nB); //Available particles
        int nP = Binomial.Sample(pAB,n); //transfers to B
        cheBP += nP;
        cheAP -= nP; //Subtract from A
        n = Math.Min(cheAP, nY); //Available particles
        nP = Binomial.Sample(pAY,n); //transfers to B
        cheYP += nP; //portion to Y
        cheAP -= nP; //Subtract from A
    }

    private bool Flagella()
    {
        int nP = Binomial.Sample(pFlagella,cheYP);
        if(nP > 10)
            return false;
        else
            return true;
    }

    private void CheY()
    { //Phosphorylated Y are auto dephosphorylated
        if(cheYP > 0)
        {
            int nP = Binomial.Sample(pY,cheYP);
            cheYP -= nP;
        }
    }

    private void CheZ()
    { //Phosphorylated Y are dephosphorylated by Z
        if(cheYP > 0)
        {
            int nP = Binomial.Sample(pZ,cheYP);
            cheYP -= nP;
        }
    }

    private void CheB()
    { //Phosphorylated B are auto dephosphorylated
        if(cheBP > 0)
        {
            int nP = Binomial.Sample(pB,cheBP);
            cheYP -= nP;
        }
    }

    public bool DecideState(float c)
    {
        this.c = c;
        MCP();
        CheA();
        bool running = Flagella();
        CheY();
        CheB();
        CheZ();
        return running;
    }

    public ICellRegulation Copy()
    {
        return new DistRegulation();
    }
}
