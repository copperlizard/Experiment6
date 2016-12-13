using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(VRubiksCubeController))] 
public class VRubiksCubeMonitor : MonoBehaviour
{
    public float m_percentComplete = 0;

    public int m_minTurns = 10, m_maxTurns = 20;
    
    [HideInInspector]
    public int m_stage = 0, m_turns = 0, m_cubePar = 0;  

    [HideInInspector]
    public Dictionary<Vector3, int> m_cubeMap;
    
    public bool m_randomizeOnStart = false;

    [HideInInspector]
    public bool m_randomizing = false;

    private VRubiksCubeController m_cubeController;
    
    private List<GameObject> m_cubes, m_touchPanels; // Do/es not include centers

    private GameObject m_whiteCenter, m_blueCenter, m_redCenter, m_orangeCenter, m_greenCenter, m_yellowCenter; // Center TouchPanels    
    
    private bool[] m_cubeStates = new bool[20];

    

    // Use this for initialization
    void Start ()
    {
        m_cubeController = GetComponent<VRubiksCubeController>();
        if (m_cubeController == null)
        {
            Debug.Log("VRubiksCubeController not found!!!");
        }

        //Build cube map
        m_cubeMap = new Dictionary<Vector3, int>();

        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, -1.0f), 0); //left bottom front
        m_cubeMap.Add(new Vector3(0.0f, -1.0f, -1.0f), 1); //middle bottom front
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, -1.0f), 2); //right bottom front

        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, 0.0f), 3); //left bottom middle
        m_cubeMap.Add(new Vector3(0.0f, -1.0f, 0.0f), 20); //middle bottom middle *center
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, 0.0f), 4); //right bottom middle

        m_cubeMap.Add(new Vector3(-1.0f, -1.0f, 1.0f), 5); //left bottom back
        m_cubeMap.Add(new Vector3(0.0f, -1.0f, 1.0f), 6); //middle bottom back
        m_cubeMap.Add(new Vector3(1.0f, -1.0f, 1.0f), 7); //right bottom back

        m_cubeMap.Add(new Vector3(-1.0f, 0.0f, -1.0f), 8); //left middle front
        m_cubeMap.Add(new Vector3(0.0f, 0.0f, -1.0f), 21); //middle middle front *center
        m_cubeMap.Add(new Vector3(1.0f, 0.0f, -1.0f), 9); //right middle front

        m_cubeMap.Add(new Vector3(-1.0f, 0.0f, 0.0f), 22); //left middle middle *center
        //m_cubeMap.Add(new Vector3(0.0f, 0.0f, 0.0f), 0); //middle middle middle *base cube
        m_cubeMap.Add(new Vector3(1.0f, 0.0f, 0.0f), 23); //right middle middle *center

        m_cubeMap.Add(new Vector3(-1.0f, 0.0f, 1.0f), 10); //left middle back 
        m_cubeMap.Add(new Vector3(0.0f, 0.0f, 1.0f), 24); //middle middle back *center
        m_cubeMap.Add(new Vector3(1.0f, 0.0f, 1.0f), 11); //right middle back

        m_cubeMap.Add(new Vector3(-1.0f, 1.0f, -1.0f), 12); //left top front
        m_cubeMap.Add(new Vector3(0.0f, 1.0f, -1.0f), 13); //middle top front
        m_cubeMap.Add(new Vector3(1.0f, 1.0f, -1.0f), 14); //right top front

        m_cubeMap.Add(new Vector3(-1.0f, 1.0f, 0.0f), 15); //left top middle
        m_cubeMap.Add(new Vector3(0.0f, 1.0f, 0.0f), 25); //middle top middle *center
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
        m_touchPanels = new List<GameObject>();
        GameObject[] gameObjs = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjs.Length; i++)
        {
            if (gameObjs[i].layer == LayerMask.NameToLayer("Cube"))
            {
                //Debug.Log("cube found ; name == " + gameObjs[i].name);

                // Store center panels
                if (gameObjs[i].tag == "center")
                {
                    Transform panel = gameObjs[i].transform.GetChild(0);

                    /*
                    Debug.Log("center cube found ; name == " + gameObjs[i].name + System.Environment.NewLine +
                        "touch panel name == " + panel.gameObject.name + System.Environment.NewLine +
                        "touch panel transform.up == " + panel.transform.up.ToString());
                    */

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
                    
                    Transform[] panels = new Transform[gameObjs[i].transform.childCount];
                    for (int j = 0; j < panels.Length; j++)
                    {
                        panels[j] = gameObjs[i].transform.GetChild(j);

                        if (panels[j] == null)
                        {
                            Debug.Log("error adding panels!");
                        }
                    }

                    foreach (Transform panel in panels)
                    {
                        m_touchPanels.Add(panel.gameObject);
                    }                                     
                }
            }
        }        

        /*
        Debug.Log("m_cubes.count == " + m_cubes.Count.ToString() + System.Environment.NewLine +
            "m_touchPanels.count == " + m_touchPanels.Count.ToString() + System.Environment.NewLine +
            "m_whiteCenter.name == " + m_whiteCenter.name + System.Environment.NewLine +
            "m_blueCenter.name == " + m_blueCenter.name + System.Environment.NewLine +
            "m_redCenter.name == " + m_redCenter.name + System.Environment.NewLine +
            "m_orangeCenter.name == " + m_orangeCenter.name + System.Environment.NewLine +
            "m_greenCenter.name == " + m_greenCenter.name + System.Environment.NewLine +
            "m_yellowCenter.name == " + m_yellowCenter.name + System.Environment.NewLine);
        */

        if (m_randomizeOnStart)
        {            
            RandomizeCube();
        }
        else
        {
            // Update cube state variables
            CheckSolved();
        }        
    }

    public void RandomizeCube()
    {
        //m_cubeSolved = false; 
        m_randomizing = true;
        StartCoroutine(Randomizing());        
    }

    IEnumerator Randomizing()
    {             
        Random.InitState((int)(Time.realtimeSinceStartup * 100.0f));
        m_cubeController.m_faceRotateSpeed *= 1000.0f;

        int turns = Random.Range(m_minTurns, m_maxTurns);
        //int turns2 = turns;
        m_cubePar = turns;

        while (turns > 0)
        {
            int panelI = Random.Range(0, m_touchPanels.Count - 1);

            Vector2 dir = (panelI % 2 == 0) ? new Vector2(1.0f, 0.0f) : new Vector2(0.0f, 1.0f);

            dir *= (((int)Time.time + panelI) % 2 == 0) ? 1.0f : -1.0f;

            m_cubeController.Turn(m_touchPanels[panelI], dir);

            while (m_cubeController.m_rotatingFace)
            {
                yield return null;
            }

            turns--;
            yield return null;
        }

        //m_turns -= turns2;
        m_turns -= m_cubePar;

        m_cubeController.m_faceRotateSpeed /= 1000.0f;

        m_randomizing = false;

        
        if (CheckSolved())
        {
            // Managed to randomly create solved cube ... consider adding a "cube complexity threshold" here if generated cubes seem to simple...
            RandomizeCube();
        }
        
        if (m_percentComplete > 0.6f) // Maybe adjust this later!!!
        {
            // Managed to randomly create a very simple cube...
            RandomizeCube();
        }

        yield return null;
    }

    public bool CheckSolved ()
    {
        if (m_randomizing)
        {
            return false;
        }

        // For each cube, check if child panels are aligned with matching color center panels
        foreach (GameObject cube in m_cubes)
        {
            // Assume the cube is correctly located
            bool cubeLocated = true;

            // Check panel alignment
            Transform[] panels = new Transform[cube.transform.childCount];
            for (int i = 0; i < cube.transform.childCount; i++)
            {
                panels[i] = cube.transform.GetChild(i);
            }

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

        //m_cubeSolved = ReadCubeStates();
        //return m_cubeSolved;
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
        m_percentComplete = (float)goodCubes / (float)m_cubeStates.Length;

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

                    if (m_cubeStates[12] && m_cubeStates[14] && m_cubeStates[17] && m_cubeStates[19]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[13] && m_cubeStates[15] && m_cubeStates[16] && m_cubeStates[18]) // Other cross complete
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

                    if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[5] && m_cubeStates[7]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[1] && m_cubeStates[3] && m_cubeStates[4] && m_cubeStates[6]) // Other cross complete
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

                    if (m_cubeStates[2] && m_cubeStates[7] && m_cubeStates[14] && m_cubeStates[19]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[4] && m_cubeStates[9] && m_cubeStates[11] && m_cubeStates[16]) // Other cross complete
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

                    if (m_cubeStates[0] && m_cubeStates[5] && m_cubeStates[12] && m_cubeStates[17]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[3] && m_cubeStates[8] && m_cubeStates[10] && m_cubeStates[15]) // Other cross complete
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
        if (m_cubeStates[1] && m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[13]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[12] && m_cubeStates[14]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[3] && m_cubeStates[4] && m_cubeStates[15] && m_cubeStates[16]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[5] && m_cubeStates[7] && m_cubeStates[17] && m_cubeStates[19]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[6] && m_cubeStates[10] && m_cubeStates[11] && m_cubeStates[18]) // Other cross complete
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

        // Check back front
        if (m_cubeStates[6] && m_cubeStates[10] && m_cubeStates[11] && m_cubeStates[18]) // Cross complete
        {
            if (1 > m_stage)
            {
                m_stage = 1;
            }

            if (m_cubeStates[5] && m_cubeStates[7] && m_cubeStates[17] && m_cubeStates[19]) // First layer complete
            {
                if (2 > m_stage)
                {
                    m_stage = 2;
                }

                if (m_cubeStates[3] && m_cubeStates[4] && m_cubeStates[15] && m_cubeStates[16]) // Second layer complete
                {
                    if (3 > m_stage)
                    {
                        m_stage = 3;
                    }

                    if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[12] && m_cubeStates[14]) // Third layer corners
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[1] && m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[13]) // Other cross complete
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

        //Debug.Log("m_percentComplete == " + m_percentComplete.ToString() + " ; m_stage == " + m_stage);

        return false;
    }
    
    public void SolveCube ()
    {
        while (!SolveCubeStage1())
        {
            Debug.Log("solving stage1!");
        }
    }   
    
    public bool SolveCubeStage1 () // recursive...
    {
        // Wait for last controller input to be processed
        if (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube)
        {
            return false;
        }

        // Find "most complete" cross (do this every step incase you get lucky...)
        int U = 0, D = 0, L = 0, R = 0, F = 0, B = 0;
        if (m_cubeStates[1])
        {
            D++;
            F++;
        }
        if (m_cubeStates[3])
        {
            D++;
            L++;
        }
        if (m_cubeStates[4])
        {
            D++;
            R++;
        }
        if (m_cubeStates[6])
        {
            D++;
            B++;
        }
        if (m_cubeStates[8])
        {
            L++;
            F++;
        }
        if (m_cubeStates[9])
        {
            R++;
            F++;
        }
        if (m_cubeStates[10])
        {
            L++;
            B++;
        }
        if (m_cubeStates[11])
        {
            R++;
            B++;
        }
        if (m_cubeStates[13])
        {
            U++;
            F++;
        }
        if (m_cubeStates[15])
        {
            U++;
            L++;
        }
        if (m_cubeStates[16])
        {
            U++;
            R++;
        }
        if (m_cubeStates[18])
        {
            U++;
            B++;
        }

        // Orient cube so cross is on top (if neccessary)
        if (U >= L && U >= R && U >= F && U >= B && U >= D)
        {
            // Already on top
        }
        else if (L >= U && L >= R && L >= F && L >= B && L >= D)
        {
            // Touch left center and swipe up
            if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_whiteCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_redCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_orangeCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_blueCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_greenCenter, new Vector2(0.0f, 300.0f));
            }
            else if(Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_yellowCenter, new Vector2(0.0f, 300.0f));
            }

            return false; // Recurse
        }
        else if (R >= U && R >= L && R >= F && R >= B && R >= D)
        {
            // Touch right center and swipe up
            if (Vector3.Dot(m_whiteCenter.transform.up, transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_whiteCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_redCenter.transform.up, transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_redCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_orangeCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_blueCenter.transform.up, transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_blueCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_greenCenter.transform.up, transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_greenCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_yellowCenter.transform.up, transform.parent.right) >= 0.9f)
            {
                m_cubeController.Turn(m_yellowCenter, new Vector2(0.0f, 300.0f));
            }

            return false; // Recurse
        }        
        else if (F >= U && F >= R && F >= L && F >= B && F >= D)
        {
            // Touch front center and swipe up
            if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_whiteCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_redCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_orangeCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_blueCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_greenCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_yellowCenter, new Vector2(0.0f, 300.0f));
            }

            return false; // Recurse
        }
        else if (B >= U && B >= R && B >= L && B >= F && F >= D)
        {
            // Touch front center and swipe down
            if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_whiteCenter, new Vector2(0.0f, -300.0f));
            }
            else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_redCenter, new Vector2(0.0f, -300.0f));
            }
            else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_orangeCenter, new Vector2(0.0f, -300.0f));
            }
            else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_blueCenter, new Vector2(0.0f, -300.0f));
            }
            else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_greenCenter, new Vector2(0.0f, -300.0f));
            }
            else if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_yellowCenter, new Vector2(0.0f, -300.0f));
            }

            return false; // Recurse

        }
        else if (D >= U && D >= R && D >= L && D >= F && D >= B)
        {
            // Touch front center and swipe up 
            if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_whiteCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_redCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_orangeCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_blueCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_greenCenter, new Vector2(0.0f, 300.0f));
            }
            else if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                m_cubeController.Turn(m_yellowCenter, new Vector2(0.0f, 300.0f));
            }

            return false; // Recurse
        }
        else
        {
            Debug.Log("Failed to find most complete cross!");
        }

        // Complete cross (if neccessary)
        return SolveStage1Cross();
    }
    
    private bool SolveStage1Cross ()
    {
        if (m_cubeStates[13] && m_cubeStates[15] && m_cubeStates[16] && m_cubeStates[18]) // Top Cross complete
        {
            return true;
        }
        else
        {
            // make sure [16] cubeState is false
        }

        return false;
    } 
}


// Check bottom up
//if ([1] && [3] && [4] && [6]) // Bottom Cross complete
//if ([13] && [15] && [16] && [18]) // Top Cross complete
//if ([3] && [8] && [10] && [15]) // Left Cross complete
//if ([4] && [9] && [11] && [16]) // Right Cross complete
//if ([1] && [8] && [9] && [13]) // Front Cross complete
//if ([6] && [10] && [11] && [18]) // Back Cross complete