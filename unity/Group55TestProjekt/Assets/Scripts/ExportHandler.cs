using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ExportHandler
{
    public static string dirPath = Application.dataPath + "/Exports/";
    static String timeStamp;
    private static int num = 0;

// checks if the directory exists, and if not create one.
    public static void init()
    {
        timeStamp = DateTime.Now.ToString("HH_mm_ss");
        dirPath = CreateDirPath();
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    // static method to save data to specific file using JSON.NET
    public static void exportData(List<object> dataToExport)
    {
        using (StreamWriter file = File.CreateText(dirPath + "Data"+ timeStamp + "_"+ num + ".json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file stream
            serializer.Serialize(file, dataToExport);
        }
        num++;
    }

    private static String CreateDirPath()
    {
        BacteriaFactory b = BacteriaFactory.GetInstance();
        Model m = Model.GetInstance();
        String enviroMode = m.environment.IsDynamic() ? "Dynamic": "Static";
        String death = b.GetLifeRegulator() is LifeRegulator ? "Death" : "Immortal";
        return Application.dataPath + "/Exports/" + b.GetRegulatorType().ToString() + "_" + death + "_" + m.GetNumCells(0) + "_" + 
            enviroMode + "_" + BacteriaFactory.GetIterations() +  "_" + timeStamp + "/";
    }
}