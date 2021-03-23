using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupScene : MonoBehaviour
{
    [SerializeField] private GameObject eColi;

    // Start is called before the first frame update
    void Start()
    {
        Model model = Model.GetInstance();

        model.SimulateCells(20, 100);
        Cell[] cells = model.GetCells();

        for(int i = 0; i < cells.Length; i++)
        {
            IPointAdapter point = cells[i].GetNextLocation();
            Instantiate(eColi, new Vector3(point.GetX(), 1, point.GetZ()), Quaternion.Euler(0, cells[i].GetAngle(), 0));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
