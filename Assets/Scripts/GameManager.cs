using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas canvas;

    public GameObject speechBubble;
    public GameObject textQTE;
    public GameObject question;

    public TextAsset gameData;

    private GameData m_gameData;

    void Start()
    {
        m_gameData = JsonUtility.FromJson<GameData>(gameData.text);
        StartCoroutine("GameCoroutine");
    }
    
    void Update()
    {
        
    }

    public IEnumerator GameCoroutine()
    {
        foreach (Day day in m_gameData.days)
        {
            // TODO: Load morning background
            foreach (TalkData talk in day.beforeInfos)
            {
                yield return null;
            }

            // TODO: Answer phone call
            foreach (TalkData talk in day.morningInfos)
            {
                yield return null;
            }

            // TODO: Hang up phone call
            foreach (TalkData talk in day.afterInfos)
            {
                yield return null;
            }

            // TODO: Transition

            // TODO: Load meeting background
            foreach (TalkData talk in day.meeting.beforeMeeting)
            {
                yield return null;
            }

            foreach (QTEData qte in day.meeting.qtes)
            {
                yield return null;
            }

            foreach (TalkData talk in day.meeting.afterMeeting)
            {
                yield return null;
            }

            // TODO: Transition

            // TODO: Load interview background
            foreach (TalkData talk in day.interview.beforeInterview)
            {
                yield return null;
            }

            foreach (QuestionData question in day.interview.questions)
            {
                yield return null;
            }

            foreach (TalkData talk in day.interview.afterInterview)
            {
                yield return null;
            }
        }
    }
}

[Serializable]
public class GameData
{
    public Day[] days;
}

[Serializable]
public class Day
{
    public TalkData[]  beforeInfos;
    public TalkData[]  morningInfos;
    public TalkData[]  afterInfos;
    public MeetingData meeting;
    public Interview   interview;
}

[Serializable]
public class MeetingData
{
    public TalkData[] beforeMeeting;
    public QTEData[]  qtes;
    public TalkData[] afterMeeting;
}

[Serializable]
public class Interview
{
    public TalkData[]     beforeInterview;
    public QuestionData[] questions;
    public TalkData[]     afterInterview;
}

[Serializable]
public class QuestionData
{
    public TalkData[] beforeQuestion;
    public string     question;
    public string[]   answers;
    public int        correctAnswerIndex;
    public int        time;
    public TalkData[] afterGoodAnswer;
    public TalkData[] afterBadAnswer;
    public QTEData    qte;
}

[Serializable]
public class QTEData
{
    public TalkData[] beforeQTE;
    public int        time;
    public string     qte;
    public TalkData[] afterGoodQTE;
    public TalkData[] afterBadQTE;
}

[Serializable]
public class TalkData
{
    public string speaker;
    public string text;
    public string bubbleType;
}
