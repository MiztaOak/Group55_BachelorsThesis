using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRegulation : ICellRegulation
{
    //Basic comparison approach
    private float dc = 0;

    public bool DecideState(float c)
    {

        int x = 5;
        if ((this.dc != 0) && (c / this.dc) < 1) //Shift likelihood of tumbling
            x += 2;
        else
            x -= 2;
        float rand = Random.Range(0.0f, 1.0f);
        this.dc = c;
        if (rand * 10 > x)
            return true; //running
        else
            return false; //Tumbling
    }
}
