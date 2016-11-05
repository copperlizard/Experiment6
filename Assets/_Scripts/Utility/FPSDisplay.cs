using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class FPSDisplay : MonoBehaviour
{
    public Text m_displayText;
    private float m_deltaTime;

	// Use this for initialization
	void Start ()
    {
        m_displayText.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_deltaTime += (Time.deltaTime - m_deltaTime) * 0.1f;

        float msec = m_deltaTime * 1000.0f;
        float fps = 1.0f / m_deltaTime;
        m_displayText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);        
    }
}
