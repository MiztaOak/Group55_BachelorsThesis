using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour, ICellDeathListener
{
    private Cell cell;
    private bool run;

    public float moveSpeed;
    public float rotSpeed;
    [SerializeField] private bool smart;


    public float smartnessFactor;

    private Animator myAnimator;

    [SerializeField] private ParticleSystem deathParticle;

    private Material cellmaterial;

    private Rigidbody cellRigidBody;

    private Vector3 originalScale;

    private Vector3 nextLocation;

    private Model model;

    // Start is called before the first frame update
    void Start()
    {
        model = Model.GetInstance();

        if (!BacteriaFactory.IsForwardSimulation() || smart)
        {
            cell = BacteriaFactory.CreateNewCell(transform.position.x, transform.position.z, transform.rotation.y, smart);
            nextLocation = TranslateToVector3(cell.GetNextLocation()); //calculate the first location
        }
 
        myAnimator = GetComponent<Animator>();
        cellRigidBody = GetComponent<Rigidbody>();

        originalScale = transform.localScale;
        run = false; // set run to false so that it begins by rotating towards the first location
    }

    private void OnMouseUp()
    {
        GameObject oldFocus = GameObject.FindGameObjectWithTag("Player");
        if(oldFocus != null)
        {
            oldFocus.gameObject.tag = "Untagged";
            oldFocus.transform.Find("Cell").GetComponent<Renderer>().material.SetFloat("Boolean_E606F07D", 0);
        }

        gameObject.tag = "Player";
        transform.Find("Cell").GetComponent<Renderer>().material.SetFloat("Boolean_E606F07D", 1);
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

    public void SetCell(Cell cell)
    {
        this.cell = cell;
        cell.AddListener(this);
        nextLocation = TranslateToVector3(cell.GetNextLocation()); //calculate the first location
    }

    private Vector3 TranslateToVector3(IPointAdapter pointToTranslate) => new Vector3(pointToTranslate.GetX(), transform.position.y, pointToTranslate.GetZ());

    public void Notify()
    {
        Instantiate(deathParticle, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1,
            gameObject.transform.position.z), gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
