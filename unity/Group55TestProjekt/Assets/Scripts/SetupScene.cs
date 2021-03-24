using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SetupScene : MonoBehaviour
{
    [SerializeField] private GameObject eColi;

    // Start is called before the first frame update
    void Start()
    {
        Model model = Model.GetInstance();


        Cell[] cells = model.GetCells();
        if (cells == null)
            return;

        for (int i = 0; i < cells.Length; i++) //create the E-Coli objects for the cells
        {
            IPointAdapter point = cells[i].GetNextLocation();
            Instantiate(eColi, new Vector3(point.GetX(), 1, point.GetZ()), Quaternion.Euler(0, cells[i].GetAngle(), 0));
        } 
    }  
}
