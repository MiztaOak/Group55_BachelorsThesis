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
    [SerializeField] private TextMeshProUGUI sourcesText;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI nOfCellsText;
    // Sliders & InputFields

    [SerializeField] private Slider i0Slider;
    [SerializeField] private Slider dSlider;
    [SerializeField] private Slider nOfCellsSlider;
    [SerializeField] private TMP_InputField nOfIterations;
    [SerializeField] private Slider sourcesSlider;
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
    private int sources = 1;
    private int n = 1;
    private int iterations = 100;
    private bool isDynamic = true;

    private Model model;

    // Dropdown
    private void Start() {

        // Create basic listeners for various elements
        simulateButton.onClick.AddListener(StartSimulation);
        i0Slider.onValueChanged.AddListener(delegate {EnvValueChanged();});
        dSlider.onValueChanged.AddListener(delegate {EnvValueChanged(); });
        sourcesSlider.onValueChanged.AddListener(delegate { EnvValueChanged(); });
        nOfCellsSlider.onValueChanged.AddListener(delegate {CellValueChanged(); });
        nOfIterations.onValueChanged.AddListener(delegate {CellValueChanged(); });
        forwardSim.onValueChanged.AddListener(delegate { CellValueChanged(); });
        quitButton.onClick.AddListener(Quit);

        model = Model.GetInstance();

        BacteriaFactory.SetCellIterations(500);
        BacteriaFactory.SetCellDeathAndDivision(true);
        BacteriaFactory.SetCellRegulatorType(RegulatorType.ODE);

        //added just to make the program a lot less anoying to use

        createBasicEnv(i0, d);
        EnvValueChanged(); // bug fix for first value change
    }

    private void Update() {
        //Debug.Log(iterations);
    }
    private void EnvValueChanged() {
        i0 = i0Slider.value;
        d = dSlider.value;
        sources = Mathf.RoundToInt(sourcesSlider.value);

        i0Text.text = i0.ToString("f3");
        dText.text = d.ToString("f1");
        sourcesText.text = sources.ToString();

        // Used for heatmap visual 
        createBasicEnv(i0, d);
        grid = new Grid(width, height, cellSize);
        Updateheatmap();
        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class   
    }

    private void CellValueChanged() {
        n = (int)nOfCellsSlider.value;
        nOfCellsText.text = n.ToString();
        if (nOfIterations.isActiveAndEnabled) {
            try {  
                iterations = int.Parse(nOfIterations.text);
            } 
            catch (FormatException e) {
                // Non valid string found, do something!! 
            }    
        } else {
            iterations = 0;
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
        EnvironmentFactory.CreateMultiEnvironment(d,i_0,sources,isDynamic);
    }
    
    public void Updateheatmap() {
        grid = new Grid(width, height, cellSize);
        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class     
    }

    public void SetCellDeathDivision(bool status)
    {
        BacteriaFactory.SetCellDeathAndDivision(status);
    }

    public void SetIsDynamic(bool status)
    {
        isDynamic = status;
    }

    public void HandleDropDownSelection(int index)
    {
        switch (index)
        {
            case 0:
                BacteriaFactory.SetCellRegulatorType(RegulatorType.ODE);
                break;
            case 1:
                BacteriaFactory.SetCellRegulatorType(RegulatorType.Hazard);
                break;
        }
    }
}
