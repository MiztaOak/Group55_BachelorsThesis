using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour {

    public float speed;
    public float rotSpeed; 
   // public GameObject GameObject;
    private Vector3 walkdest;
    bool rotated = false;
    Vector3 testDest = new Vector3(0, 1, 1);

    void Start()
    {
        //walkdest = transform.position;
        // StartCoroutine("nextDest");
        newDestReq();
    }

    void Update() {
        StartCoroutine(Go(walkdest, speed));
    }
    IEnumerator Go(Vector3 destination, float speed) {
        while (transform.position != destination) {
            // First step, angle the cell towards the destination

            // Create a test destination

            
            tumble(destination);
            //tumble(destination);
            if (rotated) walk(destination);
            // Second, wait for the cell to rotate and move in a straight line towards the destination
            
            yield return null; // Wait until next frame to execute again
            // print("MOVING");
        }
        // print("DONE");
        newDestReq();
    }
    void tumble(Vector3 dest) {
        Quaternion newRot = Quaternion.LookRotation(dest - transform.position);
        newRot = Quaternion.Euler(0, newRot.eulerAngles.y+90,0);
        Debug.DrawLine(transform.position, dest, Color.red);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotSpeed * Time.deltaTime);
        if (transform.rotation == newRot) {
            rotated = true;
        } else {
            rotated = false;
        }
    }
    void walk(Vector3 dest) {
        transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
        // print(transform.position);
    }

    
    // Generates a new random destination every second
    IEnumerator nextDest() {
        for (; ; ) {
            // Generate random walk distance
            
            float randDist = Random.Range(-5, 5);
            while (randDist == 0) randDist = Random.Range(-5, 5); // Make a new random number if length is 0 
            // Generate new angle for tumbling
            float randAngle = Random.Range(1, 359);
            Vector3 vector3 = new Vector3(Mathf.Cos(randAngle) * randDist, 0, Mathf.Sin(randAngle) * randDist);
            walkdest = transform.position + vector3;
            yield return new WaitForSeconds(1);
        }
    }


    // Generates a new destination 
    void newDestReq() {
        // Generate random walk distance 
        float randDist = Random.Range(-5, 5);
         // Generate new angle for tumbling
        float randAngle = Random.Range(0, 359);
        Vector3 vector3 = new Vector3(Mathf.Cos(randAngle) * randDist, 0, Mathf.Sin(randAngle) * randDist);

        walkdest = transform.position + vector3;
    }
}
