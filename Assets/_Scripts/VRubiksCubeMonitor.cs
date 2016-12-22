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

    public void ReOrientCube(Vector3 panelUp, Vector2 swipe) 
    {
        // Touch left center and swipe up
        if (Vector3.Dot(m_whiteCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_whiteCenter, swipe);
        }
        else if (Vector3.Dot(m_redCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_redCenter, swipe);
        }
        else if (Vector3.Dot(m_orangeCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_orangeCenter, swipe);
        }
        else if (Vector3.Dot(m_blueCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_blueCenter, swipe);
        }
        else if (Vector3.Dot(m_greenCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_greenCenter, swipe);
        }
        else if (Vector3.Dot(m_yellowCenter.transform.up, panelUp) >= 0.9f)
        {
            m_cubeController.Turn(m_yellowCenter, swipe);
        }
        else
        {
            Debug.Log("could not find top center panel to re-orient cube!");
        }
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

        Debug.Log("solving stage2!");
        while (!SolveCubeStage2())
        {
            yield return null;
        }
        Debug.Log("done solving stage2!");

        Debug.Log("re-orienting!");
        ReOrientCube(transform.parent.up, new Vector2(0.0f, 300.0f));
        while (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube)
        {
            yield return null;
        }
        ReOrientCube(transform.parent.up, new Vector2(0.0f, 300.0f));
        while (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube)
        {
            yield return null;
        }

        Debug.Log("solving stage3!");
        while (!SolveCubeStage3())
        {
            yield return null;
        }
        Debug.Log("done solving stage3!");

        Debug.Log("done solving cube!");
        yield return null;
    }

    IEnumerator ExecuteMoves (UserInput[] moves) 
    {        
        m_executingMoves = true;
        
        foreach (UserInput move in moves)
        {
            while (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube) //wait for last move to finish
            {   
                yield return null;
            }

            //Debug.Log("executing move -> loc == " + move.m_touchedParentLocation.ToString() + " ; up == " + move.m_touchedUp.ToString() + " ; dir == " + move.m_move);

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
                            //Debug.Log("TURN!");
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
            //Debug.Log("Already on top!");
        }
        else if (L >= U && L >= R && L >= F && L >= B && L >= D && L != 0)
        {
            //Debug.Log("Left face cross is most complete!");

            // Rotate cube CW around Z
            ReOrientCube(transform.parent.up, new Vector2(300.0f, 0.0f));
        }
        else if (R >= U && R >= L && R >= F && R >= B && R >= D && R != 0)
        {
            //Debug.Log("Right face cross is most complete!");

            // Rotate CCW around Z
            ReOrientCube(transform.parent.up, new Vector2(-300.0f, 0.0f));
        }        
        else if (F >= U && F >= R && F >= L && F >= B && F >= D && F != 0)
        {
            //Debug.Log("Front face cross is most complete!");

            // Rotate cube CW around X
            ReOrientCube(transform.parent.up, new Vector2(0.0f, 300.0f));
        }
        else if (B >= U && B >= R && B >= L && B >= F && F >= D && B != 0)
        {
            //Debug.Log("Back face cross is most complete!");

            // Rotate cube CCW around X
            ReOrientCube(transform.parent.up, new Vector2(0.0f, -300.0f));
        }
        else if (D >= U && D >= R && D >= L && D >= F && D >= B && D != 0)
        {
            //Debug.Log("Down face cross is most complete!");

            // Rotate cube CCW around X
            ReOrientCube(transform.parent.up, new Vector2(0.0f, -300.0f));
        }
        else
        {
            Debug.Log("Failed to find most complete cross!");                  
        }        

        // Complete cross 
        return SolveStage1Cross();
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
                        //Debug.Log("tarEdgeCube found! name == " + cube.name);
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
                        //Debug.Log("stateIndex == " + stateIndex.ToString());

                        switch(stateIndex)
                        {
                            case 16: // top right edge

                                // Ri -> F -> D -> Fi -> R -> R
                                UserInput[] moves16 = new UserInput[6];

                                moves16[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                                moves16[1] = new UserInput(13, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves16[2] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves16[3] = new UserInput(13, transform.parent.up, new Vector2(-300.0f, 0.0f));
                                moves16[4] = new UserInput(16, transform.parent.up, new Vector3(0.0f, 300.0f));
                                moves16[5] = new UserInput(16, transform.parent.up, new Vector3(0.0f, 300.0f));

                                StartCoroutine(ExecuteMoves(moves16));
                                break;

                            case 13: // top front edge

                                // F -> F -> D -> R -> R
                                UserInput[] moves13 = new UserInput[5];
                                
                                moves13[0] = new UserInput(13, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves13[1] = new UserInput(13, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves13[2] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves13[3] = new UserInput(16, transform.parent.up, new Vector3(0.0f, 300.0f));
                                moves13[4] = new UserInput(16, transform.parent.up, new Vector3(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves13));
                                break;

                            case 15: // top left edge

                                // L -> L -> D -> D -> R -> R
                                UserInput[] moves15 = new UserInput[6];

                                moves15[0] = new UserInput(15, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves15[1] = new UserInput(15, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves15[2] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves15[3] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves15[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves15[5] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves15));
                                break;

                            case 18: // top back edge

                                // B -> B -> Di -> R -> R
                                UserInput[] moves18 = new UserInput[5];

                                moves18[0] = new UserInput(18, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves18[1] = new UserInput(18, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves18[2] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                                moves18[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves18[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves18));
                                break;

                            case 8: // left middle front edge

                                // Fi -> D -> F -> R -> R
                                UserInput[] moves8 = new UserInput[5];

                                moves8[0] = new UserInput(8, -transform.parent.right, new Vector2(0.0f, -300.0f));
                                moves8[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves8[2] = new UserInput(8, -transform.parent.right, new Vector2(0.0f, 300.0f));
                                moves8[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves8[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves8));
                                break;

                            case 9: // right middle front edge

                                // R
                                UserInput[] moves9 = new UserInput[1];

                                moves9[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves9));
                                break;

                            case 10: // left middle back edge

                                // Bi -> Di -> B -> R -> R
                                UserInput[] moves10 = new UserInput[5];

                                moves10[0] = new UserInput(18, transform.parent.up, new Vector2(-300.0f, 0.0f));
                                moves10[1] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                                moves10[2] = new UserInput(18, transform.parent.up, new Vector2(300.0f, 0.0f));
                                moves10[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves10[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves10));
                                break;

                            case 11: // right middle back edge

                                // Ri
                                UserInput[] moves11 = new UserInput[1];

                                moves11[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves11));
                                break;

                            case 1: // middle bottom front edge

                                // D -> R -> R
                                UserInput[] moves1 = new UserInput[3];

                                moves1[0] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves1[1] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves1[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves1));
                                break;

                            case 3: // left bottom middle edge

                                // D -> D -> R -> R
                                UserInput[] moves3 = new UserInput[4];

                                moves3[0] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves3[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                                moves3[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves3[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves3));
                                break;

                            case 4: // right bottom middle edge

                                // R -> R
                                UserInput[] moves4 = new UserInput[2];

                                moves4[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves4[1] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves4));
                                break;

                            case 6: //middle bottom back edge

                                // Di -> R -> R
                                UserInput[] moves6 = new UserInput[3];

                                moves6[0] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                                moves6[1] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                moves6[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                                
                                StartCoroutine(ExecuteMoves(moves6));
                                break;

                            default:
                                Debug.Log("tarEdgeCube found in un-accounted position! ");
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
            else if (!m_cubeStates[13]) // rotate cube counter clockwise around Y
            {
                ReOrientCube(-transform.parent.forward, new Vector2(300.0f, 0.0f));

                return false; // recurse
            }
            else // rotate cube clockwise around Y
            {
                ReOrientCube(-transform.parent.forward, new Vector2(-300.0f, 0.0f));
                
                return false; //recurse
            }
        }
    }

    private bool SolveCubeStage2 ()
    {
        // Wait for last controller input to be processed
        if (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube || m_executingMoves)
        {
            return false;
        }

        int dir = 1;
        if (m_cubeStates[14] && m_cubeStates[12] && m_cubeStates[17] && m_cubeStates[19])
        {
            return true;
        }
        //Ensure 14 is false
        else if (!m_cubeStates[14])
        {   
            // Determine Target panels colors
            bool white = false, blue = false, red = false, orange = false, green = false, yellow = false;
            if (Vector3.Dot(m_whiteCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_whiteCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                white = true;
            }

            if (Vector3.Dot(m_blueCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_blueCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                blue = true;
            }

            if (Vector3.Dot(m_redCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_redCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                red = true;
            }

            if (Vector3.Dot(m_orangeCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_orangeCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                orange = true;
            }

            if (Vector3.Dot(m_greenCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_greenCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                green = true;
            }

            if (Vector3.Dot(m_yellowCenter.transform.up, transform.parent.up) >= 0.9f || Vector3.Dot(m_yellowCenter.transform.up, transform.parent.right) >= 0.9f || Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f)
            {
                yellow = true;
            }

            string tarCubeName = "";
            if (white && blue && orange)
            {
                tarCubeName = "WBO-Corner_Cube";
            }
            else if (white && orange && green)
            {
                tarCubeName = "WOG-Corner_Cube";
            }
            else if (white && green && red)
            {
                tarCubeName = "WGR-Corner_Cube";
            }
            else if (white && red && blue)
            {
                tarCubeName = "WBR-Corner_Cube";
            }
            else if (yellow && blue && orange)
            {
                tarCubeName = "YBO-Corner_Cube";
            }
            else if (yellow && orange && green)
            {
                tarCubeName = "YGO-Corner_Cube";
            }
            else if (yellow && green && red)
            {
                tarCubeName = "YRG-Corner_Cube";
            }
            else if (yellow && red && blue)
            {
                tarCubeName = "YBR-Corner_Cube";
            }
            else
            {
                Debug.Log("Error Identifying target edge cube for solving stage2!");
            }

            GameObject tarEdgeCube = null;
            foreach (GameObject cube in m_cubes)
            {
                if (cube.name == tarCubeName)
                {
                    //Debug.Log("tarEdgeCube found! name == " + cube.name);
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
                    //Debug.Log("stateIndex == " + stateIndex.ToString());
                    
                    // DO A TURN/s BASED ON CURRENT STATE INDEX
                    switch (stateIndex)
                    {
                        case 0: // bottom front left

                            // Ri -> D -> R
                            UserInput[] moves0 = new UserInput[3];

                            moves0[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves0[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves0[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));

                            StartCoroutine(ExecuteMoves(moves0));                                                        
                            break;

                        case 2: // bottom front right

                            // Di -> Ri -> D -> R
                            UserInput[] moves2 = new UserInput[4];

                            moves2[0] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                            moves2[1] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves2[2] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves2[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));

                            StartCoroutine(ExecuteMoves(moves2));
                            break;

                        case 5: // bottom back left

                            // Ri -> D -> D -> R
                            UserInput[] moves5 = new UserInput[4];

                            moves5[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves5[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves5[2] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves5[3] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));

                            StartCoroutine(ExecuteMoves(moves5));
                            break;

                        case 7: // bottom back right

                            // D -> Ri -> Di -> Di -> R
                            UserInput[] moves7 = new UserInput[5];

                            moves7[0] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves7[1] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves7[2] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                            moves7[3] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                            moves7[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));

                            StartCoroutine(ExecuteMoves(moves7)); 
                            break;

                        case 12: // top front left 

                            // L -> D -> Li
                            UserInput[] moves12 = new UserInput[3];

                            moves12[0] = new UserInput(15, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves12[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves12[2] = new UserInput(15, transform.parent.up, new Vector2(0.0f, 300.0f));

                            StartCoroutine(ExecuteMoves(moves12));
                            break;

                        case 14: // top front right (correct position but wrong orientation)

                            // Ri -> Di -> R  -> D -> Ri -> Di -> R 
                            UserInput[] moves14 = new UserInput[7];

                            moves14[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f)); 
                            moves14[1] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f)); 
                            moves14[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                            moves14[3] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves14[4] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));
                            moves14[5] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                            moves14[6] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                            
                            StartCoroutine(ExecuteMoves(moves14));
                            break;

                        case 17: // top back left

                            // Li -> Di -> L
                            UserInput[] moves17 = new UserInput[3];

                            moves17[0] = new UserInput(15, transform.parent.up, new Vector2(0.0f, 300.0f));
                            moves17[1] = new UserInput(1, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                            moves17[2] = new UserInput(15, transform.parent.up, new Vector2(0.0f, -300.0f));

                            StartCoroutine(ExecuteMoves(moves17));
                            break;

                        case 19: // top back right

                            // R -> D -> Ri 
                            UserInput[] moves19 = new UserInput[3];

                            moves19[0] = new UserInput(16, transform.parent.up, new Vector2(0.0f, 300.0f));
                            moves19[1] = new UserInput(1, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                            moves19[2] = new UserInput(16, transform.parent.up, new Vector2(0.0f, -300.0f));

                            StartCoroutine(ExecuteMoves(moves19));
                            break;

                        default:
                            Debug.Log("tarEdgeCube found in un-accounted position! ");
                            return true; // END RECURSION!!!      
                    }

                    return false; // RECURSE!!!
                }
            }
        }
        //else if (!m_cubeStates[12] || !m_cubeStates[17])
        //{
            //dir = 1
        //}
        else if (!m_cubeStates[19])
        {
            dir = -1;
        }

        // Touch front center and swipe left or right
        float swipe = 300.0f * dir;
        ReOrientCube(-transform.parent.forward, new Vector2(swipe, 0.0f));
        
        return false; // RECURSE!!!
    }

    private bool SolveCubeStage3()
    {
        // Wait for last controller input to be processed
        if (m_cubeController.m_rotatingFace || m_cubeController.m_rotatingCube || m_executingMoves)
        {
            return false;
        }

        if (m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[10] && m_cubeStates[11])
        {
            return true;
        }
        else if (!m_cubeStates[8] || !m_cubeStates[9])
        {
            if (!m_cubeStates[8])
            {
                SolveCubeStage3Pos8();
            }
            else
            {
                SolveCubeStage3Pos9();
            }
            return false;
        }
        else if (m_cubeStates[8] && m_cubeStates[9] && !m_cubeStates[11])
        {
            ReOrientCube(-transform.parent.forward, new Vector2(-300.0f, 0.0f));
        }
        else
        {
            ReOrientCube(-transform.parent.forward, new Vector2(300.0f, 0.0f));
        }

        return false;
    }

    private bool SolveCubeStage3Pos8 ()
    {
        // Determine Target panels colors
        bool white = false, blue = false, red = false, orange = false, green = false, yellow = false;

        if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.right) >= 0.9f)
        {
            white = true;
        }

        if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_blueCenter.transform.up, -transform.parent.right) >= 0.9f)
        {
            blue = true;
        }

        if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_redCenter.transform.up, -transform.parent.right) >= 0.9f)
        {
            red = true;
        }

        if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.right) >= 0.9f)
        {
            orange = true;
        }

        if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_greenCenter.transform.up, -transform.parent.right) >= 0.9f)
        {
            green = true;
        }

        if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.right) >= 0.9f)
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
        foreach (GameObject cube in m_cubes)
        {
            if (cube.name == tarCubeName)
            {
                //Debug.Log("tarEdgeCube found! name == " + cube.name);
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
                //Debug.Log("stateIndex == " + stateIndex.ToString());

                // DO A TURN/s BASED ON CURRENT STATE INDEX
                switch (stateIndex)
                {
                    case 13: // front top middle

                        // Ui -> Li -> U -> L -> U -> F -> Ui -> Fi
                        UserInput[] moves13 = new UserInput[8];

                        moves13[0] = new UserInput(13, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                        moves13[1] = new UserInput(8, transform.parent.up, new Vector2(0.0f, 300.0f));
                        moves13[2] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                        moves13[3] = new UserInput(8, transform.parent.up, new Vector2(0.0f, -300.0f));
                        moves13[2] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                        moves13[5] = new UserInput(13, transform.parent.up, new Vector2(300.0f, 0.0f));
                        moves13[6] = new UserInput(13, -transform.parent.forward, new Vector2(300.0f, 0.0f));
                        moves13[7] = new UserInput(13, transform.parent.up, new Vector2(-300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves13));
                        break;

                    case 15: // left top middle

                        // Ui
                        UserInput[] moves15 = new UserInput[1];

                        moves15[0] = new UserInput(13, -transform.parent.forward, new Vector2(300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves15));
                        break;

                    case 16: // right top middle

                        // U
                        UserInput[] moves16 = new UserInput[1];

                        moves16[0] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves16));
                        break;

                    case 18: // back top middle

                        // U -> U
                        UserInput[] moves18 = new UserInput[2];

                        moves18[0] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                        moves18[1] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves18));
                        break;

                    case 9: // front right middle

                        // U -> R -> Ui -> Ri -> Ui -> Fi -> U -> F

                        break;

                    case 10: // left back middle

                        // Ui -> Bi -> U -> B -> U -> L -> Ui -> Li

                        break;

                    case 11: // right back middle

                        // U -> B -> Ui -> Bi -> Ui -> Ri -> U -> R

                        break;

                    default:
                        Debug.Log("tarEdgeCube found in un-accounted position! ");
                        return true; // END RECURSION!!!      
                }

                return false; // RECURSE!!!
            }
        }

        return false;
    }

    private bool SolveCubeStage3Pos9()
    {
        // Determine Target panels colors
        bool white = false, blue = false, red = false, orange = false, green = false, yellow = false;

        if (Vector3.Dot(m_whiteCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_whiteCenter.transform.up, transform.parent.right) >= 0.9f)
        {
            white = true;
        }

        if (Vector3.Dot(m_blueCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_blueCenter.transform.up, transform.parent.right) >= 0.9f)
        {
            blue = true;
        }

        if (Vector3.Dot(m_redCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_redCenter.transform.up, transform.parent.right) >= 0.9f)
        {
            red = true;
        }

        if (Vector3.Dot(m_orangeCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_orangeCenter.transform.up, transform.parent.right) >= 0.9f)
        {
            orange = true;
        }

        if (Vector3.Dot(m_greenCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_greenCenter.transform.up, transform.parent.right) >= 0.9f)
        {
            green = true;
        }

        if (Vector3.Dot(m_yellowCenter.transform.up, -transform.parent.forward) >= 0.9f || Vector3.Dot(m_yellowCenter.transform.up, transform.parent.right) >= 0.9f)
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
        foreach (GameObject cube in m_cubes)
        {
            if (cube.name == tarCubeName)
            {
                //Debug.Log("tarEdgeCube found! name == " + cube.name);
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
                //Debug.Log("stateIndex == " + stateIndex.ToString());

                // DO A TURN/s BASED ON CURRENT STATE INDEX
                switch (stateIndex)
                {
                    case 13: // front top middle

                        // U -> R -> Ui -> Ri -> Ui -> Fi -> U -> F

                        break;

                    case 15: // left top middle

                        // Ui
                        UserInput[] moves15 = new UserInput[1];

                        moves15[0] = new UserInput(13, -transform.parent.forward, new Vector2(300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves15));
                        break;

                    case 16: // right top middle

                        // U
                        UserInput[] moves16 = new UserInput[1];

                        moves16[0] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves16));
                        break;

                    case 18: // back top middle

                        // U -> U
                        UserInput[] moves18 = new UserInput[2];

                        moves18[0] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));
                        moves18[1] = new UserInput(13, -transform.parent.forward, new Vector2(-300.0f, 0.0f));

                        StartCoroutine(ExecuteMoves(moves18));
                        break;

                    case 8: // front left middle

                        // Ui -> Li -> U -> L -> U -> F -> Ui -> Fi

                        break;

                    case 10: // left back middle

                        // Ui -> Bi -> U -> B -> U -> L -> Ui -> Li

                        break;

                    case 11: // right back middle

                        // U -> B -> Ui -> Bi -> Ui -> Ri -> U -> R

                        break;

                    default:
                        Debug.Log("tarEdgeCube found in un-accounted position! ");
                        return true; // END RECURSION!!!      
                }

                return false; // RECURSE!!!
            }            
        }

        return false;
    }
}

    
