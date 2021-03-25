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

    [SerializeField] private TextMeshProUGUI i0Text1;
    [SerializeField] private TextMeshProUGUI i0Text2;
    [SerializeField] private TextMeshProUGUI dText1;
    [SerializeField] private TextMeshProUGUI dText2;
    [SerializeField] private TextMeshProUGUI KText;
    [SerializeField] private TextMeshProUGUI maxtText;

    // Sliders
    
    [SerializeField] private Slider i0Slider1;
    [SerializeField] private Slider dSlider1;
    
    // Buttons
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button simulateButton;

    [SerializeField] private Button applyButton1;
    
    // Canvases
    [SerializeField] private Canvas optionsCanvas;

    
    // C#

    private float i0;
    private float d;
    private float k;
    private float maxT;

    private Model model;
    
    
    
    // Dropdown
    private void Start()
    {
        
       simulateButton.onClick.AddListener(StartSimulation);
       quitButton.onClick.AddListener(Quit);
       model = Model.GetInstance();
       BacteriaFactory.SetCellIterations(500);

        //added just to make the program a lot less anoying to use
        d = 25;
        createBasicEnv(i0, d);
    }

    private void Update()
    {
            i0 = i0Slider1.value;
            d =  dSlider1.value;
            //i0Slider1
            i0Text1.text = i0.ToString("f3");
            //dSlider1
            dText1.text = d.ToString("f1");
            //applyButton1.onClick.AddListener( () => { createBasicEnv(i0,d); });
       
    }


    public  void StartSimulation() {
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
}
