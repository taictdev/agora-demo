using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class CSVUtility
{
    public static string CSVtoJSON(string path)
    {
        var lines = new List<Queue<string>>();
        var contents = File.Exists(path) ? File.ReadAllLines(path) : null;
        if (contents == null)
            return string.Empty;

        foreach (var line in contents)
        {
            lines.Add(new Queue<string>(line.Split(',')));
        }

        var keys = lines.Select(line => line.Dequeue()).ToArray();
        var objResults = new List<Dictionary<string, string>>();
        while (lines.First().Count > 0)
        {
            var objResult = new Dictionary<string, string>();
            var values = lines.Select(line => line.Dequeue()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                objResult.Add(keys[i], values[i]);
            }
            objResults.Add(objResult);
        }

        return JsonConvert.SerializeObject(objResults);
    }

    public static void JSONtoCSV(string json, string path)
    {
        var xmlNode = JsonConvert.DeserializeXmlNode("{records:{record:" + json + "}}");
        var xmldoc = new XmlDocument();
        xmldoc.LoadXml(xmlNode.InnerXml);

        var xmlReader = new XmlNodeReader(xmlNode);
        var dataSet = new DataSet();
        dataSet.ReadXml(xmlReader);

        var dataTable = dataSet.Tables[0];
        var lines = new List<Queue<object>>();
        var columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
        var header = new Queue<object>(columnNames);
        var valueLines = dataTable.AsEnumerable().Select(row => new Queue<object>(row.ItemArray));
        lines.Add(header);
        lines.AddRange(valueLines);

        var contents = new List<string>();
        var attributes = lines.First();
        while (attributes.Count > 0)
        {
            contents.Add(string.Join(",", lines.Select(t => t.Dequeue())));
        }

        File.WriteAllLines(path, contents);
    }

    // Static method to load questions from a CSV file
    //public static List<Question> LoadQuestions(string fileName)
    //{
    //    List<Question> questions = new List<Question>();
    //    TextAsset csvFile = Resources.Load<TextAsset>(fileName);
    //    if (csvFile == null)
    //    {
    //        Debug.LogError("CSV file not found.");
    //        return questions;
    //    }

    //    // Read each line in the CSV file
    //    string[] lines = csvFile.text.Split('\n');
    //    for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header
    //    {
    //        string line = lines[i];
    //        if (string.IsNullOrWhiteSpace(line)) continue;

    //        string[] fields = ParseLine(line);

    //        // Create a new question from the line's fields
    //        Question question = new Question(fields);

    //        questions.Add(question);
    //    }

    //    return questions;
    //}

    private static string[] ParseLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        foreach (char ch in line)
        {
            if (ch == '\"') // Toggle quote flag
            {
                inQuotes = !inQuotes;
            }
            else if (ch == ',' && !inQuotes) // Handle commas only outside of quotes
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += ch;
            }
        }
        fields.Add(currentField); // Add the last field to the list
        return fields.ToArray();
    }
}