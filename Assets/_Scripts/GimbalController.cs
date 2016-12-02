using UnityEngine;
using System.Collections;

public class GimbalController : MonoBehaviour
{
    public float m_desiredTilt = 45.0f;

    private Vector3 m_phoneUp, m_lastPhoneUp;

    private float m_gyroPollFreq = 60.0f, m_gyroPollPer = 1.0f / 60.0f, m_gyroSmoothing = 1.0f, m_accelerometerSmoothing = 0.3f;

    private bool m_useGyro = true;

	// Use this for initialization
	void Start ()
    {
#if UNITY_ANDROID
        MobileStart();
#elif UNITY_IOS
        MobileStart();
#elif UNITY_WIN
        PCStart();
#endif 
    }

    void MobileStart ()
    {
        // Set desired tilt angle
        float prefDesiredTilt = PlayerPrefs.GetFloat("DesiredTilt", -45.0f);
        if (prefDesiredTilt != -45.0f)
        {
            m_desiredTilt = prefDesiredTilt;
        }

        // Gyro or accelerometer?
        int prefUseGyro = PlayerPrefs.GetInt("UseGyro", -1);
        if (prefUseGyro != -1)
        {
            m_useGyro = (prefUseGyro == 1);
        }

        if (m_useGyro)
        {
            Input.gyro.enabled = true;

            // Set desired gyro update rate
            float prefGyroPollFreq = PlayerPrefs.GetFloat("GyroPollFreq", 0.0f);
            if (prefGyroPollFreq != 0.0f)
            {
                m_gyroPollFreq = prefGyroPollFreq;
                m_gyroPollPer = 1.0f / m_gyroPollFreq;
            }
            Input.gyro.updateInterval = m_gyroPollPer;

            float prefGyroSmoothing = PlayerPrefs.GetFloat("GyroSmoothing", -1.0f);
            if (prefGyroSmoothing != -1.0f)
            {
                m_gyroSmoothing = prefGyroSmoothing;
            }
        }
        else
        {
            Input.gyro.enabled = false;

            float prefAccelerometerSmoothing = PlayerPrefs.GetFloat("AccelerometerSmoothing", -1.0f);
            if (prefAccelerometerSmoothing != -1.0f)
            {
                m_accelerometerSmoothing = prefAccelerometerSmoothing;
            }
        }
    }

    void PCStart ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_ANDROID
        MobileUpdate();
#elif UNITY_IOS
        MobileUpdate();
#elif UNITY_WIN
        PCUpdate();
#endif        
    }
    
    void GyroUpdate ()
    {
        m_lastPhoneUp = m_phoneUp;
        m_phoneUp = -Input.gyro.gravity;

        if (m_gyroSmoothing < 1.0f)
        {
            m_phoneUp = Vector3.Lerp(m_lastPhoneUp, m_phoneUp, m_gyroSmoothing);
        }
                
        transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * new Vector3(m_phoneUp.x * 1.5f, m_phoneUp.y * 1.5f, m_phoneUp.z).normalized);
    }
    
    void AccelerometerUpdate ()
    {
        transform.rotation *= Quaternion.Euler(Input.acceleration.y / 6, -Input.acceleration.x / 3, 0);
    }

    // When compiled for mobile
    void MobileUpdate ()
    {
        if (m_useGyro)
        {
            GyroUpdate();
        }
        else
        {
            AccelerometerUpdate();
        }
    }
    
    // When compiled for WIN PC
    void PCUpdate ()
    {
        
    }    
}
