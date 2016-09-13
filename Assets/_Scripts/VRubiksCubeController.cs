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
        if (movedX >= 0.5f)
        {
            movedRight = true;
        }
        else if (movedX <= -0.5f)
        {
            movedLeft = true;
        }

        if (movedY >= 0.5f)
        {
            movedUp = true;
        }
        else if (movedY <= -0.5f)
        {
            movedDown = true;
        }
        Debug.Log("movedLeft == " + movedLeft.ToString() + " ; movedRight == " + movedRight.ToString() + " ; movedUp == " + movedUp.ToString() + " ; movedDown == " + movedDown);
        ///

        // Front/top/bottom or side...
        float upCheck, forwardCheck, rightCheck;
        upCheck = Vector3.Dot(touched.transform.up, Vector3.up);
        forwardCheck = Vector3.Dot(touched.transform.up, Vector3.forward);
        rightCheck = Vector3.Dot(touched.transform.up, Vector3.right);

        Debug.Log("touched.transform.up == " + touched.transform.up.ToString());
        Debug.Log("upCheck == " + upCheck.ToString() + " ; forwardCheck == " + forwardCheck.ToString() + " ; rightCheck == " + rightCheck.ToString());

        /*
        bool front = false, side = false;
        //if (Mathf.Abs(upCheck) >= 0.9f || Mathf.Abs(forwardCheck) >= 0.9f)
        if ((upCheck >= 0.9f || upCheck <= -0.9f) || (forwardCheck >= 0.9f || forwardCheck <= -0.9f))
        {
            front = true;
        }
        //else if (Mathf.Abs(rightCheck) >= 0.9f)
        else if (rightCheck >= 0.9f || rightCheck <= -0.9f)
        {
            side = true;
        }

        Debug.Log("front/top/bottom == " + front.ToString() + " ; side == " + side.ToString());
        */

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

        Debug.Log("front == " + front.ToString() + " ; top == " + top.ToString() + " ; left == " + left.ToString() + " ; right == " + right.ToString());
        

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

        if (front)
        {
            if (movedLeft)
            {
                Debug.Log("Rotating whole cube left");

                float tarAng = transform.rotation.eulerAngles.y + 90.0f;

                while (transform.rotation.eulerAngles.y != tarAng)
                {
                    float dif = tarAng - transform.rotation.eulerAngles.y;

                    float ang = Mathf.Lerp(0.0f, dif, 0.1f);                    

                    transform.Rotate(Vector3.up, ang);

                    yield return null;
                }
            }
            /*
            else if (movedRight)
            {
                Debug.Log("Rotating whole cube right");

                Quaternion tarRot = transform.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90.0f, transform.rotation.eulerAngles.z);

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }
            }            
            else if (movedUp)
            {
                Debug.Log("Rotating whole cube up");

                Quaternion tarRot = transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x + 90.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }                
            }            
            else if (movedDown)
            {
                Debug.Log("Rotating whole cube down");

                Quaternion tarRot = transform.rotation * Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                //Quaternion tarRot = Quaternion.Euler(transform.rotation.eulerAngles.x - 90.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                while (transform.rotation != tarRot)
                {
                    RotateWholeCubeRotate(tarRot);
                    yield return null;
                }
            }
            */
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

    //void RotateWholeCubeRotate (Quaternion tarRot)
    void RotateWholeCubeRotate (Vector3 axis, float ang)
    {
        //REWRITE THIS TO USE TARGET EULER ANGLES AND A SPECIFIED AXIS OF ROTATION





        /*
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, 0.1f);
        if (Vector3.Dot(transform.rotation * Vector3.forward, tarRot * Vector3.forward) >= 0.99f)
        {
            transform.rotation = tarRot;            
        }
        */
    }

    IEnumerator RotateCubeFace (bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        m_rotatingCube = true;

        m_rotatingCube = false;
        yield return null;
    }
}
