using System;

[Serializable]
public class GameFiles
{
    public StringArray[] days = new StringArray[0];
}

[Serializable]
public class StringArray
{
    public String[] content = new String[0];
}

[Serializable]
public class GameData
{
    public DayDataArray[] days = new DayDataArray[0];
}

[Serializable]
public class DayDataArray
{
    public DayData[] content = new DayData[0];
}

[Serializable]
public class DayFiles
{
    public String[] morning = new String[0];
    public String[] meeting = new String[0];
    public String[] interview = new String[0];
}

[Serializable]
public class DayData
{
    public MorningData[] morning = new MorningData[0];
    public MeetingData[] meeting = new MeetingData[0];
    public InterviewData[] interview = new InterviewData[0];
}

[Serializable]
public class MorningData
{
    public TalkData[] infos = new TalkData[0];
}

[Serializable]
public class MeetingData
{
    public TalkData[] beforeMeeting = new TalkData[0];
    public QTEData[] qtes = new QTEData[0];
    public TalkData[] afterMeeting = new TalkData[0];
}

[Serializable]
public class InterviewData
{
    public TalkData[] beforeInterview = new TalkData[0];
    public QuestionData[] questions = new QuestionData[0];
    public TalkData[] afterInterview = new TalkData[0];
}

[Serializable]
public class QuestionData
{
    public TalkData[] beforeQuestion = new TalkData[0];
    public string question = "";
    public string[] answers = new string[4];
    public int correctAnswerIndex = 0;
    public int time = 0;
    public TalkData[] afterGoodAnswer = new TalkData[0];
    public TalkData[] afterBadAnswer = new TalkData[0];
    public QTEData qte = new QTEData();
}

[Serializable]
public class QTEData
{
    public TalkData[] beforeQTE = new TalkData[0];
    public int time = 0;
    public string qte = "";
    public TalkData[] afterGoodQTE = new TalkData[0];
    public TalkData[] afterBadQTE = new TalkData[0];
}

[Serializable]
public class TalkData
{
    public Action[] preActions = new Action[0];
    public string speaker = "";
    public string text = "";
    public string bubbleType = "";
    public Action[] postActions = new Action[0];
}

[Serializable]
public class Action
{
    public string character = "";
    public string action = "";
}
