
public interface ICellRegulation
{
    bool DecideState(float concentration);
    ICellRegulation Copy();
}
