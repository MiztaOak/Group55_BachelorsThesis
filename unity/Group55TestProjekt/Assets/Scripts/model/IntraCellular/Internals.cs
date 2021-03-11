using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Class representing the internals of the cell this is for the non smart version
 * The class handles the calculation of the cells movement and returns the next point when needed 
*/
public class Internals : IInternals
{
    private Model model;
    private ICellRegulation regulator;
    private IPointAdapter location;

    private readonly float v; //velocity
    private readonly float dT; //time step
    private float angle; //current angle

    public Internals(float x, float z, float v, float dT, float angle)
    {
        this.model = Model.GetInstance();
        this.regulator = new HazardRegulation();

        location = new Vector3Adapter(x, z);

        this.v = v;
        this.dT = dT;
        this.angle = angle;
    }

    private void CalculateNextLocation()
    {
        angle = CalculateTumbleAngle();

        float dX = v * dT * Mathf.Cos(angle), dZ = v * dT * Mathf.Sin(angle);

        while(GetRunningState(location.GetX(), location.GetZ()))
        {
            location.Add(dX, dZ);

            if (location.GetX() + dX > 14 || location.GetX() + dX < 14 || location.GetZ() + dZ > 14 || location.GetZ() + dZ < 14)
                break;
        }
    }

    private float CalculateTumbleAngle()
    {
        float dZ = location.GetZ() - model.environment.GetZ(), dX = location.GetX() - model.environment.GetX();

        float correctAngle = Mathf.Atan2(dZ,dX)+Mathf.PI;
        float errorAngle = Random.Range(0f, Mathf.PI/2) * (Random.value <= 0.5 ? 1 : -1);
        return correctAngle + errorAngle;
    }

    private bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        return run;
    }

    public IPointAdapter getNextLocation()
    {
        CalculateNextLocation();
        return location;
    }
}
