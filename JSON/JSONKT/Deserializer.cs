using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class Deserializer : MonoBehaviour
{
    private void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", "DataBase_ParsedToJson.txt");

        try
        {
            string json = File.ReadAllText(filePath);

            DataBase dataBase = JsonConvert.DeserializeObject<DataBase>(json);

            foreach (Data item in dataBase.Data)
            {
                Debug.Log("Name: " + item.Name + ", Description: " + item.Description);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Deserilazion Error: " + exception.Message);
        }
    }
}
