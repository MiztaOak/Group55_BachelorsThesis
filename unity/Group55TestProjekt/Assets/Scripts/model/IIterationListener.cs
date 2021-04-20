using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIterationListener
{
    void NotifyIterationChanged(int interation);
}
