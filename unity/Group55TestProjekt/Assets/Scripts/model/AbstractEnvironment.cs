
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

    //method that returns the consentration for a given postion to be implemnted by the specific sub class
    public abstract float getConsentration(float x, float y);

}
