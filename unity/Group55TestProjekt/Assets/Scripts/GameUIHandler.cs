using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour, ICellDoneListener
{
    private Cell cell;
    private GameObject EColi;
    
    private GameObject[] EColis;

    Ray ray;
    RaycastHit hit;

    private Model model;

    [SerializeField] private Image countImage;
    [SerializeField] private Image timeImage;
    [SerializeField] private Image environmentImage;

    [SerializeField] private TextMeshProUGUI countTMP;
    [SerializeField] private TextMeshProUGUI timeTMP;
    [SerializeField] private TextMeshProUGUI environmentTMP;
    [SerializeField] private TextMeshProUGUI timeScaleFactorTMP;

    [SerializeField] private Canvas largeCellInfoCanvas;
    [SerializeField] private Canvas endSimScreen;
    [SerializeField] private Canvas pauseScreen;

    [SerializeField] private Button endSimButton;
    [SerializeField] private TextMeshProUGUI numOfCells;
    [SerializeField] private TextMeshProUGUI numOfIterations;
    [SerializeField] private TextMeshProUGUI timeElapsed;

    private List<GameObject> EColiList;
    private float elpasedTime;

    private float prevTimeScaleFactor = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        timeTMP = GameObject.Find("timeTMP").GetComponent<TextMeshProUGUI>();
        environmentTMP = GameObject.Find("environmentTMP").GetComponent<TextMeshProUGUI>();
        countTMP = GameObject.Find("countTMP").GetComponent<TextMeshProUGUI>();

        model = Model.GetInstance();

        CellDoneHandler.AddListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        elpasedTime = elpasedTime + Time.deltaTime * model.GetTimeScaleFactor();
        
        
        timeTMP.text = FormatTimeString();
        countTMP.text = model.GetCells().Count.ToString();
        environmentTMP.text = "Basic";


        if (Input.GetKeyDown(KeyCode.Escape) && !pauseScreen.gameObject.activeSelf) {
            pauseScreen.gameObject.SetActive(true);
            prevTimeScaleFactor = model.GetTimeScaleFactor();
            model.SetTimeScaleFactor(0);
        } else if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen.gameObject.activeSelf) {
            OnResumeClick();
        }
           
        if (CellInfo.focusedCell != null)
        {
            largeCellInfoCanvas.gameObject.SetActive(true);
        }
    }

    private String FormatTimeString()
    {
        float minutes = Mathf.Floor(elpasedTime / 60);
        float seconds = elpasedTime % 60;
        return String.Format(minutes + ":" + Mathf.RoundToInt(seconds));
    }
    public void OnResumeClick() {
        pauseScreen.gameObject.SetActive(false);
        model.SetTimeScaleFactor(prevTimeScaleFactor);
    }
    public void OnCloseClick()
    {
        CellInfo.focusedCell = null;
        largeCellInfoCanvas.gameObject.SetActive(false);
    }
    public void OnTimeScaleChanged(Slider slider)
    {
        model.SetTimeScaleFactor(slider.value);
        timeScaleFactorTMP.SetText(slider.value.ToString());
    }

    public void OnEndSimClick() //called when the user clicks the end sim button to open the stat page
    {
        endSimButton.gameObject.SetActive(false);
        endSimScreen.gameObject.SetActive(true);
        prevTimeScaleFactor = model.GetTimeScaleFactor();
        model.SetTimeScaleFactor(0);

        //Update the texts for the end screen
        numOfCells.text = "Number of Bacteria: " + model.GetCells().Count;
        numOfIterations.text = "Number of Iterations: " + BacteriaFactory.GetIterations();
        timeElapsed.text = "Time elapsed: " + FormatTimeString();
    }

    public void OnCloseEndSimClick() //called when the stat page is closed
    {
        endSimButton.gameObject.SetActive(true);
        endSimScreen.gameObject.SetActive(false);
        model.SetTimeScaleFactor(prevTimeScaleFactor);
    }

    public void OnEndSim() //called when the user confirms that they wish to end the simulation
    {
        model.SetTimeScaleFactor(1);
        SceneManager.LoadScene(0);
    }

    public void OnCellDone()
    {
        OnEndSimClick();
    }
}
