using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StochasticRegulation : ICellRegulation
{

    //Probabilities of reactions
    private float cAY = 0.99f;
    private float cAB = 0.4f;
    private float cA = 0.9f;
    private float cMR = 0.007f;
    private float cMB = 0.01f;
    private float cZ = 0.01f;
    private float cB = 0.01f;

    //Amount of reactants
    private int y = 70;
    private int yp = 30;
    private int a = 70;
    private int ap = 30;
    private int b = 70;
    private int bp = 30;
    private int z = 100;
    private int r = 100;

    private static float tMax = 0.01f; //total reaction time tick

    private static int m0 = 30; //Initial methylation levels
    private int m = m0; //Current levels

    private int[] calculateHVector()
    {
        int[] h = { ap*y, ap*b, a, r, bp, z*yp, bp };
        return h;
    }

    public bool DecideState(float c)
    {
        int[] h = new int[7]; //Possible combinations of reactants
        // m -= (int) c*m0; //high concentrations decrease methylation
        float t = 0.0f; //Start time
        while(t < tMax)
        {
            // Debug.Log("m: " + m);
            // cA = Mathf.Max(0, Mathf.Min(1, cA*(m/m0)));
            // Debug.Log("CA: " + cA);
            h = calculateHVector();
            float[] p = {cAY, cAB, cA, cMR, cMB, cZ, cB};
            float[] v = h.Zip(p, (x1,x2) => x1*x2).ToArray(); //ai = hi*ci
            float a0 = v.Sum();
            float r1 = Random.Range(0.0f,1.0f);
            float r2 = Random.Range(0.0f,1.0f);
            float tau = (1/a0)*Mathf.Log(1/r1);
            int mu = 0;
            float v1 = 0;
            float v2 = 0;
            for(int i=0; i < v.Length-1; i++)
            {
                v1 += v[i];
                v2 = v1 + v[i+1];
                if( v1 < r2*a0 && r2*a0 <= v2 )
                {
                    mu = i+1;
                    break;
                }
            }
            switch(mu)
            { //The different reactions
                case 0 : ap--; a++; yp++; y--; break;
                case 1 : ap--; a++; bp++; b--; break;
                case 2 : a--; ap++; break;
                case 3 : m++; break;
                case 4 : m--; break;
                case 5 : yp--; y++; break;
                case 6 : bp--; b++; break;
                default : break;
            }
            t += tau;
            Debug.Log("YP: " + yp);
        }
        float rand = Random.Range(0.0f,1.0f);
        float bias = yp/(y+yp);
        if(bias > rand)
            return false; //tumble
        else
            return true; //run
    }
}
