using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

[System.Serializable]
public struct Record
{
    public float time;
    public int turns;
    public int par;
}

[System.Serializable]
public class Records
{
    public Record[] m_record = new Record[10];

    public Records ()
    {
        for (int i = 0; i < m_record.Length; i++)
        {
            m_record[i].time = 300.0f + (30.0f * i);
            m_record[i].turns = 100 + (10 * i);
            m_record[i].par = 15;
        }
    }
}

public enum GameMode { LEARN, TIMED, TURNS };

public class GameManager : MonoBehaviour
{
    public GameMode m_mode;

    public GameObject m_cube, m_pausePanel, m_recordsPanel, m_undoRedoPanel;
    public Text m_timeText, m_turnsText, m_stageText, m_lastMoveText;

    public Shader m_pauseReplacementShader;

    public bool m_isPaused = false;

    public VRubiksCubeHelper m_helper;
    
    private VRubiksCubeMonitor m_cubeMonitor;
    private VRubiksCubeController m_cubeController;
    private VRubiksCubeUserInput m_cubeInput;
    private GimbalController m_gimbalController;

    private Records m_recs = new Records();

    private string m_diplayedMove;

    private float m_solveTimeElapsed = 0.0f, m_pauseTimeElapsed = 0.0f;

    private bool m_modeComplete = false;

    // Use this for initialization
    void Start ()
    {
        if (m_cube == null)
        {
            Debug.Log("m_cube not assigned!");
        }
        else
        {
            m_cubeMonitor = m_cube.GetComponent<VRubiksCubeMonitor>();
            m_cubeController = m_cube.GetComponent<VRubiksCubeController>();
            m_cubeInput = m_cube.GetComponent<VRubiksCubeUserInput>();
            m_gimbalController = m_cube.transform.parent.gameObject.GetComponent<GimbalController>();
            
            if (m_cubeMonitor == null)
            {
                Debug.Log("m_cubeMonitor not found!");
            }

            if (m_cubeController == null)
            {
                Debug.Log("m_cubeController not found!");
            }

            if (m_cubeInput == null)
            {
                Debug.Log("m_cubeInput not found!");
            }

            if (m_gimbalController == null)
            {
                Debug.Log("m_gimbalController not found!");
            }
        }

        if (m_pausePanel == null)
        {
            Debug.Log("m_pausePanel not assigned!");
        }	

        if (m_recordsPanel == null)
        {
            Debug.Log("m_recordsPanel not assigned!");
        }

        if (m_undoRedoPanel == null)
        {
            Debug.Log("m_undoRedoPanel not assigned!");
        }

        if (m_timeText == null)
        {
            Debug.Log("m_timeText not assigned!");
        }

        if (m_turnsText == null)
        {
            Debug.Log("m_turnsText not assigned!");
        }

        if (m_stageText == null)
        {
            Debug.Log("m_stageText not assigned!");
        }

        if (m_lastMoveText == null)
        {
            Debug.Log("m_lastMoveText not assigned!");
        }

        switch (m_mode)
        {
            case GameMode.LEARN:
                m_turnsText.color = m_turnsText.color * 0.5f;
                m_timeText.color = m_timeText.color * 0.5f;

                if (m_helper == null)
                {
                    Debug.Log("m_helper not assigned!");
                }

                break;
            case GameMode.TIMED:
                m_turnsText.color = m_turnsText.color * 0.5f;
                m_stageText.color = m_stageText.color * 0.5f;
                break;
            case GameMode.TURNS:
                m_timeText.color = m_timeText.color * 0.5f;
                m_stageText.color = m_stageText.color * 0.5f;
                break;
            default:
                Debug.Log("GameMode error!");
                break;
        }

        // Start solve timer
        StartCoroutine(SolveTimer());
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateProgressText();

        switch (m_mode)
        {
            case GameMode.LEARN:
                LearnUpdate();
                break;
            case GameMode.TIMED:
                TimedUpdate();
                break;
            case GameMode.TURNS:
                TurnsUpdate();
                break;
            default:
                Debug.Log("GameMode error!");
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    IEnumerator SolveTimer()
    {
        while (m_cubeMonitor.m_cubePar == 0)
        {
            // Wait for cube to randomize
            yield return null;
        }

        float startTime = Time.time;
        //while (!m_cubeMonitor.m_cubeSolved)
        while (m_cubeMonitor.m_percentComplete != 1.0f) 
        {
            //Debug.Log("tick!");
                        
            if (!m_isPaused)
            {
                m_solveTimeElapsed = Time.time - startTime - m_pauseTimeElapsed;
            }
            else
            {
                m_pauseTimeElapsed = Time.time - startTime - m_solveTimeElapsed;
            }            
            
            yield return null;
        }

        Debug.Log("cube solved!");
        yield return null;
    }

    void UpdateProgressText ()
    {        
        float timeToDisplay = m_solveTimeElapsed, secsInHour = 3600.0f, secsInMin = 60.0f;
        int hours = 0, mins = 0, secs = 0; // Maybe add decsecs (decimal seconds)...

        while (timeToDisplay >= secsInHour)
        {
            hours++;
            timeToDisplay -= secsInHour;
        }
        while (timeToDisplay >= secsInMin)
        {
            mins++;
            timeToDisplay -= secsInMin;
        }
        secs = (int)timeToDisplay; 

        m_timeText.text = "Time - " + string.Format("{0:00}:{1:00}:{2:00}", hours, mins, secs);
        
        m_turnsText.text = "Turns - " + m_cubeMonitor.m_turns.ToString() + " / " + m_cubeMonitor.m_cubePar.ToString();

        m_stageText.text = "Stage - " + (m_cubeMonitor.m_stage + 1).ToString() + " / 6";

        // Display last move text when not randomizing, fade out with time
        if (m_cubeController.m_lastMoveType != "" && !m_cubeMonitor.m_randomizing)
        {
            m_lastMoveText.text = m_cubeController.m_lastMoveType;
            m_cubeController.m_lastMoveType = "";
            m_lastMoveText.color = new Color(m_lastMoveText.color.r, m_lastMoveText.color.g, m_lastMoveText.color.b, 1.0f);
        }
        else if (m_cubeMonitor.m_randomizing)
        {
            m_lastMoveText.text = m_cubeController.m_lastMoveType;
            m_cubeController.m_lastMoveType = "";
            m_lastMoveText.color = new Color(m_lastMoveText.color.r, m_lastMoveText.color.g, m_lastMoveText.color.b, 0.0f);
        }
        else
        {
            m_lastMoveText.color = new Color(m_lastMoveText.color.r, m_lastMoveText.color.g, m_lastMoveText.color.b, Mathf.Lerp(m_lastMoveText.color.a, 0.0f, 1.0f * Time.deltaTime));
        }
    }

    private void LearnUpdate ()
    {
        if (m_cubeMonitor.m_percentComplete <= 1.0f)
        {
            // Update helpers...  CURRENTLY NOT NECESSARY! MAYBE REDUCE TO ONE UPDATE METHOD FOR ALL GAMEMODES!
            
        }
        else
        {
            StartCoroutine(TimedComplete());
        }
    }

    private void TimedUpdate ()
    {
        if (m_cubeMonitor.m_percentComplete < 1.0f)
        {
            // Do nothing
        }
        else if (!m_modeComplete)
        {
            StartCoroutine(TimedComplete());
        }
    }

    IEnumerator TimedComplete ()
    {
        m_modeComplete = true;

        m_cubeInput.enabled = false;
        m_gimbalController.enabled = false;

        // Begin celebration
        StartCoroutine(spinCube());
        yield return new WaitForSeconds(2.5f);

        if (m_pauseReplacementShader != null)
        {
            Camera.main.SetReplacementShader(m_pauseReplacementShader, "RenderType");
        }
        m_recordsPanel.SetActive(true);
        
        m_undoRedoPanel.SetActive(false);

        Text[] panelText = m_recordsPanel.GetComponentsInChildren<Text>();
        
        panelText[1].text = BuildRecordsString();
        
        yield return null;
    }

    IEnumerator spinCube ()
    {
        while (true) // doesn't stop until the scene transition destroys the gameManager!!!
        {
            m_cube.transform.Rotate(5.0f * Time.deltaTime, 15.0f * Time.deltaTime, 20.0f * Time.deltaTime);
            yield return null;
        }        
    }

    private void TurnsUpdate ()
    {
        TimedUpdate();
    }

    // Toggles game pause state
    public void PauseGame ()
    {
        if (m_isPaused)
        {
            ResumeGame();
            return;
        }
        else
        {
            m_isPaused = true;
            m_cubeInput.enabled = false;
            m_undoRedoPanel.SetActive(false);
            m_pausePanel.SetActive(true);
            m_helper.m_gameIsPaused = true;
            if (m_pauseReplacementShader != null)
            {
                Camera.main.SetReplacementShader(m_pauseReplacementShader, "RenderType");
            }
        }
    }

    // Resumes game only
    public void ResumeGame ()
    {
        if (!m_isPaused)
        {
            return;
        }

        Camera.main.ResetReplacementShader();
        m_pausePanel.SetActive(false);
        m_helper.m_gameIsPaused = false;
        m_undoRedoPanel.SetActive(true);
        m_isPaused = false;
        m_cubeInput.enabled = true;
    }

    private void LoadRecords ()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        switch (m_mode)
        {
            case GameMode.LEARN:
                if (File.Exists(Application.persistentDataPath + "/LearnRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/LearnRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/LearnRecords.dat");
                }
                break;
            case GameMode.TIMED:
                if (File.Exists(Application.persistentDataPath + "/TimedRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TimedRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);                    
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TimedRecords.dat");
                }
                break;
            case GameMode.TURNS:
                if (File.Exists(Application.persistentDataPath + "/TurnsRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TurnsRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TurnsRecords.dat");
                }
                break;
            default:
                file = File.Create(Application.persistentDataPath + "/LearnRecords.dat");
                break;
        }

        file.Close();
    }

    private void SaveRecords ()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        switch (m_mode)
        {
            case GameMode.LEARN:
                if (File.Exists(Application.persistentDataPath + "/LearnRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/LearnRecords.dat", FileMode.Open);                    
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/LearnRecords.dat");
                }
                break;
            case GameMode.TIMED:
                if (File.Exists(Application.persistentDataPath + "/TimedRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TimedRecords.dat", FileMode.Open);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TimedRecords.dat");
                }
                break;
            case GameMode.TURNS:
                if (File.Exists(Application.persistentDataPath + "/TurnsRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TurnsRecords.dat", FileMode.Open);                    
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TurnsRecords.dat");
                }
                break;
            default:
                file = File.Create(Application.persistentDataPath + "/LearnRecords.dat");
                break;
        }

        bf.Serialize(file, m_recs);
        file.Close();
    }

    private void UpdateRecords ()
    {
        switch (m_mode)
        {
            case GameMode.LEARN:
                for (int i = 0; i < m_recs.m_record.Length; i++)
                {
                    //|| (m_solveTimeElapsed == m_recs.m_record[i].time && m_cubeMonitor.m_cubePar >= m_recs.m_record[i].par)
                    if (m_solveTimeElapsed < m_recs.m_record[i].time)
                    {
                        for (int j = 1; j <= m_recs.m_record.Length - (i + 1); j++)
                        {
                            //Debug.Log("j == " + j.ToString());
                            m_recs.m_record[m_recs.m_record.Length - j] = m_recs.m_record[(m_recs.m_record.Length - j) - 1];
                        }

                        m_recs.m_record[i].time = m_solveTimeElapsed;
                        m_recs.m_record[i].par = m_cubeMonitor.m_cubePar;
                        m_recs.m_record[i].turns = m_cubeMonitor.m_turns;
                        SaveRecords();
                        break;
                    }
                }
                break;
            case GameMode.TIMED:
                for (int i = 0; i < m_recs.m_record.Length; i++)
                {
                    //Debug.Log("recs.m_record[" + i.ToString() + "].time == " + m_recs.m_record[i].time.ToString());

                    //|| (m_solveTimeElapsed == recs.m_record[i].time && m_cubeMonitor.m_cubePar > recs.m_record[i].par)
                    if (m_solveTimeElapsed < m_recs.m_record[i].time)
                    {
                        //Debug.Log("m_solveTimeElapsed < recs.m_record[" + i.ToString() + "].time == " + (m_solveTimeElapsed < m_recs.m_record[i].time).ToString());
                        //Debug.Log(m_solveTimeElapsed.ToString() + " < " + m_recs.m_record[i].time.ToString() + " == " + (m_solveTimeElapsed < m_recs.m_record[i].time).ToString());
                        
                        for (int j = 1; j <= m_recs.m_record.Length - (i + 1); j++)
                        {
                            //Debug.Log("recs.m_record[" + (m_recs.m_record.Length - j).ToString() + "] = recs.m_record[" + ((m_recs.m_record.Length - j) - 1).ToString() + "]");

                            m_recs.m_record[m_recs.m_record.Length - j].time = m_recs.m_record[(m_recs.m_record.Length - j) - 1].time;
                            m_recs.m_record[m_recs.m_record.Length - j].par = m_recs.m_record[(m_recs.m_record.Length - j) - 1].par;
                            m_recs.m_record[m_recs.m_record.Length - j].turns = m_recs.m_record[(m_recs.m_record.Length - j) - 1].turns;
                        }

                        m_recs.m_record[i].time = m_solveTimeElapsed;
                        m_recs.m_record[i].par = m_cubeMonitor.m_cubePar;
                        m_recs.m_record[i].turns = m_cubeMonitor.m_turns;
                        SaveRecords();
                        break;
                    }
                }
                break;
            case GameMode.TURNS:
                for (int i = 0; i < m_recs.m_record.Length; i++)
                {
                    //|| (m_cubeMonitor.m_turns == m_recs.m_record[i].turns && m_cubeMonitor.m_cubePar >= m_recs.m_record[i].par)
                    if (m_cubeMonitor.m_turns < m_recs.m_record[i].turns)
                    {
                        for (int j = 1; j <= m_recs.m_record.Length - (i + 1); j++)
                        {
                            //Debug.Log("j == " + j.ToString());
                            m_recs.m_record[m_recs.m_record.Length - j] = m_recs.m_record[(m_recs.m_record.Length - j) - 1];
                        }

                        m_recs.m_record[i].time = m_solveTimeElapsed;
                        m_recs.m_record[i].par = m_cubeMonitor.m_cubePar;
                        m_recs.m_record[i].turns = m_cubeMonitor.m_turns;
                        SaveRecords();
                        break;
                    }
                }
                break;
            default:
                break;
        }
    }

    private string BuildRecordsString ()
    {
        LoadRecords();  // Load saved records

        /*
        Debug.Log(System.Environment.NewLine + "loaded records...");
        for (int i = 0; i < m_recs.m_record.Length; i++)
        {
            Debug.Log("recs.m_record[" + i.ToString() + "].time == " + m_recs.m_record[i].time.ToString());
        }
        */

        UpdateRecords();  // Check if new record set; saves records if changed      

        /*
        Debug.Log(System.Environment.NewLine + "updated records...");
        for (int i = 0; i < m_recs.m_record.Length; i++)
        {
            Debug.Log("recs.m_record[" + i.ToString() + "].time == " + m_recs.m_record[i].time.ToString());
        }
        */

        string RecordsString = "";
        if (m_mode == GameMode.LEARN || m_mode == GameMode.TIMED)
        {
            // #1 - 00:00:00.00 - 000 / 00
            for (int i = 0; i < m_recs.m_record.Length; i++)
            {
                RecordsString += "#" + (i + 1).ToString() + " - ";

                int hours = 0, mins = 0, secs = 0, decsecs = 0;
                float recTime = m_recs.m_record[i].time, secsInHour = 3600.0f, secsInMin = 60.0f;
                while (recTime >= secsInHour)
                {
                    hours++;
                    recTime -= secsInHour;
                }
                while (recTime >= secsInMin)
                {
                    mins++;
                    recTime -= secsInMin;
                }
                secs = (int)recTime;

                decsecs = (int)((recTime - secs) * 100.0f);

                RecordsString += string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hours, mins, secs, decsecs) + " - ";
                RecordsString += m_recs.m_record[i].turns.ToString() + " / " + m_recs.m_record[i].par.ToString();
                RecordsString += System.Environment.NewLine;
            }
        }
        else if (m_mode == GameMode.TURNS)
        {
            // #1 - 000 / 00 - 00:00:00.00
            for (int i = 0; i < m_recs.m_record.Length; i++)
            {
                RecordsString += "#" + (i + 1).ToString() + " - ";

                int hours = 0, mins = 0, secs = 0, decsecs = 0; // Maybe add decsecs (decimal seconds)...
                float recTime = m_recs.m_record[i].time, secsInHour = 3600.0f, secsInMin = 60.0f;
                while (recTime >= secsInHour)
                {
                    hours++;
                    recTime -= secsInHour;
                }
                while (recTime >= secsInMin)
                {
                    mins++;
                    recTime -= secsInMin;
                }
                secs = (int)recTime;

                decsecs = (int)((recTime - secs) * 100.0f);

                RecordsString += m_recs.m_record[i].turns.ToString() + " / " + m_recs.m_record[i].par.ToString() + " - ";
                RecordsString += string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hours, mins, secs, decsecs);
                RecordsString += System.Environment.NewLine;
            }
        }       
        return RecordsString;
    }
}