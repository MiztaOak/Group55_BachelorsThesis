using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float d, i_0;
    // Start is called before the first frame update
    void Start()
    {
        EnvironmentFactory.CreateEnvironment(EnvironmentType.Basic, d, i_0, transform.position.x, transform.position.z, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
