
public abstract class AbstractEnvironment
{
    protected float xCord, zCord; //postion of the source
    protected bool isDynamic = false;

    public AbstractEnvironment(float xCord, float zCord)
    {
        this.xCord = xCord;
        this.zCord = zCord;
    }

    public AbstractEnvironment() : this(0, 0) { } //defaul constructor
    //TODO change to better base case


    //method that returns the concentration for a given postion to be implemnted by the specific sub class
    public abstract float getConcentration(float x, float z);

    public float GetConcentration(float x, float z, int timeStep)
    {
        float c = getConcentration(x, z);

        if (isDynamic)
        {
            int n = Model.GetInstance().GetNumOfCloseCells(timeStep, 1, new Vector3Adapter(x, z));
            c /= n != 0 ? n : 1;
        }

        return c;
    }

    public float GetX()
    {
        return xCord;
    }

    public float GetZ()
    {
        return zCord;
    }

    public abstract float GradX(float x, float z);


    public abstract float GradZ(float x, float z);

    public abstract float GetMaxVal();

    public bool IsDynamic()
    {
        return isDynamic;
    }
}
