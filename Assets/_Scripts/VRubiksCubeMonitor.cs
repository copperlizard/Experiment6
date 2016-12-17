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

    private bool m_executingMoves = false; // Set when executing move set with coroutine

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

    private Vector3 PrepareMapInput(GameObject cube)
    {
        // Prep to record cube state (there is probably a nicer way to do this...)
        //Vector3 mapInput = Vector3.zero;
        Vector3 mapInput = transform.localRotation * cube.transform.localPosition;
        //if (cube.transform.localPosition.x < -0.5f)
        if (mapInput.x < -0.5f)
        {
            mapInput.x = -1.0f;
        }
        //else if (cube.transform.localPosition.x > 0.5f)
        else if (mapInput.x > 0.5f)
        {
            mapInput.x = 1.0f;
        }
        else
        {
            mapInput.x = 0.0f;
        }

        //if (cube.transform.localPosition.y < -0.5f)
        if (mapInput.y < -0.5f)
        {
            mapInput.y = -1.0f;
        }
        //else if (cube.transform.localPosition.y > 0.5f)
        else if (mapInput.y > 0.5f)
        {
            mapInput.y = 1.0f;
        }
        else
        {
            mapInput.y = 0.0f;
        }

        //if (cube.transform.localPosition.z < -0.5f)
        if (mapInput.z < -0.5f)
        {
            mapInput.z = -1.0f;
        }
        //else if (cube.transform.localPosition.z > 0.5f)
        else if (mapInput.z > 0.5f)
        {
            mapInput.z = 1.0f;
        }
        else
        {
            mapInput.z = 0.0f;
        }

        return mapInput;
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

            Vector3 mapInput = PrepareMapInput(cube);            

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
        Debug.Log("Solving Cube!");

        StartCoroutine(SolveCubeCoRoutine());        
    }

    IEnumerator SolveCubeCoRoutine()
    {
        CheckSolved();

        Debug.Log("solving stage1!");
        while (!SolveCubeStage1())
        {
            yield return null;
        }
        Debug.Log("done solving stage1!");


        Debug.Log("done solving cube!");
        yield return null;
    }

    IEnumerator ExecuteMoves (UserInput[] moves) 
    {
        m_executingMoves = true;

        Debug.Log("hello?");

        foreach (UserInput move in moves)
        {
            while (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube) //wait for last move to finish
            {
                Debug.Log("finishing move!");
                yield return null;
            }

            Debug.Log("executing move -> loc == " + move.m_touchedParentLocation.ToString() + " ; up == " + move.m_touchedUp.ToString() + " ; dir == " + move.m_move);

            foreach (GameObject cube in m_cubes)
            {
                Vector3 mapInput = PrepareMapInput(cube);

                int stateIndex = -1;
                bool stateIndexFound = m_cubeMap.TryGetValue(mapInput, out stateIndex);
                if (stateIndexFound)
                {
                    if (stateIndex == move.m_touchedParentLocation)
                    {
                        // Correct cube
                        GameObject tarPanel = null;
                        for (int i = 0; i < cube.transform.childCount; i++)
                        {
                            if (Vector3.Dot(cube.transform.GetChild(i).transform.up, move.m_touchedUp) >= 0.9f)
                            {
                                tarPanel = cube.transform.GetChild(i).gameObject;
                            }                            
                        }

                        if (tarPanel != null)
                        {
                            Debug.Log("TURN!");
                            m_cubeController.Turn(tarPanel, move.m_move); 
                        }
                        else
                        {
                            Debug.Log("could not find tarPanel!");
                        }

                        break;
                    }
                }
                else
                {
                    Debug.Log("cube stateIndex not found!!!");
                }
            }
        }
        
        m_executingMoves = false;
        yield return null;
    }
        
    private bool SolveCubeStage1 () 
    {
        // Wait for last controller input to be processed
        if (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube || m_executingMoves)
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

        //Debug.Log("U == " + U.ToString() + " ; D == " + D.ToString() + " ; L == " + L.ToString() + " ; R == " + R.ToString() + " ; F == " + F.ToString() + " ; B == " + B.ToString());

        // Orient cube so cross is on top (if neccessary)
        if (U >= L && U >= R && U >= F && U >= B && U >= D && U != 0)
        {
            // Already on top
            Debug.Log("Already on top!");
        }
        else if (L >= U && L >= R && L >= F && L >= B && L >= D && L != 0)
        {
            Debug.Log("Left face cross is most complete!");
            
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

            //return false; // Recurse
        }
        else if (R >= U && R >= L && R >= F && R >= B && R >= D && R != 0)
        {
            Debug.Log("Right face cross is most complete!");
            
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

            //return false; // Recurse
        }        
        else if (F >= U && F >= R && F >= L && F >= B && F >= D && F != 0)
        {
            Debug.Log("Front face cross is most complete!");
            
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

            //return false; // Recurse
        }
        else if (B >= U && B >= R && B >= L && B >= F && F >= D && B != 0)
        {
            Debug.Log("Back face cross is most complete!");
            
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

            //return false; // Recurse

        }
        else if (D >= U && D >= R && D >= L && D >= F && D >= B && D != 0)
        {
            Debug.Log("Down face cross is most complete!");
            
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
            
            //return false; // Recurse
        }
        else
        {
            Debug.Log("Failed to find most complete cross!");

            Stage1Flail();

            return false;
        }        

        // Complete cross (if neccessary)
        return SolveStage1Cross();
    }

    private int m_flails = 0;
    private bool m_flailed = false;
    private void Stage1Flail ()
    {
        if (!m_flailed)
        {
            m_cubeController.Turn((m_flails % 2 == 0) ? m_whiteCenter : m_redCenter, (m_flails % 2 == 0) ? new Vector2(0.0f, 300.0f): new Vector2(300.0f, 0.0f));
            m_flails++;
        }
        else
        {
            foreach (GameObject cube in m_cubes)
            {
                Vector3 mapInput = PrepareMapInput(cube);
                                
                int stateIndex = -1;
                bool stateIndexFound = m_cubeMap.TryGetValue(mapInput, out stateIndex);
                if (stateIndexFound)
                {
                    if (stateIndex == 14)
                    {
                        m_cubeController.Turn(cube.transform.GetChild(0).gameObject, new Vector2(0.0f, 300.0f));
                    }
                }
                else
                {
                    Debug.Log("cube stateIndex not found!!!");
                }
            }
        }
        
        m_flailed = !m_flailed;
    }
    
    private bool SolveStage1Cross ()
    {
        Debug.Log("SolvingStage1Cross()");

        if (m_cubeStates[13] && m_cubeStates[15] && m_cubeStates[16] && m_cubeStates[18]) // Top Cross complete
        {
            return true;
        }
        else
        {
            if (!m_cubeStates[16]) // Solve top right edge
            {
                // Determine Target panels colors
                bool white = false, blue = false, red = false, orange = false, green = false, yellow = false;
                
                if (Vector3.Dot(m_whiteCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_whiteCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    white = true;
                }

                if (Vector3.Dot(m_blueCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_blueCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    blue = true;
                }

                if (Vector3.Dot(m_redCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_redCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    red = true;
                }

                if (Vector3.Dot(m_orangeCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_orangeCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    orange = true;
                }

                if (Vector3.Dot(m_greenCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_greenCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    green = true;
                }

                if (Vector3.Dot(m_yellowCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_yellowCenter.transform.up, transform.parent.right) >= 0.9f)
                {
                    yellow = true;
                }

                string tarCubeName = "";
                if (white && blue)
                {
                    tarCubeName = "WB-Edge_Cube";
                }
                else if (white && red)
                {
                    tarCubeName = "WR-Edge_Cube";
                }
                else if (white && orange)
                {
                    tarCubeName = "WO-Edge_Cube";
                }
                else if (white && green)
                {
                    tarCubeName = "WG-Edge_Cube";
                }
                else if (yellow && blue)
                {
                    tarCubeName = "YB-Edge_Cube";
                }
                else if (yellow && red)
                {
                    tarCubeName = "YR-Edge_Cube";
                }
                else if (yellow && orange)
                {
                    tarCubeName = "YO-Edge_Cube";
                }
                else if (yellow && green)
                {
                    tarCubeName = "YG-Edge_Cube";
                }
                else if (orange && blue)
                {
                    tarCubeName = "BO-Edge_Cube";
                }
                else if (blue && red)
                {
                    tarCubeName = "RB-Edge_Cube";
                }
                else if (red && green)
                {
                    tarCubeName = "GR-Edge_Cube";
                }
                else if (green && orange)
                {
                    tarCubeName = "OG-Edge_Cube";
                }
                else
                {
                    Debug.Log("Error Identifying target edge cube for solving stage1 cross!");
                }

                GameObject tarEdgeCube = null;
                foreach(GameObject cube in m_cubes)
                {
                    if (cube.name == tarCubeName)
                    {
                        Debug.Log("tarEdgeCube found!");
                        tarEdgeCube = cube;
                    }
                }

                if (tarEdgeCube != null)
                {
                    Vector3 mapInput = PrepareMapInput(tarEdgeCube);

                    int stateIndex = -1;
                    bool stateIndexFound = m_cubeMap.TryGetValue(mapInput, out stateIndex);
                    if (stateIndexFound)
                    {
                        // DO A TURN/s BASED ON CURRENT STATE INDEX

                        switch(stateIndex)
                        {
                            case 13: // top front edge

                                // F -> F -> D -> R -> R
                                UserInput[] moves13 = new UserInput[5];

                                moves13[0].m_touchedParentLocation = 13;
                                moves13[0].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves13[0].m_move = new Vector2(300.0f, 0.0f);

                                moves13[1].m_touchedParentLocation = 13;
                                moves13[1].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves13[1].m_move = new Vector2(300.0f, 0.0f);

                                moves13[2].m_touchedParentLocation = 1;
                                moves13[2].m_touchedUp = -Vector3.forward; // MIGHT NEED TO CHANGE THIS!!!
                                moves13[2].m_move = new Vector2(300.0f, 0.0f);

                                moves13[3].m_touchedParentLocation = 16;
                                moves13[3].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves13[3].m_move = new Vector2(0.0f, 300.0f);

                                moves13[4].m_touchedParentLocation = 16;
                                moves13[4].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves13[4].m_move = new Vector2(0.0f, 300.0f);
                                
                                StartCoroutine(ExecuteMoves(moves13));
                                break;

                            case 15: // top left edge

                                // L -> L -> D -> D -> R -> R
                                UserInput[] moves15 = new UserInput[6];

                                moves15[0].m_touchedParentLocation = 15;
                                moves15[0].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[0].m_move = new Vector2(0.0f, 300.0f);

                                moves15[1].m_touchedParentLocation = 15;
                                moves15[1].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[1].m_move = new Vector2(0.0f, 300.0f);

                                moves15[2].m_touchedParentLocation = 1;
                                moves15[2].m_touchedUp = -Vector3.forward; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[2].m_move = new Vector2(300.0f, 0.0f);

                                moves15[3].m_touchedParentLocation = 1;
                                moves15[3].m_touchedUp = -Vector3.forward; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[3].m_move = new Vector2(300.0f, 0.0f);

                                moves15[4].m_touchedParentLocation = 16;
                                moves15[4].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[4].m_move = new Vector2(0.0f, 300.0f);

                                moves15[5].m_touchedParentLocation = 16;
                                moves15[5].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves15[5].m_move = new Vector2(0.0f, 300.0f);
                                
                                StartCoroutine(ExecuteMoves(moves15));
                                break;

                            case 18: // top back edge

                                // B -> B -> Di -> R -> R
                                UserInput[] moves18 = new UserInput[5];

                                moves18[0].m_touchedParentLocation = 18;
                                moves18[0].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves18[0].m_move = new Vector2(300.0f, 0.0f);

                                moves18[1].m_touchedParentLocation = 18;
                                moves18[1].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves18[1].m_move = new Vector2(300.0f, 0.0f);

                                moves18[2].m_touchedParentLocation = 1;
                                moves18[2].m_touchedUp = -Vector3.forward; // MIGHT NEED TO CHANGE THIS!!!
                                moves18[2].m_move = new Vector2(-300.0f, 0.0f);

                                moves18[3].m_touchedParentLocation = 16;
                                moves18[3].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves18[3].m_move = new Vector2(0.0f, 300.0f);

                                moves18[4].m_touchedParentLocation = 16;
                                moves18[4].m_touchedUp = Vector3.up; // MIGHT NEED TO CHANGE THIS!!!
                                moves18[4].m_move = new Vector2(0.0f, 300.0f);                                

                                StartCoroutine(ExecuteMoves(moves18));
                                break;

                            default:
                                Debug.Log("tarEdgeCube found in un-accounted position!");
                                return true; // END RECURSION!!!                                
                        }

                        return false;
                    }
                    else
                    {
                        Debug.Log("tarEdgeCube stateIndex not found!!!");

                        return true; // END RECURSION!!!
                    }
                }
                else
                {
                    Debug.Log("Error finding target edge cube in m_cubes!");

                    return true; // END RECURSION!!!
                }
            }
            else if (!m_cubeStates[13]) // rotate cube counter clockwise
            {                   
                if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_whiteCenter, new Vector2 (300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_blueCenter, new Vector2(300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_redCenter, new Vector2(300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_orangeCenter, new Vector2(300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_greenCenter, new Vector2(300.0f, 0.0f));
                }
                else //if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_yellowCenter, new Vector2(300.0f, 0.0f));
                }

                return false; // recurse
            }
            else // rotate cube clockwise
            {
                if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_whiteCenter, new Vector2(-300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_blueCenter, new Vector2(-300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_redCenter, new Vector2(-300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_orangeCenter, new Vector2(-300.0f, 0.0f));
                }
                else if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_greenCenter, new Vector2(-300.0f, 0.0f));
                }
                else //if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
                {
                    m_cubeController.Turn(m_yellowCenter, new Vector2(-300.0f, 0.0f));
                }

                return false; //recurse
            }
        }
    }
}


// Check bottom up
//if ([1] && [3] && [4] && [6]) // Bottom Cross complete
//if ([13] && [15] && [16] && [18]) // Top Cross complete
//if ([3] && [8] && [10] && [15]) // Left Cross complete
//if ([4] && [9] && [11] && [16]) // Right Cross complete
//if ([1] && [8] && [9] && [13]) // Front Cross complete
//if ([6] && [10] && [11] && [18]) // Back Cross complete