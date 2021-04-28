
//Abstract version of the internals containing the code that is common for the non-smart internals
public abstract class AbstractInternals : IInternals
{
    protected readonly float v; //velocity
    protected readonly float dT; //time step
    protected float angle; //current angle

    protected Model model;
    protected ICellRegulation regulator;

    public abstract IInternals Copy();
    public abstract State GetInternalState();
    public abstract IPointAdapter GetNextLocation();
    public abstract bool IsDead();
    public abstract bool IsSplit();

    public AbstractInternals(float v, float dT, float angle, ICellRegulation regulator)
    {
        this.v = v;
        this.dT = dT;
        this.angle = angle;

        model = Model.GetInstance();
        this.regulator = regulator;
    }

    protected void CalculateNextLocation(IPointAdapter location)
    {
        float dX = v * dT * MathFloat.Cos(angle), dZ = v * dT * MathFloat.Sin(angle);

        while (location.GetX() + dX > 14 || location.GetX() + dX < -14 || location.GetZ() + dZ > 14 || location.GetZ() + dZ < -14)
        {
            angle = CalculateTumbleAngle();
            dX = v * dT * MathFloat.Cos(angle);
            dZ = v * dT * MathFloat.Sin(angle);
        }
        location.Add(dX, dZ);
    }

    protected bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        return run;
    }

    protected float CalculateTumbleAngle()
    {
        //Tumble angle based on article (Edgington)
        float newAngle = RandomFloat.Range(18f, 98f);
        float rand = RandomFloat.Range(0.0f, 1.0f);
        if (rand > 0.5)
            newAngle *= -1;
        newAngle *= MathFloat.PI / 180;
        newAngle += this.angle;
        return newAngle;
    }

    public virtual float GetAngle()
    {
        return angle;
    }
}
