using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

using Microsoft.Research.Oslo;

public class ODERegulation : ICellRegulation
{

    //Static values of total concentrations and chemical properties
    //Taken from tables 1 and 2 in ODEs in Chemotaxis-article
    private static double k1=34; //CheA autophosphorylation
    private static double k2=100; //phosphotransfer to CheY 
    private static double k3=15; //phosphotransfer to CheB
    private static double k4=1.6; //CheY-P deposphorylation by CheZ
    private static double k5=0.7; //Dephosphorylation of CheB-P
    private static double k6=0.085; //Dephosphorylation of CheY-P
    private static double gR=0.0375; //Methylation by CheR
    private static double gB=3.14; //Demethylation by CheB-P

    private static int N=18; //Number of (Tar) receptors

    //Receptor constants
    private static double Kaon=0.5; //Dissociation constant of active (Tar) receptor
    private static double Kaoff=0.02; //Dissociation constant of inactive (Tar) receptor

    //Total concentration (micromolar)
    private static double At=7.9;
    private static double Yt=9.7;
    private static double Bt=0.28;
    private static double Rt=0.16;
    private static double Zt=3.8;

    //Phosphorylated concentrations with initial values (arbitrary)
    private static double q=0.5; //experimental ratio
    private static double Ap0=q*At;
    private static double Yp0=q*Yt;
    private static double Bp0=q*Bt;
    private static double m0=5;

    //Keep track of calculated values
    private double Ap;
    private double Yp;
    private double Bp;
    private double m;
    
    private double Ap2;
    private double Yp2;
    private double Bp2;
    private double m2 = 10;
    private double S;
    private double U;
    

    private static double L; //Ligand concentration

    private static System.Random rand = new System.Random();
    private static VectorBuilder<double> V = Vector<double>.Build;
    private Vector<double> V0 = V.DenseOfArray(new double[] {Ap0,Yp0,Bp0,m0});

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

    private static Vector<double> ODE( double t, Vector<double> y)
    {
        Vector<double> dy = V.Dense(4);
        dy[0] = (1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))*k1*(At-y[0])-k2*y[0]*(Yt-y[1])-k3*y[0]*(Bt-y[2]); //a
        dy[1] = k2*y[0]*(Yt-y[1])-k4*y[1]*Zt-k6*y[1]; //y
        dy[2] = k3*y[0]*(Bt-y[2])-k5*y[2]; //b
        dy[3] = (gR*Rt*(1-(1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon)))))))-
                gB*Math.Pow(y[2],2)*(1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))); //m
        return dy;
    }

    private double h(double y)
    {
        return 0.02 + 0.5*y;
    }

    private void SolveStiff(float c)
    {
        L = (double) 0.01 + 6.99 * c;
        // L = (double) 10* c;
        var sol = Ode.GearBDF(
            0,
            new Vector(Ap2, Yp2, Bp2, m2, S),
            (t,x) => new Vector(
                (1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))*kbar1*(1-x[0])-kbar2*x[0]*(1-x[1])-kbar3*x[0]*(1-x[2]), //a
                alpha1*kbar2*x[0]*(1-x[1])-(kbar4+kbar6)*x[1], //y
                alpha2*kbar3*x[0]*(1-x[2])-kbar5*x[2], //b
                (gammaR*(1-(1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon)))))))-
                 gammaB*Math.Pow(x[2],2)*(1/(1+Math.Exp(N*(1-x[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))), //m
                h(x[1])*(1-x[4])   //S
            )
        );
        var points = sol.SolveFromTo(0, 1).ToArray();
        var result = points[points.Length-1];
        Ap2 = result.X[0];
        Yp2 = result.X[1];
        Bp2 = result.X[2];
        m2  = result.X[3];
        S   = result.X[4];
        Debug.Log("Conc.: " + L);
        // Debug.Log("Ap: " + Ap2);
        Debug.Log("Yp: " + Yp2);
        // Debug.Log("S: " + S);
        // Debug.Log("U: " + U);
        // Debug.Log("Bp: " + Bp2);
        // Debug.Log("m: " + m2);
    }

    public bool DecideState(float c)
    {
        SolveStiff(c);
        // var odeFunc = new Func<double, Vector<double>, Vector<double>>( ODE );
        // L = 0.09 + ( (double) c * 0.01 ); //Only values between 0.09 and 0.1 have effect 
        // Vector<double> V1 = RungeKutta.FourthOrder(V0, 0, 0.1, 1000, odeFunc)[900];
        // Ap = V1[0];
        // Yp = V1[1];
        // Bp = V1[2];
        // m = V1[3];
        // V0 = V.DenseOfArray(new double[] {Ap,Yp,Bp,m});
        // double bias = Yp / Yt;
        double r = rand.NextDouble();
        if( S > U )
        {
            S = 0.0f;
            U = UnityEngine.Random.Range(0.0f,1.0f);
            return false; //Tumbling
        }
        else
            return true; //Running 
    }
}
