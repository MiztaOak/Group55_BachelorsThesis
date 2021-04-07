using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BernoulliRegulation : ICellRegulation
{

    private float c = 0;

    //Probabilities of reactions
    private float pA = 0.34f; //Autophosphorylation by A
    private float pY = 0.085f; //Auto dephosphorylation by Y-P
    private float pB = 0.007f; //Auto dephosphorylation by B-P
    private float pAY = 0.9f; //Phosphotransfer from A-P to Y
    private float pAB = 0.15f; //Phosphotransfer from A-P to B
    private float pZ = 0.016f; //Dephosphorylation of Y-P by Z
    private float pMR = 0.375f; //Methylation by R
    private float pMB = 0.0314f; //Demethylation by B-P

    //100 of each, all unphosphorylated (false by default)
    private bool[] cheA = new bool[100];
    private bool[] cheB = new bool[100];
    private bool[] cheY = new bool[100];

    private void MCP()
    { //Determine 'activity' of A based on ligands, cheR and cheB
        //c and cheB decrease activity, cheR increases (methylation)
        //Activity here means that it will autophosphorylate more quickly
        int CH3 = 0;
        for( int i = 0; i < 100; i++ )
        {
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            if( rand <= pMR )
                CH3++;
        }

        for( int i = 0; i < cheB.Length; i++ )
        {
            if( cheB[i] && CH3 > 0 )
            {
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if( rand <= pMB )
                    CH3--;
            }
        }
        pA = (1 - (CH3/10)) * (1-c);
    }

    private void CheA()
    { //Autophosphorylation of A, phosphorylation from A to B and Y
        
        //Autophosphorylation first
        for( int i = 0; i < cheA.Length; i++ )
        {
            if( !cheA[i] )
            {
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if(rand <= pA)
                    cheA[i] = !cheA[i];
            }
        }

        //Phosphotransfer second
        bool[] availB = Array.FindAll(cheB, b => !b);
        bool[] availY = Array.FindAll(cheY, y => !y);
        for( int i = 0; i < cheA.Length; i++ )
        {
            int ib = 0;
            int iy = 0;
            if( cheA[i] )
            {
                float ratioB = (availB.Length - ib)/cheB.Length;
                float ratioY = (availY.Length - iy)/cheY.Length;
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                // Determine order in which proteins are tried
                if( rand > 0.5f )
                { //Attempt CheB first
                    rand = UnityEngine.Random.Range(0.0f, 1.0f);
                    if( rand < pAB*ratioB && ib < availB.Length )
                    {
                        cheA[i] = !cheA[i];
                        availB[ib] = !availB[ib];
                        ib++;
                    }
                    else if( rand < pAY*ratioY && iy < availY.Length )
                    {
                        cheA[i] = !cheA[i];
                        availY[iy] = !availB[iy];
                        iy++;
                    }
                }
                else
                { //Attempt CheY first
                    rand = UnityEngine.Random.Range(0.0f, 1.0f);
                    if( rand < pAY*ratioY && iy < availY.Length )
                    {
                        cheA[i] = !cheA[i];
                        availY[iy] = !availB[iy];
                        iy++;
                    }
                    else if( rand < pAB*ratioB && ib < availB.Length )
                    {
                        cheA[i] = !cheA[i];
                        availB[ib] = !availB[ib];
                        ib++;
                    }
                }
            }
            if( ib >= availB.Length && iy >= availY.Length )
                break;
        }
        bool[] cheBP = Array.FindAll(cheB, b => b);
        cheB = new bool[100];
        cheBP.CopyTo(cheB, 0);
        availB.CopyTo(cheB, cheBP.Length);
        bool[] cheYP = Array.FindAll(cheY, y => y);
        cheY = new bool[100];
        cheYP.CopyTo(cheY, 0);
        availY.CopyTo(cheY, cheYP.Length);
    }

    private bool Flagella()
    {
        float rand = UnityEngine.Random.Range(0.0f,1.0f);
        bool[] cheYP = Array.FindAll(cheY, y => y);
        float ratioYP = (float) cheYP.Length / cheY.Length;
        return rand >= ratioYP; //Running if larger than ratio
    }

    private void CheY()
    { //Phosphorylated Y are auto dephosphorylated
        for( int i = 0; i < cheY.Length; i++ )
        {
            if( cheY[i] )
            {
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if(rand <= pY)
                    cheY[i] = !cheY[i];
            }
        }
    }

    private void CheZ()
    { //Phosphorylated Y are dephosphorylated by Z
        for( int i = 0; i < cheY.Length; i++ )
        {
            if( cheY[i] )
            {
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if(rand <= pZ)
                    cheY[i] = !cheY[i];
            }
        }
    }

    private void CheB()
    { //Phosphorylated B are auto dephosphorylated
        for( int i = 0; i < cheB.Length; i++ )
        {
            if( cheB[i] )
            {
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if(rand <= pB)
                    cheB[i] = !cheB[i];
            }
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
        return new BernoulliRegulation();
    }
}
