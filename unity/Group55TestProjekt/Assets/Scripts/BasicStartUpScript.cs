using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStartUpScript : MonoBehaviour
{
    public float d, i_0, x, y;
    // Start is called before the first frame update
    void Start()
    {
        EnvironmentFactory.CreateEnvironment(EnvironmentType.Basic, d, i_0, x, y, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
