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
        GyroInput();

        //transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * m_phoneUp);
        transform.rotation = Quaternion.LookRotation(Quaternion.Euler(m_desiredTilt, 0.0f, 0.0f) * new Vector3(m_phoneUp.x * 1.5f, m_phoneUp.y * 1.5f, m_phoneUp.z).normalized);
    }

    public void GyroInput()
    {           
        //Quaternion phoneAttitude = Input.gyro.attitude; //probably not useful to me...

        m_phoneUp = -Input.gyro.gravity;

        /*
        Vector3 phoneUpTilt = Vector3.ProjectOnPlane(m_phoneUp, Vector3.right); //check tilt
        Vector3 phoneUpRoll = Vector3.ProjectOnPlane(m_phoneUp, Vector3.up); //check roll

        float tiltAngle = Vector3.Angle(phoneUpTilt, m_desiredUp);
        float rollAngle = Vector3.Angle(phoneUpRoll, Vector3.forward);

        int tiltDir = (Vector3.Dot(m_phoneUp, m_desiredForward) > 0.0f) ? 1 : -1;
        int rollDir = (Vector3.Dot(m_phoneUp, Vector3.right) > 0.0f) ? 1 : -1;

        tiltAngle *= tiltDir;
        rollAngle *= rollDir;

        
        Debug.Log("Gyro enabled status == " + Input.gyro.enabled.ToString() + System.Environment.NewLine +
            "phoneAttidue == " + phoneAttitude.ToString() + " or " + phoneAttitude.eulerAngles.ToString() + System.Environment.NewLine +
            "phoneUp == " + m_phoneUp.ToString() + System.Environment.NewLine + 
            " ; phoneUpTilt == " + phoneUpTilt.ToString() + " ; phoneUpRoll == " + phoneUpRoll.ToString() + System.Environment.NewLine +
            "tiltAngle == " + tiltAngle.ToString() + " ; rollAngle == " + rollAngle.ToString() + System.Environment.NewLine +
            "current gimbal.rotation == " + transform.rotation.ToString() + " or " + transform.rotation.eulerAngles.ToString());
        */
    }
}
