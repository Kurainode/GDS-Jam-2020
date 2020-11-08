using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas canvas;

    public GameObject speechBubble;
    public GameObject telephoneBubble;
    public GameObject thinkingBubble;
    public GameObject textQTE;
    public GameObject question;

    public GameObject background;
    public GameObject president;

    public TextAsset gameData;

    private GameData m_gameData;
    private string[] m_backgrounds = { "Interview", "Morning", "Meeting", "Forum" };
    private string[] m_presidents = { "Neutral", "Anxious", "Monstrous", "Monstrous2", "Telephone" };

    private bool m_hasClicked = false;

    void Start()
    {
        m_gameData = JsonUtility.FromJson<GameData>(gameData.text);
        StartCoroutine("GameCoroutine");
    }
    
    void Update()
    {
        m_hasClicked = false;
        if (Input.GetKeyDown(KeyCode.Mouse0))
            m_hasClicked = true;
    }

    public IEnumerator GameCoroutine()
    {
        foreach (Day day in m_gameData.days)
        {
            SetBackground("Morning");
            SetPresident("Neutral");
            foreach (TalkData talk in day.beforeInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            // TODO: Answer phone call
            foreach (TalkData talk in day.morningInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            // TODO: Hang up phone call
            foreach (TalkData talk in day.afterInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            // TODO: Transition

            SetBackground("Meeting");
            foreach (TalkData talk in day.meeting.beforeMeeting)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            foreach (QTEData qte in day.meeting.qtes)
            {
                yield return null;
            }

            foreach (TalkData talk in day.meeting.afterMeeting)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            // TODO: Transition

            SetBackground("Interview");
            foreach (TalkData talk in day.interview.beforeInterview)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }

            foreach (QuestionData question in day.interview.questions)
            {
                yield return null;
            }

            foreach (TalkData talk in day.interview.afterInterview)
            {
                GameObject bubble = CreateSpeechBubble(talk);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.1f);
                Destroy(bubble);
            }
        }
    }

    public void SetBackground(string name)
    {
        int childCount = background.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (background.transform.GetChild(i).name == name)
                background.transform.GetChild(i).gameObject.SetActive(true);
            else
                background.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetPresident(string name)
    {
        int childCount = president.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (president.transform.GetChild(i).name == name)
                president.transform.GetChild(i).gameObject.SetActive(true);
            else
                president.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public GameObject CreateSpeechBubble(TalkData talk)
    {
        GameObject toInstanciate;
        if (talk.bubbleType == "phone")
            toInstanciate = telephoneBubble;
        else if (talk.bubbleType == "normal")
            toInstanciate = speechBubble;
        else
            toInstanciate = thinkingBubble;

        GameObject bubbleObject = Instantiate(toInstanciate, canvas.transform);
        SpeechBubble bubble = bubbleObject.GetComponent<SpeechBubble>();
        bubble.text = talk.text;

        return bubbleObject;
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
