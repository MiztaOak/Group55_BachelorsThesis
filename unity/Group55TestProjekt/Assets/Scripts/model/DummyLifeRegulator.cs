using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyLifeRegulator : ILifeRegulator
{
    public bool Die(float c)
    {
        return false;
    }

    public float GetDeath()
    {
        return 0;
    }

    public float GetLife()
    {
        return 0;
    }

    public bool Split(float c)
    {
        return false;
    }
}
