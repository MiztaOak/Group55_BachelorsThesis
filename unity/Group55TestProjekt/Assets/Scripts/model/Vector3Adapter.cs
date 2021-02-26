using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Adapter : IPointAdapter
{
    private Vector3 point;

    public Vector3Adapter(float x, float z)
    {
        this.point = new Vector3(x,z);
    }

    public void Add(float x, float z)
    {
        point += new Vector3(x, 0, z);
    }

    public float GetX()
    {
        return point.x;
    }

    public float GetZ()
    {
        return point.z;
    }

    public void SetX(float x)
    {
        point.x = x;
    }

    public void SetZ(float z)
    {
        point.z = z;
    }

    public IPointAdapter Copy()
    {
        return new Vector3Adapter(point.x, point.z);
    }

    public bool Equals(Vector3Adapter v3)
    {
        return point.x == v3.GetX() && point.z == v3.GetZ();
    }
}
