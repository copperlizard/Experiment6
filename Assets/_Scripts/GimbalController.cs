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

            Debug.Log("m_desiredTilt == " + m_desiredTilt.ToString());
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

        //Quaternion rot = Quaternion.FromToRotation(transform.up, m_phoneUp);
        //transform.rotation = rot * transform.rotation; // rotations are weird, probably useless; no flipping though...

        //transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * m_phoneUp.normalized); // works as expected but no amplification

        //transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * new Vector3(m_phoneUp.x * 1.5f, m_phoneUp.y * 1.5f, m_phoneUp.z).normalized); // sort of works but does not amplify axis equally

        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, m_phoneUp.normalized);
        //transform.rotation = Quaternion.LookRotation(Quaternion.Euler(-m_desiredTilt, 0.0f, 0.0f) * (rot * m_phoneUp.normalized)); // desired tilt not working as intended; cube flips around...
        //transform.rotation = Quaternion.LookRotation(rot * m_phoneUp.normalized); // amplification is good, but cube flips around at the limits and no desired tilt control...

        //
        //
        //  cube should rotate around x when phone rotates around x
        //  cube should rotate around y when phone rotates around y
        //  cube can ignore z rotations...
        //
        //  translate m_phoneUp into x and y cube tilt angles...

        float xAng = 0.0f, yAng = 0.0f;

        Vector3 YZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.right).normalized;
        Vector3 XZproj = Vector3.ProjectOnPlane(m_phoneUp, Vector3.up).normalized;

        xAng = Vector3.Angle(Vector3.forward, YZproj);
        yAng = Vector3.Angle(Vector3.right, XZproj);

        Debug.Log("m_phoneUp == " + m_phoneUp.ToString() + System.Environment.NewLine + 
            "YZproj == " + YZproj.ToString() + System.Environment.NewLine +
            "XZproj == " + XZproj.ToString() + System.Environment.NewLine +
            "xAng == " + xAng.ToString() + " ; yAng == " + yAng.ToString());

        //transform.rotation = Quaternion.Euler(-(xAng - m_desiredTilt), 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(-(xAng - m_desiredTilt) * 2.0f, -(yAng - 90.0f) * 2.0f, 0.0f);
        //transform.rotation = Quaternion.Euler(0.0f, -yAng * 2.0f, -(xAng - m_desiredTilt));
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
