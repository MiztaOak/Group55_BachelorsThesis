using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPointAdapter
{
    float GetX();
    float GetZ();
    void SetX(float x);
    void SetZ(float z);
    void Add(float x, float z);

    IPointAdapter Copy();
    bool Equals(IPointAdapter other);
}
