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
    private Animator myAnimator;

    private int rotDir;

    private Vector3 originalScale;

    
    [SerializeField] GameObject cellInfoCanvas;

    // Start is called before the first frame update
    void Start()
    {
        cell = new Cell();
        myAnimator = GetComponent<Animator>();
        StartCoroutine(UpdateState());
        originalScale = transform.localScale;

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

    private void OnMouseEnter()
    {
        cellInfoCanvas.SetActive(true);
        transform.localScale += new Vector3(0.05F, 0.05F, 0.05F);

    }

    private void OnMouseExit()
    {
        
        cellInfoCanvas.SetActive(false);
        transform.localScale = originalScale;
        

    }
    
}
