
public interface ILifeRegulator
{
    bool Split(float c);  
   
    bool Die(float c);

    float GetLife();

    float GetDeath();
}
