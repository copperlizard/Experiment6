using UnityEngine;
using System.Collections;

public class RotationTest : MonoBehaviour
{
    public Vector3 m_rotate;

    public bool m_debug = false;

    bool m_rotating = false;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (!m_rotating)
        {
            //Quaternion tarRot = transform.rotation * Quaternion.Euler(m_rotate);
            Quaternion tarRot = Quaternion.Euler(m_rotate) * transform.rotation;
            //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x + m_rotate.x, transform.rotation.eulerAngles.y + m_rotate.y, transform.rotation.eulerAngles.z + m_rotate.z);
            //Quaternion tarRot = Quaternion.Euler(transform.localRotation.eulerAngles.x + m_rotate.x, transform.localRotation.eulerAngles.y + m_rotate.y, transform.localRotation.eulerAngles.z + m_rotate.z);

            if (m_debug)
            {
                Debug.Log(gameObject.name + " starting rotation! ; transform.rotation == " + transform.rotation.eulerAngles.ToString() + " ; tarRot == " + tarRot.eulerAngles.ToString());
                //Debug.Log(gameObject.name + " starting rotation! ; transform.localRotation == " + transform.localRotation.eulerAngles.ToString() + " ; tarRot == " + tarRot.eulerAngles.ToString());
            }            

            StartCoroutine(RotateWholeThing(tarRot));
        }
	}

    IEnumerator RotateWholeThing (Quaternion tarRot)
    {
        m_rotating = true;

        while (transform.rotation != tarRot)
        //while (transform.localRotation != tarRot)
        {
            RotateWholeCubeRotate(tarRot);
            yield return null;
        }

        //Delay next rotation
        yield return new WaitForSeconds(1.0f);

        m_rotating = false;

        yield return null;
    }

    //int DEBUG_loopCount = 0;
    void RotateWholeCubeRotate(Quaternion tarRot)
    {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, 5.0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, 0.1f);
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, tarRot, 0.1f);
        //transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, tarRot, 0.1f);

        //DEBUG_loopCount++;

        //if (Vector3.Dot(transform.rotation * Vector3.forward, tarRot * Vector3.forward) >= 0.99f)
        if (Quaternion.Angle(transform.rotation, tarRot) <= 1.0f)
        {
            transform.rotation = tarRot;
            //transform.localRotation = tarRot;

            //Debug.Log("RotateWholeCubeRotate() DEBUG_loopCount == " + DEBUG_loopCount.ToString());
            //DEBUG_loopCount = 0;
        }
    }
}
