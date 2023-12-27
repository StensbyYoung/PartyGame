using System;
using System.IO;

namespace TestGame;

public class DataBase
{
    public static void Write(string text, string file)
    {
        if(File.Exists(file))
        {
            string[] temp = Read(file);
            if(temp.Length >= 5)
            {
                using StreamWriter outputFile = new(file);
                for(int i = 1; i < temp.Length; i++)
                {
                    outputFile.WriteLine(temp[i]);
                }
                outputFile.WriteLine(text);
            }
            else
            {
                using StreamWriter outputFile = new(file, true);
                outputFile.WriteLine(text);
            }
        }
        else
        {
            using StreamWriter outputFile = new(file, true);
            outputFile.WriteLine(text);
        }
    }

    public static string[] Read(string file)
    {
        if(!File.Exists(file))
        {
            return null;
        }
        string[] data = File.ReadAllLines(file);
        return data;
    }
}