using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{

    private Cell cell;
    private BasicEnvironment basicEnviroment = new BasicEnvironment(1, 0.1f,0,0);
    private bool run;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateState());
    }

    void Update()
    {
        if(run)
            transform.Translate(Vector3.right * Time.deltaTime);
        else
            transform.Rotate(0.0f, 1.0f, 0.0f, Space.World);
    }

    private void Awake ()
    {
        cell = GetComponent<Cell>();
    }

    IEnumerator UpdateState()
    {
        while (true)
        {
            Vector3 pos = transform.position;
            float c = basicEnviroment.getConcentration(pos.x, pos.y);
            print(c);
            yield return new WaitForSeconds(1);
            cell.SetConcentration(c);
            yield return new WaitForSeconds(0.5f);
            this.run = cell.IsRun();
        }
    }
}
