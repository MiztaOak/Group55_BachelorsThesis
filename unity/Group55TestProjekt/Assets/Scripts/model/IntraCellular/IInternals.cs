
public interface IInternals
{
    IPointAdapter GetNextLocation();
    State GetInternalState();
    float GetAngle();
    IInternals Copy();
    bool IsDead();
    bool IsSplit();
}
