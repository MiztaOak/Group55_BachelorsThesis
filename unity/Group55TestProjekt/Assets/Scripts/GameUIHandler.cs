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

public class GameUIHandler : MonoBehaviour
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
    //[SerializeField] private TextMeshProUGUI XTMP;
    //[SerializeField] private TextMeshProUGUI ZTMP;
    //[SerializeField] private TextMeshProUGUI CTMP;
    [SerializeField] private TextMeshProUGUI timeScaleFactorTMP;

    //[SerializeField] private Canvas cellInfoCanvas;
    [SerializeField] private Canvas largeCellInfoCanvas;
    [SerializeField] private Canvas endSimScreen;

    //[SerializeField] private Button addButton;
    //[SerializeField] private Button removeButton;
    [SerializeField] private Button endSimButton;

    private List<GameObject> EColiList;
    private float elpasedTime;

    private float prevTimeScaleFactor;
    
    // Start is called before the first frame update
    void Start()
    {
        timeTMP = GameObject.Find("timeTMP").GetComponent<TextMeshProUGUI>();
        environmentTMP = GameObject.Find("environmentTMP").GetComponent<TextMeshProUGUI>();
        countTMP = GameObject.Find("countTMP").GetComponent<TextMeshProUGUI>();
        EColi = GameObject.FindGameObjectWithTag("Player");


        EColiList = new List<GameObject>();
        EColiList.Add(EColi);

        //addButton.onClick.AddListener(SpawnEColi);
        //removeButton.onClick.AddListener(deleteECoi);
        model = Model.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // EColi = GameObject.FindGameObjectWithTag("Player");
        elpasedTime = elpasedTime + Time.deltaTime * model.GetTimeScaleFactor();
        float minutes = Mathf.Floor(elpasedTime / 60);
        float seconds = elpasedTime % 60;
        
        timeTMP.text = String.Format(minutes + ":" + Mathf.RoundToInt(seconds));
        countTMP.text = EColiList.Count.ToString();
        environmentTMP.text = "Basic";
        /*if (EColi != null) {
            cellInfoCanvas.gameObject.SetActive(true);
            float x_coord = EColi.transform.position.x;
            float z_coord = EColi.transform.position.z;
            XTMP.text = x_coord.ToString();
            ZTMP.text = z_coord.ToString();
            CTMP.text = model.environment.getConcentration(x_coord, z_coord).ToString();
        } else {
            cellInfoCanvas.gameObject.SetActive(false);
        }*/

        if (CellInfo.focusedCell != null)
        {
            largeCellInfoCanvas.gameObject.SetActive(true);
        }

        /*
        if (EColiList.Count <= 1)
        {
            removeButton.enabled = false;
        }
        else
        {
            removeButton.enabled = true;
        }
        */

}

    void SpawnEColi(){
        Vector3 position = new Vector3(Random.Range(-10.0F, 10.0F), 1, Random.Range(-10.0F, 10.0F));
        GameObject newEColi = Instantiate (EColi, position, Quaternion.identity);
        EColiList.Add(newEColi);
    }

    void deleteECoi()
    {
        if (EColiList.Count == 1)
        {
            // do nothing
        }
        else
        {
            GameObject EColiToDelete = EColiList[Random.Range(0, EColiList.Count)];
            EColiList.Remove(EColiToDelete);
            Destroy(EColiToDelete);
        }
        
    }

    public void OnCloseClick()
    {
        CellInfo.focusedCell = null;
        largeCellInfoCanvas.gameObject.SetActive(false);
        // largeCellInfoCanvas.enabled = false;

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
    }

    public void OnCloseEndSimClick() //called when the stat page is closed
    {
        endSimButton.gameObject.SetActive(true);
        endSimScreen.gameObject.SetActive(false);
        model.SetTimeScaleFactor(prevTimeScaleFactor);
    }

    public void OnEndSim() //called when the user confirms that they wish to end the simulation
    {
        SceneManager.LoadScene(0);
    }
}
