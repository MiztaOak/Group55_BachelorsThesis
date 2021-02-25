using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

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

    //Used to calculate tumble bias, arbitrary? Calculate somehow?
    private static double YStarved = 4.083;

    //Keep track of calculated values
    private double Ap;
    private double Yp;
    private double Bp;
    private double m;
    

    private static double L; //Ligand concentration

    private static System.Random rand = new System.Random();
    private static VectorBuilder<double> V = Vector<double>.Build;
    private Vector<double> V0 = V.DenseOfArray(new double[] {Ap0,Yp0,Bp0,m0});

    private static Vector<double> ODE( double t, Vector<double> y)
    {
        Vector<double> dy = V.Dense(4);
        dy[0] = (1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon))))))*k1*(At-y[0])-k2*y[0]*(Yt-y[1])-k3*y[0]*(Bt-y[2]);
        dy[1] = k2*y[0]*(Yt-y[1])-k4*y[1]*Zt-k6*y[1];
        dy[2] = k3*y[0]*(Bt-y[2])-k5*y[2];
        dy[3] = (gR*Rt*(1-(1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon)))))))-
                gB*Math.Pow(y[2],2)*(1/(1+Math.Exp(N*(1-y[3]/2+Math.Log((1+L/Kaoff)/(1+L/Kaon)))))));
        return dy;
    }

    public bool DecideState(float c)
    {
        var odeFunc = new Func<double, Vector<double>, Vector<double>>( ODE );
        L = 0.09 + ( (double) c * 0.01 ); //Only values between 0.09 and 0.1 have effect 
        Vector<double> V1 = RungeKutta.FourthOrder(V0, 0, 0.1, 1000, odeFunc)[900];
        Ap = V1[0];
        Yp = V1[1];
        Bp = V1[2];
        m = V1[3];
        V0 = V.DenseOfArray(new double[] {Ap,Yp,Bp,m});
        // double bias = 1 / (1 + (3/7)*Math.Pow((Yp/YStarved), 5.5)); //Taken from article, always 1??
        double bias = Yp / Yt;
        double r = rand.NextDouble();
        if( bias > r )
            return false; //Tumbling
        else
            return true; //Running 
    }
}
