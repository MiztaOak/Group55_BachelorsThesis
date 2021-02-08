
public abstract class AbstractEnvironment
{
    protected float xCord, yCord; //postion of the source
    
    public AbstractEnvironment(float xCord, float yCord)
    {
        this.xCord = xCord;
        this.yCord = yCord;
    }

    public AbstractEnvironment() : this(0, 0) { } //defaul constructor
    //TODO change to better base case

    public abstract float getConcentration(float x, float y);

}
