using UnityEngine;
using System.Collections;

public class LowPassV3Filter
{
    public Vector3 m_filteredVector;
    private float m_tau;
    private int m_iter = 0;

    public LowPassV3Filter (float tau)
    {
        this.m_tau = tau;
    }

    public Vector3 Step (float h, Vector3 raw)
    {
        if (m_iter == 0) // if it's the first iteration
            m_filteredVector = raw; // just initate filteredValue
        else
        {
            float alpha = Mathf.Exp(-h / m_tau); // calculate alpha value based on time step and filter's time constant
            m_filteredVector = alpha * m_filteredVector + (1 - alpha) * raw; // calculate new filteredValue from previous value and new raw value
        }
        m_iter++; // increment iteration number
        return m_filteredVector;
    }

    public void Reset ()
    {
        m_iter = 0;
    }
}

public class GimbalController : MonoBehaviour
{
    [HideInInspector]
    public float m_desiredTilt = 45.0f, m_tiltAmplification = 1.5f;

    private LowPassV3Filter m_lowPassFilter;

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

        float prefTiltAmplification = PlayerPrefs.GetFloat("TiltAmplification", -1.0f);
        if (prefTiltAmplification != -1.0f)
        {
            m_tiltAmplification = 1.0f + prefTiltAmplification;
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
            float prefGyroPollFreq = PlayerPrefs.GetFloat("GyroPollFreq", -1.0f);
            if (prefGyroPollFreq != -1.0f)
            {
                m_gyroPollFreq = prefGyroPollFreq * 60.0f; // 60hz max...
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

            m_lowPassFilter = new LowPassV3Filter(m_accelerometerSmoothing);
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

        if (m_gyroSmoothing > 0.0f)
        {
            m_phoneUp = Vector3.Lerp(m_lastPhoneUp, m_phoneUp, m_gyroSmoothing);
        }        

        float xAng = 0.0f, yAng = 0.0f;

        Vector3 YZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.right).normalized;
        Vector3 XZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.up).normalized;

        xAng = Vector3.Angle(Vector3.forward, YZproj);
        yAng = Vector3.Angle(Vector3.right, XZproj);

        /*
        Debug.Log("m_phoneUp == " + m_phoneUp.ToString() + System.Environment.NewLine + 
            "YZproj == " + YZproj.ToString() + System.Environment.NewLine +
            "XZproj == " + XZproj.ToString() + System.Environment.NewLine +
            "xAng == " + xAng.ToString() + " ; yAng == " + yAng.ToString());
            */
        
        transform.rotation = Quaternion.Euler(-(xAng - m_desiredTilt) * m_tiltAmplification, -(yAng - 90.0f) * m_tiltAmplification, 0.0f);        
    }

    void AccelerometerUpdate ()
    {
        m_phoneUp = m_lowPassFilter.Step(Time.deltaTime, Input.acceleration);

        float xAng = 0.0f, yAng = 0.0f;

        Vector3 YZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.right).normalized;
        Vector3 XZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.up).normalized;

        xAng = Vector3.Angle(Vector3.forward, YZproj);
        yAng = Vector3.Angle(Vector3.right, XZproj);

        /*
        Debug.Log("m_phoneUp == " + m_phoneUp.ToString() + System.Environment.NewLine + 
            "YZproj == " + YZproj.ToString() + System.Environment.NewLine +
            "XZproj == " + XZproj.ToString() + System.Environment.NewLine +
            "xAng == " + xAng.ToString() + " ; yAng == " + yAng.ToString());
            */

        transform.rotation = Quaternion.Euler((xAng - 180.0f + m_desiredTilt) * m_tiltAmplification, (yAng - 90.0f) * m_tiltAmplification, 0.0f);
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
