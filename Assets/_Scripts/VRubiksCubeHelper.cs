﻿using UnityEngine;
using System.Collections;

public class VRubiksCubeHelper : MonoBehaviour
{
    public GameManager m_gameManager;

    public GameObject m_cube, m_helperPanel, m_undoRedoPanel;

    public Shader m_helpReplacementShader;

    [HideInInspector]
    public bool m_gameIsPaused = false;

    private VRubiksCubeMonitor m_cubeMonitor;
    private VRubiksCubeUserInput m_cubeInput;

    private int m_stageMax = 0;

    private bool m_isHelping = false;

	// Use this for initialization
	void Start ()
    {
        if (m_gameManager == null)
        {
            Debug.Log("m_gameManager not assigned!");
        }

	    if (m_cube == null)
        {
            Debug.Log("m_cube not assigned!");
        }
        else
        {
            m_cubeMonitor = m_cube.GetComponent<VRubiksCubeMonitor>();
            m_cubeInput = m_cube.GetComponent<VRubiksCubeUserInput>();            

            if (m_cubeMonitor == null)
            {
                Debug.Log("m_monitor not found!");
            }

            if (m_cubeInput == null)
            {
                Debug.Log("m_cubeInput not found!");
            }
        }

        if (m_helperPanel == null)
        {
            Debug.Log("m_helperPanel not assigned");
        }
	}

    void Update ()
    {
        if (m_gameIsPaused && m_isHelping)
        {
            StopHelping();
        }
    }

    // Called by button
    public void Help ()
    {
        if (m_isHelping)
        {
            StopHelping();
            return;
        }
        else
        {
            //Debug.Log("Helping!");

            if (m_gameManager.m_modeComplete)
            {
                return;
            }

            m_isHelping = true;
            m_cubeInput.enabled = false;
            m_undoRedoPanel.SetActive(false);
            m_helperPanel.SetActive(true);
            if (m_helpReplacementShader != null)
            {
                Camera.main.SetReplacementShader(m_helpReplacementShader, "RenderType");
            }

            if (m_cubeMonitor.m_stage > m_stageMax)
            {
                m_stageMax = m_cubeMonitor.m_stage;
            }
        }
    }

    public void StopHelping ()
    {
        if (!m_isHelping)
        {
            return;
        }
        else
        {
            if (!m_gameIsPaused)
            {
                Camera.main.ResetReplacementShader();
                m_undoRedoPanel.SetActive(true);
            }
            
            m_isHelping = false;
            m_cubeInput.enabled = true;
            m_helperPanel.SetActive(false);
        }
    }

    public void StartSolve ()
    {
        StopHelping();
        m_cubeMonitor.SolveCube();
    }
}
