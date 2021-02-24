using System;
using System.IO;
using UnityEngine;

public static class ScenarioLoader
{
    public static GameData Load(String filename)
    {
        GameFiles gameFiles = LoadAsset<GameFiles>(filename, "scenarios");
        GameData gameData = new GameData();
        gameData.days = new DayDataArray[gameFiles.days.Length];
        for (int k = 0; k < gameFiles.days.Length; ++k)
        {
            String[] dayList = gameFiles.days[k].content;
            DayData[] dayDatas = new DayData[dayList.Length];
            for (int i = 0; i < dayList.Length; ++i)
            {
                dayDatas[i] = LoadDayData(dayList[i]);
            }
            gameData.days[k] = new DayDataArray();
            gameData.days[k].content = dayDatas;
        }
        return gameData;
    }

    public static DayData LoadDayData(String filename)
    {
        DayFiles dayFiles = LoadAsset<DayFiles>(filename, "days");
        MorningData[] morningDatas = new MorningData[dayFiles.morning.Length];
        for (int j = 0; j < dayFiles.morning.Length; ++j)
        {
            morningDatas[j] = LoadAsset<MorningData>(dayFiles.morning[j], "mornings");
        }
        MeetingData[] meetingDatas = new MeetingData[dayFiles.meeting.Length];
        for (int j = 0; j < dayFiles.meeting.Length; ++j)
        {
            meetingDatas[j] = LoadAsset<MeetingData>(dayFiles.meeting[j], "meetings");
        }
        InterviewData[] interviewDatas = new InterviewData[dayFiles.interview.Length];
        for (int j = 0; j < dayFiles.interview.Length; ++j)
        {
            interviewDatas[j] = LoadAsset<InterviewData>(dayFiles.interview[j], "interviews");
        }
        DayData dayData = new DayData();
        dayData.morning = morningDatas;
        dayData.meeting = meetingDatas;
        dayData.interview = interviewDatas;
        return dayData;
    }

    public static T LoadAsset<T>(string assetName, string assetFolder = "")
    {
        T assetData;
        try
        {
            Debug.Log("/GameData/" + assetFolder + "/" + assetName + ".json");
            string asset = File.ReadAllText(Application.persistentDataPath + "/GameData/" + assetFolder + "/" + assetName + ".json");
            assetData = JsonUtility.FromJson<T>(asset);
        }
        catch (Exception)
        {
            throw new AssetLoadException(assetName);
        }
        return assetData;
    }
}

public class AssetLoadException : Exception
{
    public string assetName;

    public AssetLoadException(string name)
    {
        assetName = name;
    }
}