
/*Class representing the internals of the cell this is for the non smart version
 * The class handles the calculation of the cells movement and returns the next point when needed 
*/
public class Internals : AbstractInternals
{
    private IPointAdapter location;


    public Internals(float x, float z, float v, float dT, float angle, ICellRegulation regulator): base(v,dT,angle,regulator)
    {
        location = new Vector3Adapter(x, z);
    }

    public override IPointAdapter GetNextLocation()
    {
        if (!GetRunningState(location.GetX(), location.GetZ())) //check if tumble
            angle = CalculateTumbleAngle();
        CalculateNextLocation(location); //calculate next position
        return location;
    }

    public override State GetInternalState()
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

    public override IInternals Copy()
    {
        return new Internals(location.GetX(), location.GetZ(), v, dT, angle, regulator.Copy());
    }

    public override bool IsDead()
    {
        return false;
    }

    public override bool IsSplit()
    {
        return false;
    }
}
