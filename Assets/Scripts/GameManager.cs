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
    public GameObject humanBubble;
    public GameObject textQTE;
    public GameObject question;

    public GameObject background;
    public GameObject president;
    public GameObject closePresident;
    public GameObject music;
    public GameObject gameOver;

    public Transform interviewBubbleArea;
    public Transform meetingBubbleArea;
    public Transform humanBubbleArea;

    public Text score;

    public TextAsset gameData;

    private GameData m_gameData;
    private string[] m_backgrounds = { "Interview", "Morning", "Meeting", "Forum" };
    private string[] m_presidents = { "Neutral", "Anxious", "Monstrous", "Monstrous2", "Telephone" };

    private bool m_hasClicked = false;
    private bool m_closePresident = false;

    private bool m_locked = false;
    private bool m_lastResult = false;

    private int m_currentScore = 100;
    private int m_totalScore = 0;
    private string m_currentMusic = "";

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
        score.text = m_currentScore.ToString();
    }

    public IEnumerator GameCoroutine()
    {
        foreach (Day day in m_gameData.days)
        {
            m_currentScore = 100;

            SetBackground("Morning");
            m_closePresident = false;
            SetPresident("Neutral");
            SetMusic("Morning");
            foreach (TalkData talk in day.beforeInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }


            SetPresident("Telephone");
            foreach (TalkData talk in day.morningInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            SetPresident("Neutral");
            foreach (TalkData talk in day.afterInfos)
            {
                GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            // TODO: Transition
            AutoPresident();
            SetBackground("Meeting");
            SetMusic("Meeting");
            foreach (TalkData talk in day.meeting.beforeMeeting)
            {
                GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            foreach (QTEData qte in day.meeting.qtes)
            {
                foreach (TalkData talk in qte.beforeQTE)
                {
                    GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                    yield return new WaitUntil(() => m_hasClicked);
                    yield return new WaitForSeconds(0.2f);
                    Destroy(bubble);
                }
                m_locked = true;
                CreateQTE(qte);
                yield return new WaitUntil(() => !m_locked);
                AutoPresident();
                TalkData[] aterQTE = m_lastResult ? qte.afterGoodQTE : qte.afterBadQTE;
                foreach (TalkData talk in aterQTE)
                {
                    GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                    yield return new WaitUntil(() => m_hasClicked);
                    yield return new WaitForSeconds(0.2f);
                    Destroy(bubble);
                }
            }

            foreach (TalkData talk in day.meeting.afterMeeting)
            {
                GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            // TODO: Transition

            m_closePresident = true;
            AutoPresident();
            SetBackground("Interview");
            SetMusic("Interview");
            foreach (TalkData talk in day.interview.beforeInterview)
            {
                GameObject bubble = CreateSpeechBubble(talk, interviewBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            foreach (QuestionData question in day.interview.questions)
            {
                foreach (TalkData talk in question.beforeQuestion)
                {
                    GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                    yield return new WaitUntil(() => m_hasClicked);
                    yield return new WaitForSeconds(0.2f);
                    Destroy(bubble);
                }
                m_locked = true;
                CreateQuestion(question);
                yield return new WaitUntil(() => !m_locked);
                AutoPresident();
                if (!m_lastResult)
                {
                    m_locked = true;
                    CreateQTE(question.qte);
                    yield return new WaitUntil(() => !m_locked);
                    AutoPresident();
                }
                TalkData[] afterQuestion = m_lastResult ? question.afterGoodAnswer : question.afterBadAnswer;
                foreach (TalkData talk in afterQuestion)
                {
                    GameObject bubble = CreateSpeechBubble(talk, meetingBubbleArea);
                    yield return new WaitUntil(() => m_hasClicked);
                    yield return new WaitForSeconds(0.2f);
                    Destroy(bubble);
                }
            }

            foreach (TalkData talk in day.interview.afterInterview)
            {
                GameObject bubble = CreateSpeechBubble(talk, interviewBubbleArea);
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
                Destroy(bubble);
            }

            m_totalScore += m_currentScore;
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
            if (president.transform.GetChild(i).name == name && !m_closePresident)
                president.transform.GetChild(i).gameObject.SetActive(true);
            else
                president.transform.GetChild(i).gameObject.SetActive(false);
        }
        childCount = closePresident.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (closePresident.transform.GetChild(i).name == name && m_closePresident)
                closePresident.transform.GetChild(i).gameObject.SetActive(true);
            else
                closePresident.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetMusic(string name)
    {
        int childCount = music.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (music.transform.GetChild(i).name == name)
                music.transform.GetChild(i).gameObject.SetActive(true);
            else
                music.transform.GetChild(i).gameObject.SetActive(false);
        }
        m_currentMusic = name;
    }

    public void AutoPresident()
    {
        if (m_currentScore > 75)
            SetPresident("Neutral");
        else if (m_currentScore > 50)
            SetPresident("Anxious");
        else if (m_currentScore > 25)
            SetPresident("Monstrous");
        else
            SetPresident("Monstrous2");
    }

    public GameObject CreateSpeechBubble(TalkData talk, Transform master)
    {
        GameObject toInstanciate;
        if (talk.bubbleType == "phone")
            toInstanciate = telephoneBubble;
        else if (talk.bubbleType == "normal")
            toInstanciate = speechBubble;
        else if (talk.bubbleType == "thinking")
            toInstanciate = thinkingBubble;
        else
        {
            master = humanBubbleArea;
            toInstanciate = humanBubble;
        }

        GameObject bubbleObject = Instantiate(toInstanciate, master);
        SpeechBubble bubble = bubbleObject.GetComponent<SpeechBubble>();
        bubble.text = talk.text;

        return bubbleObject;
    }

    public void CreateQTE(QTEData data)
    {
        GameObject qteObject = Instantiate(textQTE, canvas.transform);
        WordQTE qte = qteObject.GetComponent<WordQTE>();
        qte.text = data.qte;
        qte.duration = data.time;
        qte.onEnded.AddListener(OnQTEEnded);
    }

    public void OnQTEEnded(bool succeed)
    {
        if (!succeed)
        {
            m_currentScore -= 25;
        }
        if (m_currentScore <= 0)
            GameOver();
        else
        {
            m_lastResult = succeed;
            m_locked = false;
        }
    }

    public void CreateQuestion(QuestionData data)
    {
        GameObject questionObject = Instantiate(question, canvas.transform);
        Question newQuestion = questionObject.GetComponent<Question>();
        newQuestion.questionText = data.question;
        newQuestion.answersText = data.answers;
        newQuestion.duration = data.time;
        newQuestion.answeredQuestion.AddListener(OnQuestionEnded);
    }

    public void OnQuestionEnded(bool succeed)
    {
        if (succeed)
        {
            m_currentScore += 25;
        }
        m_lastResult = succeed;
        m_locked = false;
    }

    public void GameOver()
    {
        if (m_currentMusic == "Meeting")
            SetMusic("GameOverMeeting");
        else
            SetMusic("GameOverInterview");
        gameOver.SetActive(true);
    }

    public void PlaySound(string sound)
    {

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
