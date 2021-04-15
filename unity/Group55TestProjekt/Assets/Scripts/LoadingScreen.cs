﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider; 

    private int n;
    // Start is called before the first frame update
    void Start()
    {
        n = Model.GetInstance().GetNumCells(0);
        CellDoneHandler.Setup(n);
        StartCoroutine(Load(n));
    }

    IEnumerator Load(int numOfCells)
    {
        yield return null;
        int iterations = BacteriaFactory.GetIterations();
        Model model = Model.GetInstance();
        ExportHandler.init();

        //Creates the cell objects
        model.CreateCells(numOfCells);

        if(iterations > 0)
        {
            for (int i = 1; i <= iterations; i++) //Simulate the cells one timestep at a time
            {
                float procent = 0;
                model.SimulateTimeStep(i);
                procent = (float)i / iterations * 100;
                progressText.text = procent + "%";
                progressSlider.value = procent;
                yield return null;
            }
            model.GetAverageLigandC();
        }
        else
        {
            progressText.text = "100%"; //if no simulation you are done
            
        }
        
       model.ExportData(numOfCells, BacteriaFactory.GetIterations());

        AsyncOperation async = SceneManager.LoadSceneAsync(2); 

        while (!async.isDone)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));          

            yield return null;
        }
    }
}
