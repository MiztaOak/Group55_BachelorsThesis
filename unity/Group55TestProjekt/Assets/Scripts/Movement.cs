using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    private Cell cell;
    private bool run;

    public float moveSpeed;
    public float rotSpeed;
    private Animator myAnimator;

    private Rigidbody cellRigidBody;

    private int rotDir;

    // Start is called before the first frame update
    void Start()
    {
        cell = new Cell();
        myAnimator = GetComponent<Animator>();
        cellRigidBody = GetComponent<Rigidbody>(); //might have to be used later
        StartCoroutine(UpdateState());
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(0);
        }

        /*if (run)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime*-1);
        else
        {
            float angle = Random.Range(-180.0f, 180.0f);
            transform.Rotate(0.0f, rotDir*rotSpeed, 0.0f, Space.World);
        }*/
    }

    IEnumerator UpdateState()
    {
        Debug.Log("test");
        while (true)
        {
            Vector3 pos = transform.position;
            yield return new WaitForSeconds(1.0f);
            bool tmp = cell.GetRunningState(pos.x, pos.z);
            if (tmp && !run)
            {
                myAnimator.SetBool("Rotating", false);
            }
            else if(!tmp && run)
            {
                myAnimator.SetBool("Rotating", true);
                if (Random.value <= 0.5)
                    rotDir = -1;
                else
                    rotDir = 1;
            }
            run = tmp;
        }
    }

    private void FixedUpdate() //update that has to be used for the rigid body if not the collisions wont work
    {
        if (run)

            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * -1);
            //cellRigidBody.MovePosition(transform.position + Vector3.right * moveSpeed * Time.deltaTime * -1);
        else
        {
            float angle = Random.Range(-180.0f, 180.0f);
            transform.Rotate(0.0f, rotDir * rotSpeed, 0.0f, Space.World);
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

    private void HandleCollision(Collider other)
    {
        if (other.gameObject.name.Substring(0, 4) == "Wall")
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(Mathf.Clamp(pos.x, -14f, 14f), pos.y, Mathf.Clamp(pos.z, -14f, 14f)); //dumb solution plz fix
        }
        
    }
}
