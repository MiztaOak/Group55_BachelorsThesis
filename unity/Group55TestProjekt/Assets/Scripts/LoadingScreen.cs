using System.Collections;
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
    private static int runs = 1;

    // Start is called before the first frame update
    void Start()
    {
        n = Model.GetInstance().GetNumCells(0);
        CellDoneHandler.Setup(n);
        StartCoroutine(Load(n));
        ExportHandler.init();
    }

    IEnumerator Load(int numOfCells)
    {
        yield return null;
        int iterations = BacteriaFactory.GetIterations();
        Model model = Model.GetInstance();

        //Creates the cell objects
        model.CreateCells(numOfCells);
        if (iterations > 0)
        {
            for (int k = 0; k < runs; k++) //simulates m different runs only showing the last one but exporting the data for the rest
            {
                if (k != 0)
                { //reset data if not the first run (since that is already setup elsewhere in the code)
                    model.SetupCells(n, iterations);
                    model.CreateCells(numOfCells);
                }

                for (int i = 1; i <= iterations; i++) //Simulate the cells one timestep at a time
                {
                    float procent = 0;
                    model.SimulateTimeStep(i);
                    procent = (float)i / iterations * 100;
                    progressText.text = procent + "%";
                    progressSlider.value = procent;
                    yield return null;
                }


                model.ExportData(numOfCells, BacteriaFactory.GetIterations());

                yield return null;
            }

            model.GetAverageLigandC();
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

    public static void SetRuns(int runs)
    {
        LoadingScreen.runs = runs > 0 ? runs : 1;
    }
}