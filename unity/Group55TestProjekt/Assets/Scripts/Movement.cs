using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour
{
    private Cell cell;
    private bool run;

    public float moveSpeed;
    public float rotSpeed;
    [SerializeField] private readonly bool smart;
    public float smartnessFactor;

    private Animator myAnimator;

    private Material cellmaterial; 

    private Rigidbody cellRigidBody;

    private Vector3 originalScale;

    private Vector3 nextLocation;

    private Model model;

    // Start is called before the first frame update
    void Start()
    {
        model = Model.GetInstance();

        if (BacteriaFactory.IsForwardSimulation() && !smart)
            cell = model.GetCell();
        else
            cell = BacteriaFactory.CreateNewCell(transform.position.x,transform.position.z, transform.rotation.y,smart);

        myAnimator = GetComponent<Animator>();
        cellRigidBody = GetComponent<Rigidbody>();
        
        originalScale = transform.localScale;
        nextLocation = TranslateToVector3(cell.GetNextLocation()); //calculate the first location
        run = false; // set run to false so that it begins by rotating towards the first location
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(0);
        }

    }
    /*
    private void OnMouseOver() 
    {
        // Only allow one cell to be selected at once
        if ((Input.GetMouseButtonDown(0)) && gameObject.CompareTag("Untagged") && !GameObject.FindGameObjectWithTag("Player")) {
            gameObject.tag = "Player";

            CellInfo.focusedCell = cell; // send info the info panel

            // Change color
            cellmaterial = transform.Find("Cell").GetComponent<Renderer>().material;
            cellmaterial.SetFloat("Boolean_E606F07D", 1); 
            // Make a bit bigger 
            transform.localScale += new Vector3(0.05F, 0.05F, 0.05F);
        } else if ((Input.GetMouseButtonDown(0)) && gameObject.CompareTag("Player")) {
            gameObject.tag = "Untagged";
            cellmaterial.SetFloat("Boolean_E606F07D", 0);
            cellmaterial = null;
            transform.localScale = originalScale;
        }
    }*/

    private void OnMouseUp()
    {
        gameObject.tag = "Player";
        CellInfo.focusedCell = cell;
    }

    private void FixedUpdate() //update that has to be used for the rigid body if not the collisions wont work
    {
        if (cell.IsDone())
            return; 

        Vector3 currentLocation = cellRigidBody.position;

        if (currentLocation == nextLocation) //if at new location request the next location
        {
            nextLocation = TranslateToVector3(cell.GetNextLocation());         

            myAnimator.SetBool("Rotating", true);
            run = false;          
        }  

        //Rotates the cell towards the next location
        Quaternion newRot = Quaternion.LookRotation(nextLocation - currentLocation);
        newRot = Quaternion.Euler(0, newRot.eulerAngles.y + 90, 0);
        Quaternion moveRot = Quaternion.Lerp(transform.rotation, newRot, rotSpeed * Time.deltaTime * model.GetTimeScaleFactor());
        
        cellRigidBody.MoveRotation(moveRot);
        Debug.DrawLine(transform.position, nextLocation, Color.red);

        //if the cell has rotated to a close enought angle begin moving and remove the rotation animation
        if (Mathf.Abs(newRot.eulerAngles.y - cellRigidBody.rotation.eulerAngles.y) < 5f && !run)
        {
            run = true;
            myAnimator.SetBool("Rotating", false);
        }

        if (run) //move towards the location
        {
            cellRigidBody.MovePosition(Vector3.MoveTowards(currentLocation, nextLocation, moveSpeed * Time.deltaTime * model.GetTimeScaleFactor()));
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider other) //this should not be needed since the cell should not hit the walls but who knows
    {
        if (other.gameObject.name.Substring(0, 4) == "Wall")
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(Mathf.Clamp(pos.x, -14f, 14f), pos.y, Mathf.Clamp(pos.z, -14f, 14f)); //dumb solution plz fix
        }
        
    }

    private Vector3 TranslateToVector3(IPointAdapter pointToTranslate) => new Vector3(pointToTranslate.GetX(), transform.position.y, pointToTranslate.GetZ());
}
