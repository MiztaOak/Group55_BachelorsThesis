using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour {
    public bool isIdle;
    public float moveSpeed;
    public float rotSpeed;
    private Animator myAnimator;
    private Vector3 walkdest;
    bool rotating = false;
    bool req = true;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Update() {
        if (!isIdle) StartCoroutine(Go(walkdest, moveSpeed));
        if (req) newDestReq();
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(0);
        }
        
    }
    IEnumerator Go(Vector3 destination, float speed) {
  
        while (transform.position != destination) {
            // First step, angle the cell towards the destination
            tumble(destination,rotSpeed);
            //tumble(destination);
            if (!rotating) walk(destination, speed);
            // Second, wait for the cell to rotate and move in a straight line towards the destination
            yield return null; // Wait until next frame to execute again
        }
        req = true; 
    }
    void tumble(Vector3 dest,float speed) {
        req = false;
        Quaternion newRot = Quaternion.LookRotation(dest - transform.position);
        newRot = Quaternion.Euler(0, newRot.eulerAngles.y+90,0);
        Debug.DrawLine(transform.position, dest, Color.red);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, speed * Time.deltaTime);
        if (transform.rotation == newRot) {
            rotating = false;
            myAnimator.SetBool("Rotating", false);
           // print("ROT FALSE");
        }
    }
    void walk(Vector3 dest, float speed) {
        transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
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
         // Make sure only to request once
        rotating = true;
        myAnimator.SetBool("Rotating", true);
        print("REQUESTING");
        // Generate random walk distance 
        float randDist = Random.Range(-5, 5);
         // Generate new angle for tumbling
        float randAngle = Random.Range(0, 359);
        Vector3 vector3 = new Vector3(Mathf.Cos(randAngle) * randDist, 0, Mathf.Sin(randAngle) * randDist);

        walkdest = transform.position + vector3;
        req = false;
    }





    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 8)
        {
            Destroy(other.gameObject);
            Debug.Log("dfddfdf");
        }
        Debug.Log("aaaaaaaaaaaa");
    
    }
}
