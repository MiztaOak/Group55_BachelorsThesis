using System.Collections;
using System.Collections.Generic;

public class RandomFloat
{
    private static System.Random random;
    public static float Range(float min, float max)
    {
        if (random == null)
            random = new System.Random();
        //System.Random random = new System.Random();
        double n = random.NextDouble()*(max - min) + min;
        return (float)n;
    }

    public static float NextFloat()
    {
        return Range(0f, 1f);
    }
}
