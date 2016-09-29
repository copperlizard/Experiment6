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
        
	}

    public void Turn(GameObject touched, Vector2 move)
    {
        // Find move direction... 
        float movedX = Vector2.Dot(move.normalized, Vector2.right);
        float movedY = Vector2.Dot(move.normalized, Vector2.up);
        // Deadzone approach; Maybe change this to a simple more x or more y comparison (would "eliminate" diagonals; allowing for fewer condition checks; would require an 2 Mathf.Abs() calls)...
        bool movedLeft = false, movedRight = false, movedUp = false, movedDown = false;
        if (movedX >= 0.85f)
        {
            movedRight = true;
        }
        else if (movedX <= -0.85f)
        {
            movedLeft = true;
        }
        else if (movedY >= 0.85f)
        {
            movedUp = true;
        }
        else if (movedY <= -0.85f)
        {
            movedDown = true;
        }        
        ///

        // Front/top/bottom or side...
        float upCheck, forwardCheck, rightCheck;
        upCheck = Vector3.Dot(touched.transform.up, Vector3.up);
        //upCheck = Vector3.Dot(transform.InverseTransformVector(touched.transform.up), Vector3.up);
        forwardCheck = Vector3.Dot(touched.transform.up, Vector3.forward);
        //forwardCheck = Vector3.Dot(transform.InverseTransformVector(touched.transform.up), Vector3.forward);
        rightCheck = Vector3.Dot(touched.transform.up, Vector3.right);
        //rightCheck = Vector3.Dot(transform.InverseTransformVector(touched.transform.up), Vector3.right);

        bool front = false, top = false, bottom = false, left = false, right = false;
        if (upCheck >= 0.35f)
        {
            top = true;
        }
        else if (upCheck <= -0.35f)
        {
            bottom = true;
        }
        else if (forwardCheck <= -0.9f)
        {
            front = true;
        }        
        else if (rightCheck >= 0.35f)
        {
            right = true;
        }
        else if (rightCheck <= -0.35f)
        {
            left = true;
        }
        
        Debug.Log("movedLeft == " + movedLeft.ToString() + " ; movedRight == " + movedRight.ToString() + " ; movedUp == " + movedUp.ToString() + " ; movedDown == " + movedDown + System.Environment.NewLine +
            "touched.transform.up == " + touched.transform.up.ToString() + System.Environment.NewLine +
            "upCheck == " + upCheck.ToString() + " ; forwardCheck == " + forwardCheck.ToString() + " ; rightCheck == " + rightCheck.ToString() + System.Environment.NewLine +
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

        // Which direction did user input?
        if (movedLeft)
        {
            Quaternion tarRot = Quaternion.Euler(0.0f, 90.0f, 0.0f) * transform.localRotation;

            if (top)
            {
                tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * transform.localRotation;
            }
            else if (bottom)
            {
                tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * transform.localRotation;
            }

            //Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

            while (transform.localRotation != tarRot)
            {
                RotateWholeCubeRotate(tarRot);
                yield return null;
            }
        }
        else if (movedRight)
        {
            Quaternion tarRot = Quaternion.Euler(0.0f, -90.0f, 0.0f) * transform.localRotation;

            if (top)
            {
                tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * transform.localRotation;
            }
            else if (bottom)
            {
                tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * transform.localRotation;
            }

            //Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

            while (transform.localRotation != tarRot)
            {
                RotateWholeCubeRotate(tarRot);
                yield return null;
            }
        }
        else if (movedUp)
        {
            Quaternion tarRot = Quaternion.Euler(90.0f, 0.0f, 0.0f) * transform.localRotation;

            if (left)
            {

            }
            else if (right)
            {

            }            

            //Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

            while (transform.localRotation != tarRot)
            {
                RotateWholeCubeRotate(tarRot);
                yield return null;
            }
        }
        else if (movedDown)
        {
            Quaternion tarRot = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * transform.localRotation;

            if (left)
            {

            }
            else if (right)
            {

            }

            //Debug.Log("tarRot == " + tarRot.ToString() + " or " + tarRot.eulerAngles.ToString());

            while (transform.localRotation != tarRot)
            {
                RotateWholeCubeRotate(tarRot);
                yield return null;
            }
        }

        m_rotatingCube = false;
        yield return null;
    }

    //int DEBUG_loopCount = 0;
    void RotateWholeCubeRotate (Quaternion tarRot)
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, tarRot, 0.1f);
        
        //DEBUG_loopCount++;
        if (Quaternion.Angle(transform.localRotation, tarRot) <= 1.0f)
        {
            transform.localRotation = tarRot;

            //Debug.Log("RotateWholeCubeRotate() DEBUG_loopCount == " + DEBUG_loopCount.ToString());
            //DEBUG_loopCount = 0;            
        }        
    }

    IEnumerator RotateCubeFace (bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        m_rotatingCube = true;

        m_rotatingCube = false;
        yield return null;
    }
}
