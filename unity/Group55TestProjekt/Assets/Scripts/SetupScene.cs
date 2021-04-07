using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SetupScene : MonoBehaviour, ICellBirthListener
{
    [SerializeField] private Movement eColi;
    [SerializeField] private GameObject food;

    public void Notify(Cell cell)
    {
        IPointAdapter point = cell.GetNextLocation();
        Movement tmp = Instantiate(eColi, new Vector3(point.GetX(), 1, point.GetZ()), Quaternion.Euler(0, cell.GetAngle(), 0));
        tmp.SetCell(cell);
    }

    // Start is called before the first frame update
    void Start()
    {
        Model model = Model.GetInstance();
        model.AddListener(this);
        List<Cell> cells = model.GetCells(0);
        if (cells == null)
            return;

        for (int i = 0; i < model.GetNumCells(0); i++) //create the E-Coli objects for the cells
        {
            IPointAdapter point = cells[i].GetNextLocation();
            Movement tmp = Instantiate(eColi, new Vector3(point.GetX(), 1, point.GetZ()), Quaternion.Euler(0, cells[i].GetAngle(), 0));
            tmp.SetCell(cells[i]);
        }

        AbstractEnvironment environment = model.environment;
        if(environment is MultiLigandEnvironment)
        {
            foreach(Environment env in ((MultiLigandEnvironment)environment).GetEnvironments())
            {
                Instantiate(food, new Vector3(env.GetX(), 1.88f, env.GetZ()), Quaternion.identity);
            }
        }
        else
        {
            Instantiate(food, new Vector3(environment.GetX(), 1.88f, environment.GetX()), Quaternion.identity);
        }

        
    }  
}
