using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(VRubiksCubeUserInput))]
[RequireComponent(typeof(VRubiksCubeMonitor))]
public class VRubiksCubeController : MonoBehaviour
{
    public float m_cubeRotateSpeed, m_faceRotateSpeed;

    [HideInInspector]
    public bool m_rotatingCube = false, m_rotatingFace = false;

    private VRubiksCubeUserInput m_userInput;

    private VRubiksCubeMonitor m_monitor;

    private List<GameObject> m_cubes, m_rotationGroup;

    private GameObject m_faceRotator;

	// Use this for initialization
	void Start ()
    {
        m_userInput = GetComponent<VRubiksCubeUserInput>();

        m_monitor = GetComponent<VRubiksCubeMonitor>();

        m_cubes = new List<GameObject>();
        m_rotationGroup = new List<GameObject>();

        GameObject[] gameObjs = FindObjectsOfType<GameObject>();

        for (int i = 0; i < gameObjs.Length; i++)
        {
            if (gameObjs[i].layer == LayerMask.NameToLayer("Cube"))
            {
                m_cubes.Add(gameObjs[i]);
            }
        }

        //Debug.Log("m_cubes.count == " + m_cubes.Count.ToString());

        m_faceRotator = new GameObject();
        m_faceRotator.transform.position = transform.parent.position;
        m_faceRotator.transform.rotation = transform.parent.rotation;
        m_faceRotator.transform.parent = transform.parent;                
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void Turn(GameObject touched, Vector2 move)
    {
        m_monitor.m_turns++;

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
        Vector3 unTilted = Quaternion.Inverse(transform.parent.rotation) * touched.transform.up;
        upCheck = Vector3.Dot(unTilted, Vector3.up);
        forwardCheck = Vector3.Dot(unTilted, Vector3.forward);
        rightCheck = Vector3.Dot(unTilted, Vector3.right);                
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
        
        /*
        Debug.Log("movedLeft == " + movedLeft.ToString() + " ; movedRight == " + movedRight.ToString() + " ; movedUp == " + movedUp.ToString() + " ; movedDown == " + movedDown + System.Environment.NewLine +
            "touched.transform.up == " + touched.transform.up.ToString() + System.Environment.NewLine +
            "upCheck == " + upCheck.ToString() + " ; forwardCheck == " + forwardCheck.ToString() + " ; rightCheck == " + rightCheck.ToString() + System.Environment.NewLine +
            "front == " + front.ToString() + " ; top == " + top.ToString() + " ; left == " + left.ToString() + " ; right == " + right.ToString());
        */

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
                StartCoroutine(RotateCubeFace(touched, front, top, bottom, left, right, movedLeft, movedRight, movedUp, movedDown));
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
                tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * transform.localRotation;
            }
            else if (right)
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
        else if (movedDown)
        {
            Quaternion tarRot = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * transform.localRotation;

            if (left)
            {
                tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * transform.localRotation;
            }
            else if (right)
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

        m_rotatingCube = false;
        yield return null;
    }
        
    void RotateWholeCubeRotate (Quaternion tarRot)
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, tarRot, m_cubeRotateSpeed * Time.deltaTime);
        
        if (Quaternion.Angle(transform.localRotation, tarRot) <= 1.0f)
        {
            transform.localRotation = tarRot;          
        }        
    }

    IEnumerator RotateCubeFace (GameObject touched, bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        m_rotatingCube = true;
        
        SelectCubeFace(touched, front, top, bottom, left, right, movedLeft, movedRight, movedUp, movedDown);
        
        if (movedLeft)
        {
            if (front || left || right)
            {
                // Group rotates CW around Y
                Quaternion tarRot = Quaternion.Euler(0.0f, 90.0f, 0.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (top)
            {
                // Group rotates CCW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (bottom)
            {
                // Group rotates CW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }                      
        }
        else if (movedRight)
        {
            if (front || left || right)
            {
                // Group rotates CCW around Y
                Quaternion tarRot = Quaternion.Euler(0.0f, -90.0f, 0.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (top)
            {
                // Group rotates CW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (bottom)
            {
                // Group rotates CCW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }            
        }
        else if (movedUp)
        {
            if (front || top || bottom)
            {
                // Group rotates CW around X
                Quaternion tarRot = Quaternion.Euler(90.0f, 0.0f, 0.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }            
            else if (left)
            {
                // Group rotates CW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (right)
            {
                // Group rotates CCW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
        }
        else if (movedDown)
        {
            if (front || top || bottom)
            {
                // Group rotates CCW around x
                Quaternion tarRot = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }            
            else if (left)
            {
                // Group rotates CCW around Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, 90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
            else if (right)
            {
                // Group rotates CW arond Z
                Quaternion tarRot = Quaternion.Euler(0.0f, 0.0f, -90.0f) * m_faceRotator.transform.localRotation;

                do
                {
                    RotateCubeFaceRotate(tarRot);
                    yield return null;
                } while (m_rotatingFace);
            }
        }

        // Check cube status
        m_monitor.CheckSolved();        

        m_rotatingCube = false;
        yield return null;
    }

    void SelectCubeFace (GameObject touched, bool front, bool top, bool bottom, bool left, bool right, bool movedLeft, bool movedRight, bool movedUp, bool movedDown)
    {
        if (movedLeft || movedRight)
        {
            if (front || left || right)
            {
                // Use Y coord to select group
                //float tarCoord = touched.transform.parent.localPosition.y;
                float tarCoord = (transform.localRotation * touched.transform.parent.localPosition).y;

                //Debug.Log("tarCoords == " + tarCoord.ToString());

                m_rotationGroup.Clear();

                foreach (GameObject cube in m_cubes)
                {
                    float checkCoord = (transform.localRotation * cube.transform.localPosition).y;
                    //if (cube.transform.localPosition.y <= tarCoord + 0.1f && cube.transform.localPosition.y >= tarCoord - 0.1f)
                    if (checkCoord <= tarCoord + 0.1f && checkCoord >= tarCoord - 0.1f)
                    {
                        m_rotationGroup.Add(cube);
                    }
                }

                //Debug.Log("m_rotationGroup.count == " + m_rotationGroup.Count.ToString());
            }
            else if (top || bottom)
            {
                // Use Z coord to select group                
                //float tarCoord = touched.transform.parent.localPosition.z;
                float tarCoord = (transform.localRotation * touched.transform.parent.localPosition).z;

                //Debug.Log("tarCoords == " + tarCoord.ToString());

                m_rotationGroup.Clear();

                foreach (GameObject cube in m_cubes)
                {
                    float checkCoord = (transform.localRotation * cube.transform.localPosition).z;
                    //if (cube.transform.localPosition.z <= tarCoord + 0.1f && cube.transform.localPosition.z >= tarCoord - 0.1f)
                    if (checkCoord <= tarCoord + 0.1f && checkCoord >= tarCoord - 0.1f)
                    {
                        m_rotationGroup.Add(cube);
                    }
                }
            }
        }
        else if (movedUp || movedDown)
        {
            if (front || top || bottom)
            {
                // Use X coord to select group
                //float tarCoord = touched.transform.parent.localPosition.x;
                float tarCoord = (transform.localRotation * touched.transform.parent.localPosition).x;

                //Debug.Log("tarCoords == " + tarCoord.ToString());

                m_rotationGroup.Clear();

                foreach (GameObject cube in m_cubes)
                {
                    float checkCoord = (transform.localRotation * cube.transform.localPosition).x;
                    //if (cube.transform.localPosition.x <= tarCoord + 0.1f && cube.transform.localPosition.x >= tarCoord - 0.1f)
                    if (checkCoord <= tarCoord + 0.1f && checkCoord >= tarCoord - 0.1f)
                    {
                        m_rotationGroup.Add(cube);
                    }
                }
            }
            else if (left || right)
            {
                // Use Z coord to select group
                //float tarCoord = touched.transform.parent.localPosition.z;
                float tarCoord = (transform.localRotation * touched.transform.parent.localPosition).z;

                //Debug.Log("tarCoords == " + tarCoord.ToString());

                m_rotationGroup.Clear();

                foreach (GameObject cube in m_cubes)
                {
                    float checkCoord = (transform.localRotation * cube.transform.localPosition).z;
                    //if (cube.transform.localPosition.z <= tarCoord + 0.1f && cube.transform.localPosition.z >= tarCoord - 0.1f)
                    if (checkCoord <= tarCoord + 0.1f && checkCoord >= tarCoord - 0.1f)
                    {
                        m_rotationGroup.Add(cube);
                    }
                }
            }
        }
    }

    void RotateCubeFaceRotate (Quaternion tarRot)
    {
        // Set rotation group parent transform on first call
        if (!m_rotatingFace)
        {
            foreach (GameObject cube in m_rotationGroup)
            {
                cube.transform.parent = m_faceRotator.transform;
            }

            m_rotatingFace = true;
        }               

        // Rotate
        m_faceRotator.transform.localRotation = Quaternion.Slerp(m_faceRotator.transform.localRotation, tarRot, m_faceRotateSpeed * Time.deltaTime);
        
        if (Quaternion.Angle(m_faceRotator.transform.localRotation, tarRot) <= 1.0f)
        {
            // Finalize rotation
            m_faceRotator.transform.localRotation = tarRot;

            // Re-parent cubes
            foreach (GameObject cube in m_rotationGroup)
            {
                cube.transform.parent = transform;
            }            

            m_rotatingFace = false;                        
        }
    }
}