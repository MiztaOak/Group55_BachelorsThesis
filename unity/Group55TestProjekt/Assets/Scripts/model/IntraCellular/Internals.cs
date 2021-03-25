﻿using System.Collections;
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

    public Internals(float x, float z, float v, float dT, float angle, ICellRegulation regulator)
    {
        this.model = Model.GetInstance();
        this.regulator = regulator;

        location = new Vector3Adapter(x, z);

        this.v = v;
        this.dT = dT;
        this.angle = angle;
    }

    private void CalculateNextLocation()
    {
        angle = CalculateTumbleAngle();

        float dX = v * dT * Mathf.Cos(angle), dZ = v * dT * Mathf.Sin(angle);
        while (GetRunningState(location.GetX(), location.GetZ()))
        {
            location.Add(dX, dZ);
            if (location.GetX() + dX > 14 || location.GetX() - dX < -14 || location.GetZ() + dZ > 14 || location.GetZ() - dZ < -14)
                break;
        }
    }

    //Returns absolute tumble angle in radians
    private float CalculateTumbleAngle()
    {
        //Help bacteria tumble in general direction of food
        // float dZ = location.GetZ() - model.environment.GetZ(), dX = location.GetX() - model.environment.GetX();

        // float correctAngle = Mathf.Atan2(dZ,dX)+Mathf.PI;
        // float errorAngle = Random.Range(0f, Mathf.PI/2) * (Random.value <= 0.5 ? 1 : -1);
        // return correctAngle + errorAngle;

        //Tumble angle based on article (Edgington)
        float newAngle = Random.Range(18f, 98f);
        float rand = Random.Range(0.0f, 1.0f);
        if (rand > 0.5)
            newAngle *= -1;
        newAngle *= Mathf.PI / 180;
        newAngle += this.angle;
        return newAngle;
    }

    private bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        return run;
    }

    public IPointAdapter GetNextLocation()
    {
        CalculateNextLocation();
        return location;
    }

    public State GetInternalState()
    {
        State state = new State();
        if (this.regulator is ODERegulation)
        {
            ODERegulation r = (ODERegulation)this.regulator;
            state.yp = r.GetYP();
            state.ap = r.GetAP();
            state.bp = r.GetBP();
            state.m = r.GetM();
            state.l = r.GetL();
        }
        return state;
    }

    public float GetAngle()
    {
        return angle;
    }
}
