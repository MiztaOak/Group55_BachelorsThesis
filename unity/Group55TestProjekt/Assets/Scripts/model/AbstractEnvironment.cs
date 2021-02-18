
public abstract class AbstractEnvironment
{
    protected float xCord, zCord; //postion of the source
    
    public AbstractEnvironment(float xCord, float zCord)
    {
        this.xCord = xCord;
        this.zCord = zCord;
    }

    public AbstractEnvironment() : this(0, 0) { } //defaul constructor
    //TODO change to better base case


    //method that returns the concentration for a given postion to be implemnted by the specific sub class
    public abstract float getConcentration(float x, float z);


}
