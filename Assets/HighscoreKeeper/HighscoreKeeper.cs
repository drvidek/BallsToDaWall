using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreKeeper
{
    private static List<KeyValuePair<string, float>> _entries = new();
    /// <summary>
    /// A sorted list of all highscore entries, where Key is the name and Value is the score
    /// </summary>
    public static List<KeyValuePair<string, float>> Entries => _entries;
    private static int _maxEntriesToKeep = 10;
    private static string _savePath = Path.Combine(Application.streamingAssetsPath, "ToolkitResources/Highscores/Save.txt");
    private static string _defaultPath = Path.Combine(Application.streamingAssetsPath, "ToolkitResources/Highscores/Default.txt");
    /// <summary>
    /// A string of \n separated names in order from high to low
    /// </summary>
    public static string NameList
    {
        get
        {
            string names = "";
            for (int i = 0; i < _entries.Count; i++)
            {
                names += _entries[i].Key;
                if (i + 1 != _entries.Count)
                    names += "\n";
            }
            return names;
        }
    }
    /// <summary>
    /// A string of \n separated scores in order from high to low
    /// </summary>
    public static string ScoreList
    {
        get
        {
            string scores = "";
            for (int i = 0; i < _entries.Count; i++)
            {
                scores += _entries[i].Value;
                if (i + 1 != _entries.Count)
                    scores += "\n";
            }
            return scores;
        }
    }

    /// <summary>
    /// Attempt to add a new entry to the table with Key name and Value score
    /// </summary>
    /// <param name="name"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    public static bool ValidateNewEntry(string name, float score)
    {
        KeyValuePair<string, float> entry = new(name, score);
        _entries.Add(entry);
        SortEntries();

        if (_entries.Count <= _maxEntriesToKeep)
            return true;

        _entries.RemoveAt(_entries.Count - 1);
        return (_entries.Contains(entry));
    }

    /// <summary>
    /// Load the saved high score entries from file into memory
    /// </summary>
    public static void LoadEntries()
    {
        _entries.Clear();
        string path = File.Exists(_savePath) ? _savePath : _defaultPath;
        if (Application.platform != RuntimePlatform.Android)
        {
            UnpackEntriesFromStringArray(LoadStringArrayFromFile(path));
        }
        else
        {

        }
        SortEntries();
    }

    /// <summary>
    /// Save the current list of high scores to file
    /// </summary>
    public static void SaveEntries()
    {
        SortEntries();
        SaveStringArrayToFile(PackEntriesToStringArray(), _savePath);
    }

    /// <summary>
    /// Resets the saved high score file to default - this will permanently delete existing high score data
    /// </summary>
    public static void ResetEntriesToDefault()
    {
        LoadDefaultEntries();
        SaveEntries();
    }

    private static void LoadDefaultEntries()
    {
        _entries.Clear();
        UnpackEntriesFromStringArray(LoadStringArrayFromFile(_defaultPath));
        SortEntries();
    }

    private static string[] LoadStringArrayFromFile(string path)
    {
        StreamReader reader = new(path);
        string[] array = new string[_maxEntriesToKeep];
        string line;
        int i = 0;
        while ((line = reader.ReadLine()) != null && line != "")
        {
            array[i] = line;
            i++;
        }
        reader.Close();

        return array;
    }

    private static void SaveStringArrayToFile(string[] array, string path)
    {
        StreamWriter reader = new(path, false);
        foreach (var item in array)
        {
            string line = item;
            reader.WriteLine(line);
        }
        reader.Close();
    }

    private static string[] PackEntriesToStringArray()
    {
        string[] array = new string[_maxEntriesToKeep];
        for (int i = 0; i < _maxEntriesToKeep && i < _entries.Count; i++)
        {
            array[i] = $"{_entries[i].Key}:{_entries[i].Value}";
        }
        return array;
    }

    private static void UnpackEntriesFromStringArray(string[] saveStrings)
    {
        foreach (string save in saveStrings)
        {
            Debug.Log(save);
            if (save == null)
                continue;

            string[] splitData = save.Split(':');
            KeyValuePair<string, float> pair = new(splitData[0], float.Parse(splitData[1]));
            _entries.Add(pair);
        }
    }

    private static void SortEntries()
    {
        _entries.Sort((x, y) => y.Value.CompareTo(x.Value));
    }

}
