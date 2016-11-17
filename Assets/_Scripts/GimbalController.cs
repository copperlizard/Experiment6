using UnityEngine;
using System.Collections;

public class GimbalController : MonoBehaviour
{
    public float m_desiredTilt = 45.0f;

    private Vector3 m_desiredUp, m_desiredForward, m_phoneUp;
    
	// Use this for initialization
	void Start ()
    {
        Input.gyro.enabled = true;

        m_desiredUp = Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * Vector3.up;
        m_desiredForward = Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * Vector3.forward;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isMobilePlatform)
        {
            m_phoneUp = -Input.gyro.gravity;
            transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * new Vector3(m_phoneUp.x * 1.5f, m_phoneUp.y * 1.5f, m_phoneUp.z).normalized);
        }
    }    
}
