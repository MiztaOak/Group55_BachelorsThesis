using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;

    // Start is called before the first frame update
    void Start()
    {
        Model.GetInstance().SetupCells(2, 1000);
        StartCoroutine(Load(2));
    }

    IEnumerator Load(int numOfCells)
    {
        yield return null;

        Model model = Model.GetInstance();

        for(int i = 0; i < numOfCells; i++)
        {
            model.SimulateNextCell(i);
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
