using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool run = true;
    private float c = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetConcentration(float c)
    {
        this.c = c;
        DecideState();
    }

    public bool IsRun()
    {
        return this.run;
    }

    private void DecideState()
    {
        int rand = Random.Range(0,10);
        if(rand <= 10*this.c)
            this.run = true;
        else 
            this.run = false;
    }
}
