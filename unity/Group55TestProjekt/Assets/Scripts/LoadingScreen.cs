using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider; 

    private int n;
    private static int runs = 1;
    private int iterations;

    private int simulatedStep;


    // Start is called before the first frame update
    void Start()
    {
        n = Model.GetInstance().GetNumCells(0);
        iterations = BacteriaFactory.GetIterations();
        CellDoneHandler.Setup(n);
        StartCoroutine(Load());
        ExportHandler.init();
    }

    IEnumerator Load()
    {
        yield return null;
       
        //Creates the cell objects
        Model.GetInstance().CreateCells(n);
        if (iterations > 0)
        {
            Thread thread = new Thread(Run);
            thread.Start();
            while (thread.IsAlive){
                float procent = (float)(simulatedStep-1) / iterations * 100;
                progressText.text = procent + "%";
                progressSlider.value = procent;
                yield return null;
            }
            
        }
        else
        {
            progressText.text = "100%"; //if no simulation you are done

        }

        AsyncOperation async = SceneManager.LoadSceneAsync(2);

        while (!async.isDone)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b,
                Mathf.PingPong(Time.time, 1));
            
            yield return null;
        }
    }

    public void Run()
    {
        Model model = Model.GetInstance();
        for (int k = 0; k < runs; k++) //simulates m different runs only showing the last one but exporting the data for the rest
        {
            if (k != 0)
            { //reset data if not the first run (since that is already setup elsewhere in the code)
                model.SetupCells(n, iterations);
                model.CreateCells(n);
            }

            for (simulatedStep = 1; simulatedStep <= iterations; simulatedStep++) //Simulate the cells one timestep at a time
            {
                model.SimulateTimeStep(simulatedStep);
            }


            model.ExportData(n, BacteriaFactory.GetIterations());
        }

        model.GetAverageLigandC();
    }

    public static void SetRuns(int runs)
    {
        LoadingScreen.runs = runs > 0 ? runs : 1;
    }
}