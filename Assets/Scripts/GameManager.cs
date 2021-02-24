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
    public GameObject sound;
    public GameObject gameOver;

    public Transform interviewBubbleArea;
    public Transform meetingBubbleArea;
    public Transform humanBubbleArea;

    public PauseMenu pauseMenu;

    public Text score;
    public Text totalScore;

    public String gameData;

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
        m_gameData = ScenarioLoader.Load(gameData);
        Debug.Log(JsonUtility.ToJson(m_gameData));
        StartCoroutine("GameCoroutine");
    }

    void Update()
    {
        m_hasClicked = false;
        if (Input.GetKeyDown(KeyCode.Mouse0))
            m_hasClicked = true;
        score.text = m_currentScore.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
            pauseMenu.Pause();
    }

    public IEnumerator GameCoroutine()
    {
        foreach (DayDataArray dayPossibilities in m_gameData.days)
        {
            int dayIndex = UnityEngine.Random.Range(0, dayPossibilities.content.Length);
            DayData day = dayPossibilities.content[dayIndex];
            m_currentScore = 100;

            int morningIndex = UnityEngine.Random.Range(0, day.morning.Length);
            MorningData morning = day.morning[morningIndex];

            SetBackground("Morning");
            m_closePresident = false;
            SetPresident("Neutral");
            SetMusic("Morning");

            PlaySound("PhoneRing");
            yield return new WaitUntil(() => m_hasClicked);
            yield return new WaitForSeconds(0.2f);
            SetPresident("Telephone");
            yield return SpeechHandler(morning.infos, meetingBubbleArea);
            

            SetPresident("Neutral");

            int meetingIndex = UnityEngine.Random.Range(0, day.meeting.Length);
            MeetingData meeting = day.meeting[meetingIndex];

            // TODO: Transition
            AutoPresident();
            SetBackground("Meeting");
            SetMusic("Meeting");
            yield return SpeechHandler(meeting.beforeMeeting, meetingBubbleArea);

            foreach (QTEData qte in meeting.qtes)
            {
                yield return SpeechHandler(qte.beforeQTE, meetingBubbleArea);
                m_locked = true;
                CreateQTE(qte);
                yield return new WaitUntil(() => !m_locked);
                AutoPresident();
                TalkData[] aterQTE = m_lastResult ? qte.afterGoodQTE : qte.afterBadQTE;
                yield return SpeechHandler(aterQTE, meetingBubbleArea);
            }
            yield return SpeechHandler(meeting.afterMeeting, meetingBubbleArea);

            int interviewIndex = UnityEngine.Random.Range(0, day.interview.Length);
            InterviewData interview = day.interview[interviewIndex];

            // TODO: Transition
            m_closePresident = true;
            AutoPresident();
            SetBackground("Interview");
            SetMusic("Interview");
            yield return SpeechHandler(interview.beforeInterview, interviewBubbleArea);
            
            foreach (QuestionData question in interview.questions)
            {
                yield return SpeechHandler(question.beforeQuestion, interviewBubbleArea);
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
                yield return SpeechHandler(afterQuestion, interviewBubbleArea);
            }
            yield return SpeechHandler(interview.afterInterview, interviewBubbleArea);

            m_totalScore += m_currentScore;
            totalScore.text = m_totalScore.ToString();
        }
        PickEnding();
    }

    public void PickEnding()
    {
        if (m_totalScore >= 400)
        {
            //Perfect Ending
        }
        else if (m_totalScore >= 300)
        {
            //Good Ending
        }
        else
        {
            //Bad Ending
        }
    }

    public IEnumerator SpeechHandler(TalkData[] talkData, Transform bubbleArea)
    {
        foreach (TalkData talk in talkData)
        {
            GameObject bubble = CreateSpeechBubble(talk, bubbleArea);
            yield return new WaitUntil(() => m_hasClicked);
            yield return new WaitForSeconds(0.2f);
            Destroy(bubble);
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

    public void PlaySound(string name)
    {
        int childCount = sound.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (sound.transform.GetChild(i).name == name)
            {
                sound.transform.GetChild(i).gameObject.SetActive(true);
                sound.transform.GetChild(i).GetComponent<AudioSource>().Play();
            }
        }
    }
}
