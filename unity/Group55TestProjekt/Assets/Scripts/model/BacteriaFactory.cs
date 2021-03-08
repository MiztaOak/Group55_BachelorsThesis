using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaFactory
{
    private static BacteriaFactory instance;
    private float v; //speed of the cell might not be needed but is nice not to have to send in as a parameter
    private float dT; //time delta for the cell
    private float smartnessFactor; //smartness factor of the smart cell

    private RegulatorType regulatorType; //type of regulator that should be used

    private BacteriaFactory() //sets the default values
    {
        v = 0.7f;
        dT = 0.05f;
        smartnessFactor = .75f;

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
            return new Cell(new SmartInternals(x, z, dT, v, smartnessFactor));
        ICellRegulation regulator;
        switch (regulatorType)
        {
            case RegulatorType.ODE:
                regulator = new ODERegulation();
                break;
            case RegulatorType.Hazard:
                regulator = new HazardRegulation();
                break;
            case RegulatorType.Distance:
                regulator = new DistRegulation();
                break;
            case RegulatorType.Bernoulli:
                regulator = new BernoulliRegulation();
                break;
            default:
                regulator = new BasicRegulation();
                break;
        }
    
        return new Cell(new Internals(x,z,v,dT,angle,regulator));
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
        this.smartnessFactor = Mathf.Clamp(smartnessFactor,0,1);
    }

    public void SetRegulatorType(RegulatorType regulatorType)
    {
        this.regulatorType = regulatorType;
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
}
