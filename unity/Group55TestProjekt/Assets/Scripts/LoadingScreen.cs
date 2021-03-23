using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(Load());
    }

    private void Update()
    {
        //do something cool here
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(3);

        AsyncOperation async = SceneManager.LoadSceneAsync(2);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
