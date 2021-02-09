using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{

    private Cell cell;
    private AbstractEnvironment env = new BasicEnvironment(1, 0.1f,0,0);
    private bool run;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateState());
    }

    void Update()
    {
        if(run)
            transform.Translate(Vector3.right * 2 * Time.deltaTime);
        else {
            float angle = Random.Range(-180.0f,180.0f);
            transform.Rotate(0.0f, angle, 0.0f, Space.World);
        }
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
            float c = env.getConcentration(pos.x, pos.z);
            print(c);
            yield return new WaitForSeconds(0.1f);
            cell.SetConcentration(c);
            yield return new WaitForSeconds(0.1f);
            this.run = cell.IsRun();
        }
    }
}
