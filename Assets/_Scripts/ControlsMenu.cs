using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlsMenu : MonoBehaviour
{
    public Text m_homeTiltText, m_gyroSmoothingText, m_gyroPollRateText, m_accelerometerSmoothingText;

    public Slider m_homeTiltSlider, m_gyroSmoothingSlider, m_gyroPollRateSlider, m_accelerometerSmoothingSlider;

    public Toggle m_gyroToggle;

    private bool m_gyroOn = false;

	// Use this for initialization
	void Awake ()
    {
        if (m_gyroSmoothingText == null)
        {
            Debug.Log("m_gyroSliderText not assigned!");
        }

        if (m_accelerometerSmoothingText == null)
        {
            Debug.Log("m_accelerometerSmoothingText not assigned!");
        }

        if (m_gyroSmoothingSlider == null)
        {
            Debug.Log("m_gyroSmoothingSlider not assigned!");
        }

        if (m_accelerometerSmoothingSlider == null)
        {
            Debug.Log("m_accelerometerSmoothingSlider not assigned!");
        }

        if (m_gyroToggle == null)
        {
            Debug.Log("m_gyroToggle not assigned!");
        }

        LoadPlayerPrefs();

        ToggleGyro();
    }

    private void LoadPlayerPrefs ()
    {
        float prefHomeTilt = PlayerPrefs.GetFloat("DesiredTilt", -45.0f);
        if (prefHomeTilt != -45.0f)
        {
            Debug.Log("loaded home tilt pref!");

            m_homeTiltSlider.value = prefHomeTilt / 90.0f;
        }

        // Gyro or accelerometer?
        int prefUseGyro = PlayerPrefs.GetInt("UseGyro", -1);
        if (prefUseGyro != -1)
        {
            m_gyroToggle.isOn = (prefUseGyro == 1);
        }

        // Set desired gyro update rate
        float prefGyroPollFreq = PlayerPrefs.GetFloat("GyroPollFreq", -1.0f);
        if (prefGyroPollFreq != -1.0f)
        {
            //Debug.Log("prefGyroPollFreq == " + prefGyroPollFreq.ToString());

            m_gyroPollRateSlider.value = prefGyroPollFreq;           
        }        

        float prefGyroSmoothing = PlayerPrefs.GetFloat("GyroSmoothing", -1.0f);
        if (prefGyroSmoothing != -1.0f)
        {
            m_gyroSmoothingSlider.value = prefGyroSmoothing;
        }

        float prefAccelerometerSmoothing = PlayerPrefs.GetFloat("AccelerometerSmoothing", -1.0f);
        if (prefAccelerometerSmoothing != -1.0f)
        {
            m_accelerometerSmoothingSlider.value = prefAccelerometerSmoothing;
        }
    }
		
	public void ToggleGyro ()
    {
	    if (m_gyroToggle.isOn)
        {
            //Debug.Log("toggle is on!");

            if (m_gyroOn == false)
            {
                //Debug.Log("Setting gyro on! changing text colors...");

                m_gyroOn = true;
                PlayerPrefs.SetInt("UseGyro", 1);

                m_accelerometerSmoothingText.color = new Color(0.5f, 0.5f, 0.5f);
                m_accelerometerSmoothingSlider.interactable = false;

                ColorBlock cblock = m_accelerometerSmoothingSlider.colors;
                cblock.colorMultiplier = 0.5f;
                m_accelerometerSmoothingSlider.colors = cblock;

                if (!m_gyroSmoothingSlider.interactable)
                {
                    m_gyroSmoothingText.color = new Color(1.0f, 1.0f, 1.0f);
                    m_gyroPollRateText.color = new Color(1.0f, 1.0f, 1.0f);
                    m_gyroSmoothingSlider.interactable = true;
                    m_gyroPollRateSlider.interactable = true;
                    cblock = m_gyroSmoothingSlider.colors;
                    cblock.colorMultiplier = 1.0f;
                    m_gyroSmoothingSlider.colors = cblock;
                }
            }
        }
        else
        {
            //Debug.Log("toggle is off!");

            if (m_gyroOn == true)
            {
                //Debug.Log("Setting gyro off! changing text colors...");

                m_gyroOn = false;
                PlayerPrefs.SetInt("UseGyro", 0);

                m_gyroSmoothingText.color = new Color(0.5f, 0.5f, 0.5f);
                m_gyroPollRateText.color = new Color(0.5f, 0.5f, 0.5f);
                m_gyroSmoothingSlider.interactable = false;
                m_gyroPollRateSlider.interactable = false;

                ColorBlock cblock = m_gyroSmoothingSlider.colors;
                cblock.colorMultiplier = 0.5f;
                m_gyroSmoothingSlider.colors = cblock;

                if (!m_accelerometerSmoothingSlider.interactable)
                {
                    m_accelerometerSmoothingText.color = new Color(1.0f, 1.0f, 1.0f);
                    m_accelerometerSmoothingSlider.interactable = true;
                    cblock = m_accelerometerSmoothingSlider.colors;
                    cblock.colorMultiplier = 1.0f;
                    m_accelerometerSmoothingSlider.colors = cblock;
                }
            }
        }
	}

    public void ResetControls ()
    {
        m_homeTiltSlider.value = 0.5f;       
        m_gyroToggle.isOn = true;
        m_gyroSmoothingSlider.value = 0.0f;
        m_gyroPollRateSlider.value = 1.0f;
        m_accelerometerSmoothingSlider.value = 0.3f;
        ToggleGyro();

        /*
        Debug.Log("m_homeTiltSlider.value == " + m_homeTiltSlider.value.ToString() + System.Environment.NewLine +
            "m_gyroSmoothingSlider.value == " + m_gyroSmoothingSlider.value.ToString() + System.Environment.NewLine +
            "m_gyroPollRateSlider.value == " + m_gyroPollRateSlider.value.ToString() + System.Environment.NewLine +
            "m_accelerometerSmoothingSlider.value == " + m_accelerometerSmoothingSlider.value.ToString()); */
    }

    public void SetHomeTilt ()
    {
        //Debug.Log("setting home tilt to " + (m_homeTiltSlider.value * 90.0f).ToString());

        PlayerPrefs.SetFloat("DesiredTilt", m_homeTiltSlider.value * 90.0f);
    }

    public void SetGyroPollRate ()
    {
        PlayerPrefs.SetFloat("GyroPollFreq", m_gyroPollRateSlider.value);
    }

    public void SetGyroSmoothing ()
    {
        //Debug.Log("setting gyrosmoothing to " + m_gyroSmoothingSlider.value.ToString());

        PlayerPrefs.SetFloat("GyroSmoothing", m_gyroSmoothingSlider.value);
    }

    public void SetAccelerometerSmoothing()
    {
        PlayerPrefs.SetFloat("AccelerometerSmoothing", m_accelerometerSmoothingSlider.value);
    }
}
