using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Toggle = UnityEngine.UI.Toggle;
public class MainMenu : MonoBehaviour

{
    // Texts

    [SerializeField] private TextMeshProUGUI i0Text;
    [SerializeField] private TextMeshProUGUI dText;
    [SerializeField] private TextMeshProUGUI tooltipText;
    // Sliders

    [SerializeField] private Slider i0Slider;
    [SerializeField] private Slider dSlider;
    
    // Buttons
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button simulateButton;
    // Canvases
    
    [SerializeField] private Canvas optionsCanvas;

    // Images
    [SerializeField] private RawImage i0Info;
    [SerializeField] private RawImage dInfo;
    [SerializeField] private Texture2D questionMark;

    // ToolTip
    [SerializeField] private GameObject toolTip;
    [SerializeField] private RectTransform backgroundTransform;
    // Heatmap
    private int width = 120;
    private int height = 120;
    private float cellSize = 0.25f;
    private Grid grid;
    [SerializeField] private HeatmapVisual heatmapVisual;

    // C#
    private float i0 = 0.1f;
    private float d = 50;
    private float k;
    private float maxT;

    private Model model;
 

    // Dropdown
    private void Start() {

        Debug.Log(tooltipText.text);
        //ShowToolTip("this is for the helpo");
        simulateButton.onClick.AddListener(StartSimulation);
        i0Slider.onValueChanged.AddListener(delegate {ValueChanged();});
        dSlider.onValueChanged.AddListener(delegate { ValueChanged(); });
        quitButton.onClick.AddListener(Quit);
        model = Model.GetInstance();
        BacteriaFactory.SetCellIterations(500);
        //added just to make the program a lot less anoying to use

        createBasicEnv(i0, d);
        ValueChanged(); // bug fix for first value change
    }

    private void ValueChanged() {
        i0 = i0Slider.value;
        d = dSlider.value;
        //i0Slider1
        i0Text.text = i0.ToString("f3");
        //dSlider1
        dText.text = d.ToString("f1");
        createBasicEnv(i0, d);

        grid = new Grid(width, height, cellSize);
        Updateheatmap();
        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class   
    }

    public void StartSimulation() {
        createBasicEnv(i0, d);
        if (BacteriaFactory.IsForwardSimulation())
            SceneManager.LoadScene("Loading");
        else
            SceneManager.LoadScene("SampleScene");
        
        //better way
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void Quit() {
        Application.Quit();
    }

    void createBasicEnv(float i_0, float d) {
        EnvironmentFactory.CreateBasicEnvionment(d,i_0);
    }
    

    public void Updateheatmap() {
        grid = new Grid(width, height, cellSize);
        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class     
    }

    private void ShowToolTip(string text) {
        toolTip.SetActive(true);

        tooltipText.text = text;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + 8, tooltipText.preferredHeight + 1);
        
        backgroundTransform.sizeDelta = backgroundSize; 
       
    }
    private void HideToolTip() {
        toolTip.SetActive(false);
    }
}
