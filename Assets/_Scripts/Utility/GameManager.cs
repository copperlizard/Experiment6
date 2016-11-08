using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameMode { LEARN, TIMED, TURNS };

public class GameManager : MonoBehaviour
{
    public GameMode m_mode;

    public GameObject m_cube, m_pausePanel;
    public Text m_timeText, m_turnsText, m_stageText, m_lastMoveText;

    public Shader m_pauseReplacementShader;

    public bool m_isPaused = false;
    
    private VRubiksCubeMonitor m_cubeMonitor;
    private VRubiksCubeController m_cubeController;

    private string m_diplayedMove;

    private float m_solveTimeElapsed = 0.0f, m_pauseTimeElapsed = 0.0f;

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
            
            if (m_cubeMonitor == null)
            {
                Debug.Log("m_cubeMonitor not found!");
            }

            if (m_cubeController == null)
            {
                Debug.Log("m_cubeController not found!");
            }
        }

        if (m_pausePanel == null)
        {
            Debug.Log("m_pausePanel not assigned!");
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

        m_stageText.text = "Stage - " + (m_cubeMonitor.m_stage + 1).ToString() + " / 7";

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

    }

    private void TimedUpdate ()
    {

    }

    private void TurnsUpdate ()
    {

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
            m_pausePanel.SetActive(true);
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
        m_isPaused = false;
    }
}
