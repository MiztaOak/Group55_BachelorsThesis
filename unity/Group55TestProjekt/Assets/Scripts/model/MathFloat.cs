using System;

//Class that adapts System.Math to use float instead of double 
public class MathFloat
{
    public static readonly float PI = (float)Math.PI;

    public static float Pow(float n, float m)
    {
        return (float)Math.Pow(n, m);
    }

    public static float Sin(float x)
    {
        return (float)Math.Sin(x);
    }

    public static float Cos(float x)
    {
        return (float)Math.Cos(x);
    }

    public static float Clamp(float value, float min, float max)
    {
        return value > max ? max : (value < min ? min : value); 
    }

    public static float Exp(float n)
    {
        return (float)Math.Exp(n);
    }

    public static float Atan2(float x,float y)
    {
        return (float)Math.Atan2(x, y);
    }

    public static float Log(float x)
    {
        return (float)Math.Log(x);
    }
}
