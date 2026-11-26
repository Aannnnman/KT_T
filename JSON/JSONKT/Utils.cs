using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class Utils
{
    public static void ParveCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data");

        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        List<Data> dataList = new List<Data>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            string name = values[0].Trim();
            string description = values[1].Trim();

            dataList.Add(new Data(name, description));
        }

        DataBase dataBase = new DataBase(dataList.ToArray());

        string filePath = Path.Combine(Application.dataPath, "Resources", "DataBase_ParsedToJson.txt");

        try
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("Data");
                jsonWriter.WriteStartArray();

                foreach (Data item in dataBase.Data)
                {
                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("Name");
                    jsonWriter.WriteValue(item.Name);

                    jsonWriter.WritePropertyName("Description");
                    jsonWriter.WriteValue(item.Description);

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
                jsonWriter.WriteEndObject();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Wrong write JSON: " + exception.Message);
        }
    }
}
