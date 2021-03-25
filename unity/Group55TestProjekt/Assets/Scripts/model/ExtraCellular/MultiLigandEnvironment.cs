using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLigandEnvironment : AbstractEnvironment
{
    private Environment[] environments;
    private float max = 0;

    public MultiLigandEnvironment(float[] d, float[] i_0, float[] x, float[] z, float[] max, int n)
    {
        environments = new Environment[n];
        for(int i = 0; i < n; i++)
        {
            this.max += max[i];
            environments[i] = new Environment(d[i], i_0[i], x[i], z[i], max[i]);
        }
        
    }

    public override float getConcentration(float x, float z)
    {
        float c = 0;
        foreach(Environment environment in environments)
        {
            c += environment.getConcentration(x, z);
        }

        return c;
    }

    public override float GetMaxVal()
    {
        return max;
    }

    public override float GradX(float x, float z)
    {
        float gradX = 0;
        foreach(Environment environment in environments)
        {
            gradX += environment.GradX(x, z);
        }
        return gradX;
    }

    public override float GradZ(float x, float z)
    {
        float gradZ = 0;
        foreach (Environment environment in environments)
        {
            gradZ += environment.GradZ(x, z);
        }
        return gradZ;
    }
}
