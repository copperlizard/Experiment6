﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameMode { LEARN, TIMED, TURNS };

public class GameManager : MonoBehaviour
{
    public GameMode m_mode;

    public GameObject m_cube, m_pausePanel;
    public Text m_timeText, m_turnsText, m_stageText;

    public Shader m_pauseReplacementShader;

    public bool m_isPaused = false;
    
    private VRubiksCubeMonitor m_cubeMonitor;

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
            
            if (m_cubeMonitor == null)
            {
                Debug.Log("m_cubeMonitor not found!");
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
        while (!m_cubeMonitor.m_cubeSolved)
        {            
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
        
        m_timeText.text = "Time - " + hours.ToString() + ":" + mins.ToString() + ":" + secs.ToString();

        m_turnsText.text = "Turns - " + m_cubeMonitor.m_turns.ToString() + "/" + m_cubeMonitor.m_cubePar.ToString();

        m_stageText.text = "Stage - " + (m_cubeMonitor.m_stage + 1).ToString() + "/7";
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
