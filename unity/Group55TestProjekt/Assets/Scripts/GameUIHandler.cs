﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;
using Random = UnityEngine.Random;

public class GameUIHandler : MonoBehaviour
{
    private Cell cell;
    private GameObject EColi;
    
    private GameObject[] EColis;

    Ray ray;
    RaycastHit hit;

    

    [SerializeField] private Image countImage;
    [SerializeField] private Image timeImage;
    [SerializeField] private Image environmentImage;

    [SerializeField] private TextMeshProUGUI countTMP;
    [SerializeField] private TextMeshProUGUI timeTMP;
    [SerializeField] private TextMeshProUGUI environmentTMP;
    [SerializeField] private TextMeshProUGUI XTMP;
    [SerializeField] private TextMeshProUGUI ZTMP;
    [SerializeField] private TextMeshProUGUI CTMP;

    [SerializeField] private Canvas cellInfoCanvas;

    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;

    private List<GameObject> EColiList;
    
    

    // Start is called before the first frame update
    void Start()
    {
        timeTMP = GameObject.Find("timeTMP").GetComponent<TextMeshProUGUI>();
        environmentTMP = GameObject.Find("environmentTMP").GetComponent<TextMeshProUGUI>();
        countTMP = GameObject.Find("countTMP").GetComponent<TextMeshProUGUI>();
        EColi = GameObject.FindGameObjectWithTag("Player");
        

        EColiList = new List<GameObject>();
        EColiList.Add(EColi);

        addButton.onClick.AddListener(SpawnEColi);
        removeButton.onClick.AddListener(deleteECoi);




    }

    // Update is called once per frame
    void Update()
    {
        float elpasedTime = Time.timeSinceLevelLoad;
        float minutes = Mathf.Floor(elpasedTime / 60);
        float seconds = elpasedTime % 60;
        
        timeTMP.text = String.Format(minutes + ":" + Mathf.RoundToInt(seconds));
        countTMP.text = EColiList.Count.ToString();
        environmentTMP.text = "Basic";
        




        if (cellInfoCanvas.isActiveAndEnabled)
        {
            XTMP.text = Mathf.Floor(EColi.transform.position.x).ToString();
            ZTMP.text = Mathf.Floor(EColi.transform.position.z).ToString();
            CTMP.text = "Concentration";
        }

        if (EColiList.Count <= 1)
        {
            removeButton.enabled = false;
        }
        else
        {
            removeButton.enabled = true;
        }
    

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
    
}