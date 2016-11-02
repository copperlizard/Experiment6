using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Shader m_pauseReplacementShader;

    public bool m_isPaused = false;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

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

            if (m_pauseReplacementShader != null)
            {
                Camera.main.SetReplacementShader(m_pauseReplacementShader, "RenderType");
            }
        }
    }

    public void ResumeGame ()
    {
        Camera.main.ResetReplacementShader();
        m_isPaused = false;
    }
}
