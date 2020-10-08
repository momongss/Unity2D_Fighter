using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;


public class FileManager
{
    // CSV

    string File;

    public FileManager()
    {
        string FilePath = Application.dataPath + "/Resources/data/";
        string FileName = "test.csv";

        File = FilePath + FileName;
    }

    public FileManager(string FileName)
    {
        string FilePath = Application.dataPath + "/Resources/data/";

        File = FilePath + FileName;
    }

    public FileManager(string FilePath, string FileName)
    {
        FilePath = Application.dataPath + FilePath;

        File = FilePath + FileName;
    }

    public List<string[]> ReadData()
    {
        StreamReader Reader = new StreamReader(File);

        List<string[]> data = new List<string[]>();

        while (!Reader.EndOfStream)
            data.Add(Reader.ReadLine().Split(','));

        Reader.Close();

        return data;
    }

    public void WriteData(List<string[]> data)
    {
        StreamWriter Writer = new StreamWriter(File);

        foreach (string[] line in data)
        {
            string LineData = "";
            for (int i = 0; i < line.Length - 1; i++)
                LineData += line[i] + ',';
            LineData += line[line.Length - 1];
                
            Writer.WriteLine(LineData);
        }

        Writer.Close();
    }
}
