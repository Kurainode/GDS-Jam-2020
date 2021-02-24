using System;
using System.IO;
using UnityEngine;

public static class ScenarioSaver
{
    public static void SaveAsset(object assetData, string assetName, string assetFolder = "")
    {
        try
        {
            string assetJson = JsonUtility.ToJson(assetData);
            File.WriteAllText(Application.persistentDataPath + "/GameData/" + assetFolder + "/" + assetName + ".json", assetJson);
        }
        catch (Exception)
        {
            throw new AssetSaveException(assetName);
        }
    }
}

public class AssetSaveException : Exception
{
    public string assetName;

    public AssetSaveException(string name)
    {
        assetName = name;
    }
}