using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VRubiksCubeUserInput))]
public class VRubiksCubeController : MonoBehaviour
{
    private VRubiksCubeUserInput m_userInput;

    private bool m_rotatingCube = false;

	// Use this for initialization
	void Start ()
    {
        m_userInput = GetComponent<VRubiksCubeUserInput>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        GyroInput();
	}

    public void GyroInput ()
    {
        // Cube tilt in response to phone gyro

        if (!m_rotatingCube)
        {

        }
    }

    public void Turn(GameObject touched, Vector2 move)
    {
        // Find move direction... 
        float movedX = Vector2.Dot(move.normalized, Vector2.right);
        float movedY = Vector2.Dot(move.normalized, Vector2.up);
        // Deadzone approach; Maybe change this to a simple more x or more y comparison (would "eliminate" diagonals; allowing for fewer condition checks; would require an 2 Mathf.Abs() calls)...
        bool movedLeft = false, movedRight = false, movedUp = false, movedDown = false;
        if (movedX >= 0.8f)
        {
            movedRight = true;
        }
        else if (movedX <= -0.8f)
        {
            movedLeft = true;
        }
        else if (movedY >= 0.8f)
        {
            movedUp = true;
        }
        else if (movedY <= -0.8f)
        {
            movedDown = true;
        }        
        ///

        // Front/top/bottom or side...
        float upCheck, forwardCheck, rightCheck;
        upCheck = Vector3.Dot(touched.transform.up, Vector3.up);
        forwardCheck = Vector3.Dot(touched.transform.up, Vector3.forward);
        rightCheck = Vector3.Dot(touched.transform.up, Vector3.right);
        
        bool front = false, top = false, bottom = false, left = false, right = false;
        if (upCheck >= 0.9f)
        {
            top = true;
        }
        else if (upCheck <= -0.9f)
        {
            bottom = true;
        }
        else if (forwardCheck <= -0.9f)
        {
            front = true;
        }
        else if (rightCheck >= 0.9f)
        {
            right = true;
        }
        else if (rightCheck <= -0.9f)
        {
            left = true;
        }

        //Debug.Log("movedLeft == " + movedLeft.ToString() + " ; movedRight == " + movedRight.ToString() + " ; movedUp == " + movedUp.ToString() + " ; movedDown == " + movedDown);
        //Debug.Log("touched.transform.up == " + touched.transform.up.ToString());
        //Debug.Log("upCheck == " + upCheck.ToString() + " ; forwardCheck == " + forwardCheck.ToString() + " ; rightCheck == " + rightCheck.ToString());
        //Debug.Log("front == " + front.ToString() + " ; top == " + top.ToString() + " ; left == " + left.ToString() + " ; right == " + right.ToString());

        Debug.Log("movedLeft == " + movedLeft.ToString() + " ; movedRight == " + movedRight.ToString() + " ; movedUp == " + movedUp.ToString() + " ; movedDown == " + movedDown + System.Environment.NewLine +
            "touched.transform.up == " + touched.transform.up.ToString() + System.Environment.NewLine +
            /*"upCheck == " + upCheck.ToString() + " ; forwardCheck == " + forwardCheck.ToString() + " ; rightCheck == " + rightCheck.ToString() + System.Environment.NewLine +*/
            "front == " + front.ToString() + " ; top == " + top.ToString() + " ; left == " + left.ToString() + " ; right == " + right.ToString());        

        // Center touch...
        if (touched.transform.parent.gameObject.tag == "center")
        {
            if (!m_rotatingCube)
            {
                StartCoroutine(RotateWholeCube(front, top, bottom, left, right, movedLeft, movedRight, movedUp, movedDown));
            }               
        }
        else
        {
            if (!m_rotatingCube)
            {
                StartCoroutine(RotateCubeFace(front, top, bottom, left, right, movedLeft, movedRight, movedUp, movedDown));
            }
        }
    }

    IEnumerator RotateWholeCube (bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        m_rotatingCube = true;

        float orientationCheckX, orientationCheckY, orientationCheckZ;

        orientationCheckX = Vector3.Dot(transform.up, Vector3.right);
        orientationCheckY = Vector3.Dot(transform.up, Vector3.up);
        orientationCheckZ = Vector3.Dot(transform.up, Vector3.forward);

        bool xAligned = false, yAligned = false, zAlingned = false;

        if (orientationCheckX >= 0.9f || orientationCheckX <= -0.9)
        {
            Debug.Log("xAligned!");
            xAligned = true;
        }
        else if (orientationCheckY >= 0.9f || orientationCheckY <= -0.9f)
        {
            Debug.Log("yAligned!");
            yAligned = true;
        }
        else if (orientationCheckZ >= 0.9f || orientationCheckZ <= -0.9f)
        {
            Debug.Log("zAligned!");
            zAlingned = true;
        }
        else
        {
            Debug.Log("cube not aligned with any axis!!!");
        }


        // Which face was touched?
        if (front)
        {
            // Which direction did user input?
            if (movedLeft)
            {
                // Should rotate cube 90deg about world Y axis
                
                Vector3 axisCheck = new Vector3(90.0f, 90.0f, 90.0f);
                //axisCheck.x *= transform.up.x;
                //axisCheck.y *= transform.up.y;
                //axisCheck.z *= transform.up.z;
                
                if (xAligned)
                {
                    axisCheck.z *= -transform.up.x;
                    axisCheck.y *= transform.up.y;
                    axisCheck.x *= transform.up.z;
                }
                else if (yAligned)
                {
                    axisCheck.x *= transform.up.x;
                    axisCheck.y *= transform.up.y;
                    axisCheck.z *= transform.up.z;
                }
                else if (zAlingned)
                {
                    axisCheck.x *= transform.up.x;
                    axisCheck.y *= transform.up.y;
                    axisCheck.z *= -transform.up.z;
                }

                Quaternion tarRot = transform.rotation * Quaternion.Euler(axisCheck.x, axisCheck.y, axisCheck.z);

                //Quaternion tarRot = transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(0.0f, 90.0f, 0.0f) * transform.rotation;
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90.0f, transform.rotation.eulerAngles.z);

                Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }
            }            
            else if (movedRight)
            {
                Vector3 axisCheck = new Vector3(-90.0f, -90.0f, -90.0f);
                axisCheck.x *= transform.up.x;
                axisCheck.y *= transform.up.y;
                axisCheck.z *= transform.up.z;

                Quaternion tarRot = transform.rotation * Quaternion.Euler(axisCheck.x, axisCheck.y, axisCheck.z);

                //Quaternion tarRot = transform.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                //Quaternion tarRot = transform.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90.0f, transform.rotation.eulerAngles.z);

                Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }                
            }                        
            else if (movedUp)
            {
                Vector3 axisCheck = new Vector3(90.0f, 90.0f, 90.0f);
                axisCheck.x *= transform.right.x;
                axisCheck.y *= transform.right.y;
                axisCheck.z *= transform.right.z;

                Quaternion tarRot = transform.rotation * Quaternion.Euler(axisCheck.x, axisCheck.y, axisCheck.z);

                //Quaternion tarRot = transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x + 90.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }                
            }                        
            else if (movedDown)
            {
                Vector3 axisCheck = new Vector3(-90.0f, -90.0f, -90.0f);
                axisCheck.x *= transform.right.x;
                axisCheck.y *= transform.right.y;
                axisCheck.z *= transform.right.z;

                Quaternion tarRot = transform.rotation * Quaternion.Euler(axisCheck.x, axisCheck.y, axisCheck.z);

                //Quaternion tarRot = transform.rotation * Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = transform.rotation * Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x - 90.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }
            }            
        }
        else if (top)
        {

        }
        else if (bottom)
        {

        }
        else if (left)
        {

        }
        else if (right)
        {

        }

        m_rotatingCube = false;
        yield return null;
    }

    static int DEBUG_loopCount = 0;
    void RotateWholeCubeRotate (Quaternion tarRot)
    {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, 5.0f);

        //transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, 0.1f);
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, tarRot, 0.1f);

        DEBUG_loopCount++;
                
        //if (Vector3.Dot(transform.rotation * Vector3.forward, tarRot * Vector3.forward) >= 0.99f)
        if (Quaternion.Angle(transform.rotation, tarRot) <= 1.0f)
        {
            transform.rotation = tarRot;

            Debug.Log("RotateWholeCubeRotate() DEBUG_loopCount == " + DEBUG_loopCount.ToString());
            DEBUG_loopCount = 0;            
        }        
    }

    IEnumerator RotateCubeFace (bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        m_rotatingCube = true;

        m_rotatingCube = false;
        yield return null;
    }
}
