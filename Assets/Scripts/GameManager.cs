using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string playerName = "test";

    public Canvas canvas;

    public GameObject speechBubble;
    public GameObject telephoneBubble;
    public GameObject thinkingBubble;
    public GameObject humanBubble;
    public GameObject textQTE;
    public GameObject question;

    public GameObject background;
    public GameObject gameOver;

    public Transform interviewBubbleArea;
    public Transform meetingBubbleArea;
    public Transform humanBubbleArea;

    public Transform bestEnding;
    public Transform goodEnding;
    public Transform badEnding;

    public PauseMenu pauseMenu;

    public Text score;
    public Text totalScore;

    public String gameData;
    public float textSpeed = 20.0f;

    public List<CharacterInfo> characters;
    private Dictionary<string, Character> m_characters = new Dictionary<string, Character>();

    private GameData m_gameData;
    private string[] m_backgrounds = { "Interview", "Morning", "Meeting", "Forum" };
    private string[] m_presidents = { "Neutral", "Anxious", "Monstrous", "Monstrous2", "Telephone" };

    private bool m_hasClicked = false;

    private bool m_locked = false;
    private bool m_lastResult = false;

    private int m_currentScore = 100;
    private int m_totalScore = 0;

    private string m_gameState = "Morning";
    private Stack<string> m_gameStack = new Stack<string>();
    private Queue<string> m_loadQueue = new Queue<string>();

    void Start()
    {
        foreach (CharacterInfo character in characters)
        {
            m_characters.Add(character.name, character.character);
        }
        GameLauncher gl = FindObjectOfType<GameLauncher>();
        if (gl != null)
        {
            if (gl.saveFile != "" && gl.saveFile != null)
                Load(gl.saveFile);
            else
            {
                playerName = gl.playerName;
                if (gl.scenario != "" && gl.scenario != null)
                    gameData = gl.scenario;
                textSpeed = gl.options.textSpeed;
                SoundManager.instance.masterVolume = gl.options.soundVolume;
                Load();
            }
            Destroy(gl.gameObject);
        }
        else
            Load();
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
        int index = 0;
        if (m_loadQueue.Count > 0)
            index = int.Parse(m_loadQueue.Dequeue());

        for (; index < m_gameData.days.Length; ++index)
        {
            m_gameStack.Push(index.ToString());
            
            DayDataArray dayPossibilities = m_gameData.days[index];
            int dayIndex = UnityEngine.Random.Range(0, dayPossibilities.content.Length);
            if (m_loadQueue.Count > 0)
                dayIndex = int.Parse(m_loadQueue.Dequeue());

            DayData day = dayPossibilities.content[dayIndex];
            if (m_loadQueue.Count == 0)
                m_currentScore = 100;
            m_gameStack.Push(dayIndex.ToString());

            if (m_loadQueue.Count > 0)
            {
                string position = m_loadQueue.Dequeue();
                switch (position)
                {
                    case "meeting":
                        goto Meeting;
                    case "interview":
                        goto Interview;
                    default:
                        break;
                }
            }

            int morningIndex = UnityEngine.Random.Range(0, day.morning.Length);
            if (m_loadQueue.Count > 0)
                morningIndex = int.Parse(m_loadQueue.Dequeue());

            MorningData morning = day.morning[morningIndex];
            m_gameStack.Push("morning");
            m_gameStack.Push(morningIndex.ToString());

            SetState("Morning");

            yield return SpeechHandler(morning.infos, meetingBubbleArea);

            m_characters["President"].Do("Neutral");

            m_gameStack.Pop();
            m_gameStack.Pop();

        Meeting:
            int meetingIndex = UnityEngine.Random.Range(0, day.meeting.Length);
            if (m_loadQueue.Count > 0)
                meetingIndex = int.Parse(m_loadQueue.Dequeue());

            MeetingData meeting = day.meeting[meetingIndex];
            m_gameStack.Push("meeting");
            m_gameStack.Push(meetingIndex.ToString());

            // TODO: Transition

            SetState("Meeting");

            if (m_loadQueue.Count > 0)
            {
                string position = m_loadQueue.Dequeue();
                switch (position)
                {
                    case "qtes":
                        goto Qtes;
                    case "afterMeeting":
                        goto AfterMeeting;
                    default:
                        break;
                }
            }

            m_gameStack.Push("beforeMeeting");
            SoundManager.PlaySound("PeopleTalk");
            yield return SpeechHandler(meeting.beforeMeeting, meetingBubbleArea);
            SoundManager.StopSound("PeopleTalk");
            m_gameStack.Pop();

        Qtes:
            m_gameStack.Push("qtes");

            int i = 0;
            if (m_loadQueue.Count > 0)
                i = int.Parse(m_loadQueue.Dequeue());
            for (; i < meeting.qtes.Length; ++i)
            {
                m_gameStack.Push(i.ToString());

                QTEData qte = meeting.qtes[i];
                SoundManager.StopSound("PeopleClap");
                SoundManager.StopSound("PeopleBoo");

                if (m_loadQueue.Count > 0)
                {
                    string position = m_loadQueue.Dequeue();
                    switch (position)
                    {
                        case "qte":
                            goto QteQte;
                        case "afterGoodQTE":
                            m_lastResult = true;
                            goto QteAfterQte;
                        case "afterBadQTE":
                            m_lastResult = false;
                            goto QteAfterQte;
                        default:
                            break;
                    }
                }

                m_gameStack.Push("beforeQTE");
                yield return SpeechHandler(qte.beforeQTE, meetingBubbleArea);
                m_gameStack.Pop();

            QteQte:
                m_gameStack.Push("qte");
                m_locked = true;
                CreateQTE(qte);
                yield return new WaitUntil(() => !m_locked);
                m_gameStack.Pop();

                AutoPresident();

            QteAfterQte:
                TalkData[] aterQTE = m_lastResult ? qte.afterGoodQTE : qte.afterBadQTE;
                if (m_lastResult)
                    SoundManager.PlaySound("PeopleClap");
                else
                    SoundManager.PlaySound("PeopleBoo");

                m_gameStack.Push(m_lastResult ? "afterGoodQTE" : "afterBadQTE");
                yield return SpeechHandler(aterQTE, meetingBubbleArea);
                m_gameStack.Pop();

                m_gameStack.Pop();
            }
            SoundManager.StopSound("PeopleClap");
            SoundManager.StopSound("PeopleBoo");
            m_gameStack.Pop();

        AfterMeeting:
            m_gameStack.Push("afterMeeting");
            SoundManager.PlaySound("PeopleCheer");
            yield return SpeechHandler(meeting.afterMeeting, meetingBubbleArea);
            SoundManager.StopSound("PeopleCheer");
            m_gameStack.Pop();

            m_gameStack.Pop();
            m_gameStack.Pop();

        Interview:
            int interviewIndex = UnityEngine.Random.Range(0, day.interview.Length);
            if (m_loadQueue.Count > 0)
                interviewIndex = int.Parse(m_loadQueue.Dequeue());

            InterviewData interview = day.interview[interviewIndex];
            m_gameStack.Push("interview");
            m_gameStack.Push(interviewIndex.ToString());

            // TODO: Transition

            SetState("Interview");

            if (m_loadQueue.Count > 0)
            {
                string position = m_loadQueue.Dequeue();
                switch (position)
                {
                    case "questions":
                        goto Questions;
                    case "afterInterview":
                        goto AfterInterview;
                    default:
                        break;
                }
            }

            m_gameStack.Push("beforeInterview");
            yield return SpeechHandler(interview.beforeInterview, interviewBubbleArea);
            m_gameStack.Pop();

            m_gameStack.Push("questions");

        Questions:
            int j = 0;
            if (m_loadQueue.Count > 0)
                j = int.Parse(m_loadQueue.Dequeue());
            for (; j < interview.questions.Length; ++j)
            {
                m_gameStack.Push(j.ToString());

                QuestionData question = interview.questions[j];

                if (m_loadQueue.Count > 0)
                {
                    string position = m_loadQueue.Dequeue();
                    switch (position)
                    {
                        case "question":
                            goto QuestionQuestion;
                        case "qte":
                            m_lastResult = false;
                            goto QuestionQte;
                        case "afterGoodAnswer":
                            m_lastResult = true;
                            goto AfterQuestion;
                        case "afterBadAnswer":
                            m_lastResult = false;
                            goto AfterQuestion;
                        default:
                            break;
                    }
                }

                m_gameStack.Push("beforeQuestion");
                yield return SpeechHandler(question.beforeQuestion, interviewBubbleArea);
                m_gameStack.Pop();

            QuestionQuestion:
                m_gameStack.Push("question");
                m_locked = true;
                CreateQuestion(question);
                yield return new WaitUntil(() => !m_locked);
                m_gameStack.Pop();
                
                AutoPresident();

            QuestionQte:
                if (!m_lastResult)
                {
                    m_gameStack.Push("qte");
                    m_locked = true;
                    CreateQTE(question.qte);
                    yield return new WaitUntil(() => !m_locked);
                    m_gameStack.Pop();

                    AutoPresident();
                }
                
            AfterQuestion:
                TalkData[] afterQuestion = m_lastResult ? question.afterGoodAnswer : question.afterBadAnswer;

                m_gameStack.Push(m_lastResult ? "afterGoodAnswer" : "afterBadAnswer");
                yield return SpeechHandler(afterQuestion, interviewBubbleArea);
                m_gameStack.Pop();

                m_gameStack.Pop();
            }
            m_gameStack.Pop();

        AfterInterview:
            m_gameStack.Push("afterInterview");
            yield return SpeechHandler(interview.afterInterview, interviewBubbleArea);
            m_gameStack.Pop();

            m_gameStack.Pop();
            m_gameStack.Pop();

            m_gameStack.Pop();
            if (m_gameStack.Count > 0)
                m_gameStack.Pop();

            m_totalScore += m_currentScore;
            totalScore.text = m_totalScore.ToString();
        }
        PickEnding();
    }

    public void SetState(string state)
    {
        m_gameState = state;
        SoundManager.PlayMusic(state);
        SetBackground(state);
        foreach (Character character in m_characters.Values)
            character.state = state;
        AutoPresident();
    }

    public void PickEnding()
    {
        if (m_totalScore >= 500)
        {
            //Perfect Ending
            bestEnding.gameObject.SetActive(true);
            SoundManager.PlayMusic("Morning");
        }
        else if (m_totalScore >= 300)
        {
            //Good Ending
            goodEnding.gameObject.SetActive(true);
            SoundManager.PlayMusic("War");
        }
        else
        {
            //Bad Ending
            badEnding.gameObject.SetActive(true);
            SoundManager.PlayMusic("GameOverMeeting");
        }
        HighScores.SaveScore(playerName, m_totalScore);
    }

    public IEnumerator SpeechHandler(TalkData[] talkData, Transform bubbleArea)
    {
        foreach (TalkData talk in talkData)
        {
            foreach (Action action in talk.preActions)
            {
                if (m_characters.ContainsKey(action.character))
                {
                    m_characters[action.character].Do(action.action);
                    if (m_characters[action.character].IsBlocking(action.action))
                    {
                        yield return new WaitUntil(() => m_hasClicked);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            SpeechBubble bubble = CreateSpeechBubble(talk, bubbleArea);
            yield return new WaitUntil(() => m_hasClicked);
            yield return new WaitForSeconds(0.2f);
            if (!bubble.Finished)
            {
                bubble.Skip();
                yield return new WaitUntil(() => m_hasClicked);
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(bubble.gameObject);
            foreach (Action action in talk.postActions)
            {
                if (m_characters.ContainsKey(action.character))
                {
                    m_characters[action.character].Do(action.action);
                    if (m_characters[action.character].IsBlocking(action.action))
                    {
                        yield return new WaitUntil(() => m_hasClicked);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
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

    public void AutoPresident()
    {
        if (m_currentScore > 75)
            m_characters["President"].Do("Neutral");
        else if (m_currentScore > 50)
            m_characters["President"].Do("Anxious");
        else if (m_currentScore > 25)
            m_characters["President"].Do("Monstrous");
        else
            m_characters["President"].Do("Monstrous2");
    }

    public SpeechBubble CreateSpeechBubble(TalkData talk, Transform master)
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
        bubble.textSpeed = textSpeed;

        return bubble;
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
        if (m_gameState == "Meeting")
            SoundManager.PlayMusic("GameOverMeeting");
        else
            SoundManager.PlayMusic("GameOverInterview");
        gameOver.SetActive(true);
    }

    public void Load()
    {
        StopCoroutine("GameCoroutine");
        m_locked = false;
        Time.timeScale = 1;
        List<MonoBehaviour> toDestroy = new List<MonoBehaviour>();
        toDestroy.AddRange(FindObjectsOfType<SpeechBubble>());
        toDestroy.AddRange(FindObjectsOfType<WordQTE>());
        toDestroy.AddRange(FindObjectsOfType<Question>());
        toDestroy.ForEach(x => Destroy(x.gameObject));
        m_gameData = ScenarioLoader.Load(gameData);
        Debug.Log(JsonUtility.ToJson(m_gameData));
        bestEnding.gameObject.SetActive(false);
        goodEnding.gameObject.SetActive(false);
        badEnding.gameObject.SetActive(false);
        StartCoroutine("GameCoroutine");
    }

    public void Load(string savefile)
    {
        StopCoroutine("GameCoroutine");

        try
        {
            SaveFile saveData = ScenarioLoader.LoadAsset<SaveFile>(savefile, "saves");
            m_currentScore = saveData.score;
            m_totalScore = saveData.totalScore;
            totalScore.text = m_totalScore.ToString();
            string[] state = saveData.state.Split('/');
            m_gameStack = new Stack<string>();
            m_loadQueue = new Queue<string>(state);
            gameData = saveData.scenario;
            playerName = saveData.name;
        }
        catch
        {
            SceneManager.LoadScene("MainMenu");
        }

        Load();
    }

    public void Save(string filename)
    {
        SaveFile saveData = new SaveFile();

        saveData.score = m_currentScore;
        saveData.totalScore = m_totalScore;
        string[] state = m_gameStack.ToArray();
        Array.Reverse(state);
        saveData.state = string.Join("/", state);
        saveData.scenario = gameData;
        saveData.name = playerName;

        ScenarioSaver.SaveAsset(saveData, filename, "saves");
    }
}

[Serializable]
public class CharacterInfo
{
    public string name;
    public Character character;
}
