using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStartUpScript : MonoBehaviour
{
    public float d, i_0;
    // Start is called before the first frame update
    void Start()
    {
        EnvironmentFactory.CreateBasicEnvionment(d, i_0, transform.position.x, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
