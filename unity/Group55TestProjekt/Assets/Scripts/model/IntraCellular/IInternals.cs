using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInternals
{
    IPointAdapter GetNextLocation();
    State GetInternalState();
    float GetAngle();
}
