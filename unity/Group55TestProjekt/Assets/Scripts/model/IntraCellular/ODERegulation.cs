using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Microsoft.Research.Oslo;

public class ODERegulation : ICellRegulation
{

    //Reaction constants (from Edgington article)       
    private static double kbar1 = 48.571;
    private static double kbar2 = 1382.714;
    private static double kbar3 = 6;
    private static double kbar4 = 8.686;
    private static double kbar5 = 1;
    private static double kbar6 = 0.121;
    private static double alpha1 = 0.814;
    private static double alpha2 = 28.214;
    private static double gammaR = 0.00857;
    private static double gammaB = 0.352;

    private static int N=30; //Number of (Tar) receptors

    //Receptor constants
    private static double Kaon=500; //Dissociation constant of active (Tar) receptor
    private static double Kaoff=20; //Dissociation constant of inactive (Tar) receptor
    
    private static double initRatios = 0;

    //Concentration of phosphorylated proteins
    private double Ap = initRatios;
    private double Yp = initRatios;
    private double Bp = initRatios;
    private double m = 5;
    private double S; //Increasing tumbling trigger
    private double U; //Threshold for tumbling
    private double L; //Ligand concentration

    //How far we solve the ODEs for each time step
    private static double tSpan = 1;

    private static System.Random rand = new System.Random();

    public double GetYP() { return this.Yp; }
    public double GetAP() { return this.Ap; }
    public double GetBP() { return this.Bp; }
    public double GetM() { return this.m; }
    public double GetL() { return this.L; }

    private static double h(double y)
    {
        return 0.02 + 0.5*y;
    }

    private void SolveStiff(float c)
    {
        L = (double) 7*c;//0.01 + 6.99 * c;
        var sol = Ode.GearBDF(
            0,
            new Vector(Ap, Yp, Bp, m, S),
            (t,x) => new Vector(
                (1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))*kbar1*(1-x[0])-kbar2*x[0]*(1-x[1])-kbar3*x[0]*(1-x[2]), //a
                alpha1*kbar2*x[0]*(1-x[1])-(kbar4+kbar6)*x[1], //y
                alpha2*kbar3*x[0]*(1-x[2])-kbar5*x[2], //b
                (gammaR*(1-(1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon)))))))-
                 gammaB*Math.Pow(x[2],2)*(1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))), //m
                h(x[1])*(1-x[4])   //S
            )
        );
        var points = sol.SolveFromTo(0, tSpan).ToArray();
        var result = points[points.Length-1];
        Ap = result.X[0];
        Yp = result.X[1];
        Bp = result.X[2];
        m  = result.X[3];
        S  = result.X[4];
    }

    public bool DecideState(float c)
    {
        SolveStiff(c);
        double r = rand.NextDouble();
        if( S > U )
        {
            S = 0.0f;
            U = UnityEngine.Random.Range(0.0f,0.8f);
            return false; //Tumbling
        }
        else
            return true; //Running 
    }
}
