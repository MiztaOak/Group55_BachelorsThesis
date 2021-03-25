using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;
    private int iterations = 1000;

    // Start is called before the first frame update
    void Start()
    {
        Model.GetInstance().SetupCells(50, iterations);
        StartCoroutine(Load(50));
    }

    IEnumerator Load(int numOfCells)
    {
        yield return null;

        Model model = Model.GetInstance();
        ExportHandler.init();

        for(int i = 0; i < numOfCells; i++)
        {
            model.SimulateNextCell(i);
            model.ExportData(i,iterations);
            progressText.text = "Loading progress: " + ((float)(i+1)/numOfCells * 100) + "%";
            yield return null;
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(2); 

        while (!async.isDone)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));          

            yield return null;
        }
    }
}
