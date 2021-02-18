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

    private int rotDir;

    // Start is called before the first frame update
    void Start()
    {
        cell = new Cell();
        myAnimator = GetComponent<Animator>();
        StartCoroutine(UpdateState());
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(0);
        }

        if (run)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime*-1);
        else
        {
            float angle = Random.Range(-180.0f, 180.0f);
            transform.Rotate(0.0f, rotDir*rotSpeed, 0.0f, Space.World);
        }
    }

    IEnumerator UpdateState()
    {
        while (true)
        {
            Vector3 pos = transform.position;
            yield return new WaitForSeconds(1.0f);
            cell.SetPos(pos.x, pos.z);
            yield return new WaitForSeconds(0.1f);
            bool tmp = cell.IsRun();
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

}
