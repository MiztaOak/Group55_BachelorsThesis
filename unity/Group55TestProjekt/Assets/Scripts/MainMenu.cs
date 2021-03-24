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
    [SerializeField] private Slider i0Slider2;
    [SerializeField] private Slider dSlider1;
    [SerializeField] private Slider dSlider2;
    [SerializeField] private Slider KSlider;
    [SerializeField] private Slider maxtSlider;
    
    // Buttons
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button applyButton1;
    [SerializeField] private Button applyButton2;
    
    // Toggles

    [SerializeField] private Toggle basicToggle;
    [SerializeField] private Toggle timeDepToggle;
    
    // Canvases

    [SerializeField] private Canvas basicEnvCanvas;
    [SerializeField] private Canvas timeDepEnvCanvas;
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
        
       startButton.onClick.AddListener(StartSimulation);
       quitButton.onClick.AddListener(Quit);
       model = Model.GetInstance();
       BacteriaFactory.SetCellIterations(500);

        //added just to make the program a lot less anoying to use
        d = 25;
        createBasicEnv(i0, d);
    }

    private void Update()
    {


        if (optionsCanvas.isActiveAndEnabled)
        {
            basicToggle.onValueChanged.AddListener((value) => { initBasicEnv(); });
            timeDepToggle.onValueChanged.AddListener ( (value) => {initTimeDepEnv();});
            
        }
        if (basicEnvCanvas.isActiveAndEnabled)
        {
            i0 = i0Slider1.value;
            d =  dSlider1.value;
                //i0Slider1
            i0Text1.text = i0.ToString("f3");
            //dSlider1
            dText1.text = d.ToString("f1");
            applyButton1.onClick.AddListener( () => { createBasicEnv(i0,d); });
        }

        if (timeDepEnvCanvas.isActiveAndEnabled)
        {
            i0 = i0Slider2.value;
            d =  dSlider2.value;
            k = KSlider.value;
            maxT =  maxtSlider.value;
            //i0Slider2
            i0Text2.text = i0.ToString("f3");
            //dSlider2
            dText2.text = d.ToString("f1");
            //kSlider
            KText.text = k.ToString("f3");
            //tmaxSlider
            maxtText.text = maxT.ToString("f3");
            applyButton2.onClick.AddListener( () => { createTimeDepEnv(i0,d,maxT,k); });


        }
    }


    public  void StartSimulation()
    {
        if(BacteriaFactory.IsForwardSimulation())
            SceneManager.LoadScene("Loading");
        else
            SceneManager.LoadScene("SampleScene");
        
        //better way
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void Quit()
    {

        Debug.Log("DONE");
        Application.Quit();
    }



    private void initBasicEnv()
    {
        basicEnvCanvas.gameObject.SetActive(true);
        timeDepEnvCanvas.gameObject.SetActive(false);

    }
    private void initTimeDepEnv()
    {
        basicEnvCanvas.gameObject.SetActive(false);
        timeDepEnvCanvas.gameObject.SetActive(true);

    }

    void createBasicEnv(float i_0, float d)
    {
        EnvironmentFactory.CreateBasicEnvionment(d,i_0);
    }
    void createTimeDepEnv(float i_0, float d,float max_t,float k)
    {
        EnvironmentFactory.CreateTimeDependentEnvionment(d,i_0,max_t,k);
    }

}
