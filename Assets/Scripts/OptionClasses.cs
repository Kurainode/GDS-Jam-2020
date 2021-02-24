using System;

/**
 * The data representation of a save.
 * scenario: scenario file used
 * state: current position in GameData (exemple: "root/1/content/0/morning/infos/2")
 * name: The player's name
 * score: current day score
 * totalScore: the total score
 */
[Serializable]
public class SaveFile
{
    public string scenario;
    public string state;
    public string name;
    public int score;
    public int totalScore;
}

/**
 * The data representation of the options file.
 * soundVolume: the global sound volume
 * resolutionIndex: the index of the picked resolution in the resolution array (exemple: 0 -> 1920*1080)
 * textSpeed: the display speed of bubble texts
 */
[Serializable]
public class OptionsFile
{
    public float soundVolume;
    public int resolutionIndex;
    public float textSpeed;
}