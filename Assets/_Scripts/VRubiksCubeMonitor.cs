using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRubiksCubeMonitor : MonoBehaviour
{
    public float m_percentComplete = 0;
    public int m_stage = 0; // Solved stages    

    private List<GameObject> m_cubes; // Does not include centers

    private GameObject m_whiteCenter, m_blueCenter, m_redCenter, m_orangeCenter, m_greenCenter, m_yellowCenter; // Center TouchPanels

    private Dictionary<Vector3, int> m_cubeMap = new Dictionary<Vector3, int>();
    private bool[] m_cubeStates = new bool[20];

    // Use this for initialization
    void Start ()
    {
        //Build cube map
        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, -1.0f), 0); //left bottom front
        m_cubeMap.Add(new Vector3(0.0f, -1.0f, -1.0f), 1); //middle bottom front
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, -1.0f), 2); //right bottom front

        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, 0.0f), 3); //left bottom middle
        //m_cubeMap.Add(new Vector3(0.0f, -1.0f, 0.0f), 0); //middle bottom middle *center
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, 0.0f), 4); //right bottom middle

        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, 1.0f), 5); //left bottom back
        m_cubeMap.Add(new Vector3(0.0f, -1.0f, 1.0f), 6); //middle bottom back
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, 1.0f), 7); //right bottom back

        m_cubeMap.Add(new Vector3(-1.0f, 0.0f, -1.0f), 8); //left middle front
        //m_cubeMap.Add(new Vector3(0.0f, 0.0f, -1.0f), 0); //middle middle front *center
        m_cubeMap.Add(new Vector3(1.0f, 0.0f, -1.0f), 9); //right middle front

        //m_cubeMap.Add(new Vector3(-1.0f, 0.0f, 0.0f), 0); //left middle middle *center
        //m_cubeMap.Add(new Vector3(0.0f, 0.0f, 0.0f), 0); //middle middle middle *base cube
        //m_cubeMap.Add(new Vector3(1.0f, 0.0f, 0.0f), 0); //right middle middle *center

        m_cubeMap.Add(new Vector3(-1.0f, 0.0f, 1.0f), 10); //left middle back 
        //m_cubeMap.Add(new Vector3(0.0f, 0.0f, 1.0f), 0); //middle middle back *center
        m_cubeMap.Add(new Vector3(1.0f, 0.0f, 1.0f), 11); //right middle back

        m_cubeMap.Add(new Vector3(-1.0f, 1.0f, -1.0f), 12); //left top front
        m_cubeMap.Add(new Vector3(0.0f, 1.0f, -1.0f), 13); //middle top front
        m_cubeMap.Add(new Vector3(1.0f, 1.0f, -1.0f), 14); //right top front

        m_cubeMap.Add(new Vector3(-1.0f, 1.0f, 0.0f), 15); //left top middle
        //m_cubeMap.Add(new Vector3(0.0f, 1.0f, 0.0f), 0); //middle top middle *center
        m_cubeMap.Add(new Vector3(1.0f, 1.0f, 0.0f), 16); //right top middle

        m_cubeMap.Add(new Vector3(-1.0f, 1.0f, 1.0f), 17); //left top back
        m_cubeMap.Add(new Vector3(0.0f, 1.0f, 1.0f), 18); //middle top back
        m_cubeMap.Add(new Vector3(1.0f, 1.0f, 1.0f), 19); //right top back

        for (int i = 0; i < m_cubeStates.Length; i++)
        {
            m_cubeStates[i] = false;
        }

        // Build cube list
        m_cubes = new List<GameObject>();
        GameObject[] gameObjs = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjs.Length; i++)
        {
            if (gameObjs[i].layer == LayerMask.NameToLayer("Cube"))
            {
                // Store center panels
                if (gameObjs[i].tag == "center")
                {
                    Transform panel = gameObjs[i].GetComponentInChildren<Transform>();

                    switch (panel.gameObject.tag)
                    {
                        case "white":
                            m_whiteCenter = panel.gameObject;
                            break;
                        case "blue":
                            m_blueCenter = panel.gameObject;
                            break;
                        case "red":
                            m_redCenter = panel.gameObject;
                            break;
                        case "orange":
                            m_orangeCenter = panel.gameObject;
                            break;
                        case "green":
                            m_greenCenter = panel.gameObject;
                            break;
                        case "yellow":
                            m_yellowCenter = panel.gameObject;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    m_cubes.Add(gameObjs[i]);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public bool CheckSolved ()
    {
        // For each cube, check if child panels are aligned with matching color center panels
        foreach (GameObject cube in m_cubes)
        {
            // Assume the cube is correctly located
            bool cubeLocated = true;
                        
            // Check panel alignment
            Transform[] panels = cube.GetComponentsInChildren<Transform>();
            foreach (Transform panel in panels)
            {
                float panelCheck = 0.0f;
                switch (panel.gameObject.tag)
                {
                    case "white":
                        panelCheck = Vector3.Dot(panel.up, m_whiteCenter.transform.up);
                        break;
                    case "blue":
                        panelCheck = Vector3.Dot(panel.up, m_blueCenter.transform.up);
                        break;
                    case "red":
                        panelCheck = Vector3.Dot(panel.up, m_redCenter.transform.up);
                        break;
                    case "orange":
                        panelCheck = Vector3.Dot(panel.up, m_orangeCenter.transform.up);
                        break;
                    case "green":
                        panelCheck = Vector3.Dot(panel.up, m_greenCenter.transform.up);
                        break;
                    case "yellow":
                        panelCheck = Vector3.Dot(panel.up, m_yellowCenter.transform.up);
                        break;
                    default:
                        break;
                }

                // If cube not correctly located
                if (panelCheck < 0.9f)
                {
                    cubeLocated = false;
                    break;
                }
            }

            // Prep to record cube state (there is probably a nicer way to do this...)
            Vector3 mapInput = Vector3.zero;
            if (cube.transform.localPosition.x < -0.5f)
            {
                mapInput.x = -1.0f;
            }
            else if (cube.transform.localPosition.x > 0.5f)
            {
                mapInput.x = 1.0f;
            }
            else
            {
                mapInput.x = 0.0f;
            }

            if (cube.transform.localPosition.y < -0.5f)
            {
                mapInput.y = -1.0f;
            }
            else if (cube.transform.localPosition.y > 0.5f)
            {
                mapInput.y = 1.0f;
            }
            else
            {
                mapInput.y = 0.0f;
            }

            if (cube.transform.localPosition.z < -0.5f)
            {
                mapInput.z = -1.0f;
            }
            else if (cube.transform.localPosition.z > 0.5f)
            {
                mapInput.z = 1.0f;
            }
            else
            {
                mapInput.z = 0.0f;
            }

            // Store cube state/s
            int stateIndex = -1;
            bool stateIndexFound = m_cubeMap.TryGetValue(mapInput, out stateIndex);
            if (stateIndexFound)
            {
                m_cubeStates[stateIndex] = cubeLocated;
            }
            else
            {
                Debug.Log("cube stateIndex not found!!!");
            }
        }
        
        return ReadCubeStates();
    }

    private bool ReadCubeStates ()
    {
        // Calculate percent complete
        int goodCubes = 0;
        foreach (bool cubeState in m_cubeStates)
        {
            if (cubeState)
            {
                goodCubes++;
            }
        }
        m_percentComplete = goodCubes / m_cubeStates.Length;

        // Determine cube stage...
        m_stage = 0; // Assume no completed stages

        // Check bottom up
        if (m_cubeStates[1] && m_cubeStates[3] && m_cubeStates[4] && m_cubeStates[6]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[5] && m_cubeStates[7]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[10] && m_cubeStates[11]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[13] && m_cubeStates[15] && m_cubeStates[16] && m_cubeStates[18]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[12] && m_cubeStates[14] && m_cubeStates[17] && m_cubeStates[19]) // Third layer complete
                        {
                            if (5 > m_stage)
                            {
                                m_stage = 5;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        // Check top down        
        if (m_cubeStates[13] && m_cubeStates[15] && m_cubeStates[16] && m_cubeStates[18]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[12] && m_cubeStates[14] && m_cubeStates[17] && m_cubeStates[19]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[10] && m_cubeStates[11]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[1] && m_cubeStates[3] && m_cubeStates[4] && m_cubeStates[6]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[5] && m_cubeStates[7]) // Third layer complete
                        {
                            if (5 > m_stage)
                            {
                                m_stage = 5;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        // Check left right               
        if (m_cubeStates[3] && m_cubeStates[8] && m_cubeStates[10] && m_cubeStates[15]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[0] && m_cubeStates[5] && m_cubeStates[12] && m_cubeStates[17]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[1] && m_cubeStates[6] && m_cubeStates[13] && m_cubeStates[18]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[4] && m_cubeStates[9] && m_cubeStates[11] && m_cubeStates[16]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[2] && m_cubeStates[7] && m_cubeStates[14] && m_cubeStates[19]) // Third layer complete
                        {
                            if (5 > m_stage)
                            {
                                m_stage = 5;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        // Check right left
        if (m_cubeStates[4] && m_cubeStates[9] && m_cubeStates[11] && m_cubeStates[16]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[2] && m_cubeStates[7] && m_cubeStates[14] && m_cubeStates[19]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[1] && m_cubeStates[6] && m_cubeStates[13] && m_cubeStates[18]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[3] && m_cubeStates[8] && m_cubeStates[10] && m_cubeStates[15]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[0] && m_cubeStates[5] && m_cubeStates[12] && m_cubeStates[17]) // Third layer complete
                        {
                            if (5 > m_stage)
                            {
                                m_stage = 5;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        // Check front back

        // Check back front

        return true;
    }
}
