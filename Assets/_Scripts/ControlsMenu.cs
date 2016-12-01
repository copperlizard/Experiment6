using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlsMenu : MonoBehaviour
{
    public Text m_gyroSliderText, m_accelerometerSliderText;

    public Slider m_gyroSlider, m_accelerometerSlider;

    public Toggle m_gyroToggle;

    private bool m_gyroOn = false;

	// Use this for initialization
	void Start ()
    {
        if (m_gyroSliderText == null)
        {
            Debug.Log("m_gyroSliderText not assigned!");
        }

        if (m_accelerometerSliderText == null)
        {
            Debug.Log("m_accelerometerSliderText not assigned!");
        }

        if (m_gyroSlider == null)
        {
            Debug.Log("m_gyroSlider not assigned!");
        }

        if (m_accelerometerSlider == null)
        {
            Debug.Log("m_accelerometerSlider not assigned!");
        }

        if (m_gyroToggle == null)
        {
            Debug.Log("m_gyroToggle not assigned!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (m_gyroToggle.isOn)
        {
            if (m_gyroOn == false)
            {
                m_gyroOn = true;

                m_accelerometerSliderText.color = new Color(0.5f, 0.5f, 0.5f);
                m_accelerometerSlider.interactable = false;

                ColorBlock cblock = m_accelerometerSlider.colors;
                cblock.colorMultiplier = 0.5f;
                m_accelerometerSlider.colors = cblock;

                if (!m_gyroSlider.interactable)
                {
                    m_gyroSliderText.color = new Color(1.0f, 1.0f, 1.0f);
                    m_gyroSlider.interactable = true;
                    cblock = m_gyroSlider.colors;
                    cblock.colorMultiplier = 1.0f;
                    m_gyroSlider.colors = cblock;
                }
            }
        }
        else
        {
            if (m_gyroOn == true)
            {
                m_gyroOn = false;

                m_gyroSliderText.color = new Color(0.5f, 0.5f, 0.5f);
                m_gyroSlider.interactable = false;

                ColorBlock cblock = m_gyroSlider.colors;
                cblock.colorMultiplier = 0.5f;
                m_gyroSlider.colors = cblock;

                if (!m_accelerometerSlider.interactable)
                {
                    m_accelerometerSliderText.color = new Color(1.0f, 1.0f, 1.0f);
                    m_accelerometerSlider.interactable = true;
                    cblock = m_accelerometerSlider.colors;
                    cblock.colorMultiplier = 1.0f;
                    m_accelerometerSlider.colors = cblock;
                }
            }
        }
	}
}
