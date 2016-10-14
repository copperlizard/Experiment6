using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRubiksCubeMonitor : MonoBehaviour
{
    public float m_percentComplete = 0;
    public int m_stage = 0; // Solved stages    

    public bool m_randomizeOnStart = false;

    private List<GameObject> m_cubes, m_cornerCubes, m_edgeCubes; // Does not include centers

    private GameObject m_whiteCenter, m_blueCenter, m_redCenter, m_orangeCenter, m_greenCenter, m_yellowCenter; // Center TouchPanels

    private Dictionary<Vector3, int> m_cubeMap;
    private Dictionary<int, Vector3> m_mapCube;
    private bool[] m_cubeStates = new bool[20];

    // Use this for initialization
    void Start ()
    {
        //Build cube map
        m_cubeMap = new Dictionary<Vector3, int>();

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


        //Build mapCube
        m_mapCube = new Dictionary<int, Vector3>();

        m_mapCube.Add(0, new Vector3(-1.0f, -1.0f, -1.0f)); //left bottom front
        m_mapCube.Add(1, new Vector3(0.0f, -1.0f, -1.0f)); //middle bottom front
        m_mapCube.Add(2, new Vector3(1.0f, -1.0f, -1.0f)); //right bottom front

        m_mapCube.Add(3, new Vector3(-1.0f, -1.0f, 0.0f)); //left bottom middle
        //m_mapCube.Add(0, new Vector3(0.0f, -1.0f, 0.0f)); //middle bottom middle *center
        m_mapCube.Add(4, new Vector3(1.0f, -1.0f, 0.0f)); //right bottom middle

        m_mapCube.Add(5, new Vector3(-1.0f, -1.0f, 1.0f)); //left bottom back
        m_mapCube.Add(6, new Vector3(0.0f, -1.0f, 1.0f)); //middle bottom back
        m_mapCube.Add(7, new Vector3(1.0f, -1.0f, 1.0f)); //right bottom back

        m_mapCube.Add(8, new Vector3(-1.0f, 0.0f, -1.0f)); //left middle front
        //m_mapCube.Add(0, new Vector3(0.0f, 0.0f, -1.0f)); //middle middle front *center
        m_mapCube.Add(9, new Vector3(1.0f, 0.0f, -1.0f)); //right middle front

        //m_mapCube.Add(0, new Vector3(-1.0f, 0.0f, 0.0f)); //left middle middle *center
        //m_mapCube.Add(0, new Vector3(0.0f, 0.0f, 0.0f)); //middle middle middle *base cube
        //m_mapCube.Add(0, new Vector3(1.0f, 0.0f, 0.0f)); //right middle middle *center

        m_mapCube.Add(10, new Vector3(-1.0f, 0.0f, 1.0f)); //left middle back 
        //m_mapCube.Add(0, new Vector3(0.0f, 0.0f, 1.0f)); //middle middle back *center
        m_mapCube.Add(11, new Vector3(1.0f, 0.0f, 1.0f)); //right middle back

        m_mapCube.Add(12, new Vector3(-1.0f, 1.0f, -1.0f)); //left top front
        m_mapCube.Add(13, new Vector3(0.0f, 1.0f, -1.0f)); //middle top front
        m_mapCube.Add(14, new Vector3(1.0f, 1.0f, -1.0f)); //right top front

        m_mapCube.Add(15, new Vector3(-1.0f, 1.0f, 0.0f)); //left top middle
        //m_mapCube.Add(0, new Vector3(0.0f, 1.0f, 0.0f)); //middle top middle *center
        m_mapCube.Add(16, new Vector3(1.0f, 1.0f, 0.0f)); //right top middle

        m_mapCube.Add(17, new Vector3(-1.0f, 1.0f, 1.0f)); //left top back
        m_mapCube.Add(18, new Vector3(0.0f, 1.0f, 1.0f)); //middle top back
        m_mapCube.Add(19, new Vector3(1.0f, 1.0f, 1.0f)); //right top back

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
                //Debug.Log("cube found ; name == " + gameObjs[i].name);

                // Store center panels
                if (gameObjs[i].tag == "center")
                {
                    //Transform panel = gameObjs[i].GetComponentInChildren<Transform>();

                    Transform panel = gameObjs[i].transform.GetChild(0);

                    Debug.Log("center cube found ; name == " + gameObjs[i].name + System.Environment.NewLine +
                        "touch panel name == " + panel.gameObject.name + System.Environment.NewLine +
                        "touch panel transform.up == " + panel.transform.up.ToString());

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

        // Make lists of corner and edge cubes using m_cubes (for randomizing cube)...
        m_cornerCubes = new List<GameObject>();
        m_edgeCubes = new List<GameObject>();
        foreach (GameObject cube in m_cubes)
        {
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

            // Corner or egde
            int stateIndex = -1;
            bool stateIndexFound = m_cubeMap.TryGetValue(mapInput, out stateIndex);
            if (!stateIndexFound)
            {
                Debug.Log("cube stateIndex not found while creating corner/edge lists!!!");
            }
            else
            {
                // Corner cube
                if (stateIndex == 0 || stateIndex == 2 || stateIndex == 5 || stateIndex == 7 || stateIndex == 12 || stateIndex == 14 || stateIndex == 17 || stateIndex == 19) 
                {
                    m_cornerCubes.Add(cube);
                }
                // Edge cube
                else if (stateIndex == 1 || stateIndex == 3 || stateIndex == 4 || stateIndex == 6 || stateIndex == 8 || stateIndex == 9 || stateIndex == 10 || stateIndex == 11 ||
                    stateIndex == 13 || stateIndex == 15 || stateIndex == 16 || stateIndex == 18) 
                {
                    m_edgeCubes.Add(cube);
                }
            }
        }

        /*
        Debug.Log("m_cubes.count == " + m_cubes.Count.ToString() + System.Environment.NewLine +
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
    }

    public void RandomizeCube ()
    {
        //Random.seed = (int)(Time.realtimeSinceStartup + Time.deltaTime * 100.0f);
        Random.seed = 0;

        List<int> cornerSpots = new List<int>();
        cornerSpots.Add(0);
        cornerSpots.Add(2);
        cornerSpots.Add(5);
        cornerSpots.Add(7);
        cornerSpots.Add(12);
        cornerSpots.Add(14);
        cornerSpots.Add(17);
        cornerSpots.Add(19);

        List<int> edgeSpots = new List<int>();
        edgeSpots.Add(1);
        edgeSpots.Add(3);
        edgeSpots.Add(4);
        edgeSpots.Add(6);
        edgeSpots.Add(8);
        edgeSpots.Add(9);
        edgeSpots.Add(10);
        edgeSpots.Add(11);
        edgeSpots.Add(13);
        edgeSpots.Add(15);
        edgeSpots.Add(16);
        edgeSpots.Add(18);

        // Randomize Corner cubes
        foreach (GameObject cube in m_cornerCubes)
        {
            // Pick a new spot for each cube (each spot picked only once)
            int pick = Random.Range(0, cornerSpots.Count);
            int spot = cornerSpots[pick];
            cornerSpots.RemoveAt(pick); 

            Vector3 tarPos;
            bool foundPos = m_mapCube.TryGetValue(spot, out tarPos);
            if (!foundPos)
            {
                Debug.Log("Error finding new cube pos while randomizing corners...");
            }
            else
            {
                RepositionCube(cube, tarPos, true, spot);
            }
        }

        /*
        // Randomize Edge cubes
        foreach (GameObject cube in m_edgeCubes)
        {
            // Pick a new spot for each cube (each spot picked only once)
            int pick = Random.Range(0, edgeSpots.Count);
            int spot = edgeSpots[pick];
            edgeSpots.RemoveAt(pick);

            Vector3 tarPos;
            bool foundPos = m_mapCube.TryGetValue(spot, out tarPos);
            if (!foundPos)
            {
                Debug.Log("Error finding new cube pos while randomizing corners...");
            }
            else
            {
                RepositionCube(cube, tarPos, false, spot);
            }
        }
        */
    }

    private void RepositionCube (GameObject cube, Vector3 tarPos, bool corner, int spot)
    {
        Vector3 oldPos = cube.transform.localPosition;

        cube.transform.localPosition = tarPos;

        //Vector3 diff = oldPos - tarPos;

        // Store panels in random order
        GameObject[] panels = new GameObject[cube.transform.childCount];
        List<int> slots = new List<int>();
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            slots.Add(i);
        }
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            int pick = Random.Range(0, slots.Count - 1);
            int slot = slots[pick];
            slots.RemoveAt(pick);

            panels[slot] = cube.transform.GetChild(i).gameObject;
        }

        
        // Use tarPos/spot and panelUps to generate new cube orientation
        Quaternion rot = new Quaternion();
        if (corner)
        {
            switch (spot)
            {
                case 0:

                    // FIRST SEE WHAT NO ROTATION DOES; THEN TRY TO ALIGN ONE PANEL; THEN TRY AND GET MULTIPLE PANELS ALIGNED


                    // left bottom front

                    // I need to align panel.transform.up's with -transform.up, -transform.right, -transform.forward

                    // An orientation quaternion ... the desired orientation of the panels ... no correlation to current cube orientation
                    //Quaternion tarRot = Quaternion.LookRotation(-transform.forward, -transform.up);

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    //rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    //cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    //rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    //cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with -transform.forward
                    //rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    //cube.transform.localRotation = rot * cube.transform.localRotation;

                    break;
                case 2:
                    // right bottom front

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 5:
                    // left bottom back

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 7:
                    // right bottom back

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 12:
                    // left top front

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 14:
                    // right top front

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 17:
                    // left top back

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 19:
                    // right top back

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[2] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[2].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;                    
                    break;
            }
        }
        else
        {
            switch (spot)
            {
                case 1:
                    // middle bottom front

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;                    
                case 3:
                    // left bottom middle

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 4:
                    // right bottom middle

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 6:
                    // middle bottom back

                    // Rotate cube so panelUp[0] is aligned with -transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 8:
                    // left middle front

                    // Rotate cube so panelUp[0] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 9:
                    // right middle front

                    // Rotate cube so panelUp[0] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 10:
                    // left middle back

                    // Rotate cube so panelUp[0] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 11:
                    // right middle back

                    // Rotate cube so panelUp[0] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 13:
                    // middle top front

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 15:
                    // left top middle

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with -transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(-transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 16:
                    // right top middle

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.right
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.right));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
                case 18:
                    // middle top back

                    // Rotate cube so panelUp[0] is aligned with transform.up
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[0].transform.up), cube.transform.InverseTransformVector(transform.up));
                    cube.transform.localRotation = rot * cube.transform.localRotation;

                    // Rotate cube so panelUp[1] is aligned with transform.forward
                    rot = Quaternion.FromToRotation(cube.transform.InverseTransformVector(panels[1].transform.up), cube.transform.InverseTransformVector(transform.forward));
                    cube.transform.localRotation = rot * cube.transform.localRotation;
                    break;
            }
        }        

        Debug.Log("repositiong cube.name == " + cube.name + " to " + tarPos.ToString() + System.Environment.NewLine +
            "cube.LocalRotation == " + cube.transform.localRotation.ToString() + " or " + cube.transform.localRotation.eulerAngles.ToString());
    }

    public bool CheckSolved ()
    {
        // For each cube, check if child panels are aligned with matching color center panels
        foreach (GameObject cube in m_cubes)
        {
            // Assume the cube is correctly located
            bool cubeLocated = true;

            // Check panel alignment
            //Transform[] panels = cube.GetComponentsInChildren<Transform>();
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

                    if (m_cubeStates[6] && m_cubeStates[10] && m_cubeStates[11] && m_cubeStates[18]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[5] && m_cubeStates[7] && m_cubeStates[17] && m_cubeStates[19]) // Third layer complete
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

                    if (m_cubeStates[1] && m_cubeStates[8] && m_cubeStates[9] && m_cubeStates[13]) // Other cross complete
                    {
                        if (4 > m_stage)
                        {
                            m_stage = 4;
                        }

                        if (m_cubeStates[0] && m_cubeStates[2] && m_cubeStates[12] && m_cubeStates[14]) // Third layer complete
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

        return true;
    }
}
