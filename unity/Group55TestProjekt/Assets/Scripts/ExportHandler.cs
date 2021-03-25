using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ExportHandler
{
    public static readonly string dirPath = Application.dataPath + "/Exports/";

    public static void init()
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
            
    }

    public static void exportData(List<Model.DataToExport> dataToExport)
    {
        using (StreamWriter file = File.CreateText(dirPath+ "exported_data.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file stream
            serializer.Serialize(file, dataToExport);
        }
    }
}