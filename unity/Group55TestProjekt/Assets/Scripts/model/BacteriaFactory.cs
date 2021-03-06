﻿using System.Collections;
using System.Collections.Generic;

public class BacteriaFactory
{
    private static BacteriaFactory instance;
    private float v; //speed of the cell might not be needed but is nice not to have to send in as a parameter
    private float dT; //time delta for the cell
    private float smartnessFactor; //smartness factor of the smart cell
    private bool deathAndDivision;

    private int iterations;

    private RegulatorType regulatorType; //type of regulator that should be used

    private BacteriaFactory() //sets the default values
    {
        v = 1.4f;
        dT = 0.1f;
        smartnessFactor = .75f;
        iterations = 0;
        deathAndDivision = true;

        regulatorType = RegulatorType.ODE; 
    }

    public static BacteriaFactory GetInstance()
    {
        if (instance == null)
            instance = new BacteriaFactory();
        return instance;
    }

    public Cell CreateCell(float x, float z, float angle, bool smart)
    {
        if (smart)
            return new Cell(new SmartInternals(x, z, 1, v, smartnessFactor));
        ICellRegulation regulator;
        switch (regulatorType)
        {
            case RegulatorType.ODE:
                regulator = new ODERegulation();
                break;
            case RegulatorType.Delta:
                regulator = new DeltaRegulation();
                break;
            default:
                regulator = new DeltaRegulation();
                break;
        }
        
        if(iterations == 0)
            return new Cell(new Internals(x,z,v,dT,angle,regulator));
        else
            return new Cell(new ForwardInternals(x, z, v, dT, angle, regulator,iterations));
    }

    public ILifeRegulator GetLifeRegulator()
    {
        if (deathAndDivision)
            return new LifeRegulator();
        return new DummyLifeRegulator();
    }

    //static version of the previous method might be nicer to use in code
    public static Cell CreateNewCell(float x, float z, float angle, bool smart)
    {
        return GetInstance().CreateCell(x, z, angle, smart);
    }

    public void SetV(float v)
    {
        this.v = v;
    }

    public void SetDT(float dT)
    {
        this.dT = (dT > 0.1 ? dT : 0.1f);
    }

    public void SetSmartnessFactor(float smartnessFactor)
    {
        this.smartnessFactor = MathFloat.Clamp(smartnessFactor,0,1);
    }

    public void SetRegulatorType(RegulatorType regulatorType)
    {
        this.regulatorType = regulatorType;
    }

    public void SetIterations(int iterations)
    {
        this.iterations = iterations;
    }

    public void SetDeathAndDivision(bool deathAndDivision)
    {
        this.deathAndDivision = deathAndDivision;
    }

    public RegulatorType GetRegulatorType()
    {
        return regulatorType;
    }

    //static versions of the setters
    public static void SetCellV(float v)
    {
        GetInstance().SetV(v);
    }

    public static void SetCellDT(float dT)
    {
        GetInstance().SetDT(dT);
    }

    public static void SetCellSmartnessFactor(float smartnessFactor)
    {
        GetInstance().SetSmartnessFactor(smartnessFactor);
    }

    public static void SetCellRegulatorType(RegulatorType regulatorType)
    {
        GetInstance().SetRegulatorType(regulatorType);
    }

    public static void SetCellIterations(int iterations)
    {
        GetInstance().SetIterations(iterations);
    }

    public static void SetCellDeathAndDivision(bool deathAndDivision)
    {
        GetInstance().SetDeathAndDivision(deathAndDivision);
    }

    public static bool IsForwardSimulation()
    {
        return GetInstance().iterations != 0;
    }

    public static int GetIterations()
    {
        return GetInstance().iterations;
    }
}
