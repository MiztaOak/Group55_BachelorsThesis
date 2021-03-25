using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ExportHandler
{
    public static readonly string dirPath = Application.dataPath + "/Exports/";

// checks if the directory exists, and if not create one.
    public static void init()
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    // static method to save data to specific file using JSON.NET
    public static void exportData(List<Model.DataToExport> dataToExport)
    {
        using (StreamWriter file = File.CreateText(dirPath + "exported_data.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file stream
            serializer.Serialize(file, dataToExport);
        }
    }
}