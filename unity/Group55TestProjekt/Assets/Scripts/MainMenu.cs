﻿using System;
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
    [SerializeField] private TextMeshProUGUI nOfCellsText;
    // Sliders & InputFields

    [SerializeField] private Slider i0Slider;
    [SerializeField] private Slider dSlider;
    [SerializeField] private Slider nOfCellsSlider;
    [SerializeField] private TMP_InputField nOfIterations;
    // Buttons & Toggles

    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button simulateButton;
    [SerializeField] private Toggle forwardSim; 
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
    private int n = 1;
    private int iterations = 100;

    private Model model;

    // Dropdown
    private void Start() {

        // Create basic listeners for various elements
        simulateButton.onClick.AddListener(StartSimulation);
        i0Slider.onValueChanged.AddListener(delegate {EnvValueChanged();});
        dSlider.onValueChanged.AddListener(delegate {EnvValueChanged(); });
        nOfCellsSlider.onValueChanged.AddListener(delegate {CellValueChanged(); });
        nOfIterations.onValueChanged.AddListener(delegate {CellValueChanged(); });
        forwardSim.onValueChanged.AddListener(delegate {ToggleChanged(); });
        quitButton.onClick.AddListener(Quit);

        model = Model.GetInstance();

        BacteriaFactory.SetCellIterations(500);

        //added just to make the program a lot less anoying to use

        createBasicEnv(i0, d);
        EnvValueChanged(); // bug fix for first value change
    }
    private void EnvValueChanged() {
        i0 = i0Slider.value;
        d = dSlider.value;

        i0Text.text = i0.ToString("f3");
        dText.text = d.ToString("f1");
       

        // Used for heatmap visual 
        createBasicEnv(i0, d);
        grid = new Grid(width, height, cellSize);
        Updateheatmap();
        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class   
    }

    private void CellValueChanged() {
        n = (int)nOfCellsSlider.value;
        nOfCellsText.text = n.ToString();
        try {  
            iterations = int.Parse(nOfIterations.text);
            } 
        catch (FormatException e) {
            // Non valid string found, do something!! 
        }    
    }
    private void ToggleChanged() {
        // Ensure that iterations has the correct value when the toggle value is changed
        if (!nOfIterations.isActiveAndEnabled) {
            iterations = 0;
        } else {
            CellValueChanged();
        }
        
    }

    public void StartSimulation() {
        createBasicEnv(i0, d);
        model.SetupCells(n, iterations);
        CellDoneHandler.Setup(n);

        SceneManager.LoadScene(1); // Scene 1 is loading screen
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
}
