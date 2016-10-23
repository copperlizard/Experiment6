using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserInput
{
    public Vector3 m_touchedUp; // Local to VRubiksCube
    public Vector2 m_move;
    public int m_touchedParentLocation;
    public UserInput(int touchedLocation, Vector3 touchedUp, Vector2 move)
    {
        m_touchedParentLocation = touchedLocation;
        m_touchedUp = touchedUp;
        m_move = move;
    }
}

public class LimitedStack<T> : LinkedList<T>
{
    private readonly int _maxSize;
    public LimitedStack(int maxSize)
    {
        _maxSize = maxSize;
    }

    public void Push(T item)
    {
        this.AddFirst(item);

        if (this.Count > _maxSize)
            this.RemoveLast();
    }

    public T Pop()
    {
        var item = this.First.Value;
        this.RemoveFirst();
        return item;
    }
}

[RequireComponent(typeof(VRubiksCubeMonitor))]
[RequireComponent(typeof(VRubiksCubeController))]
public class VRubiksCubeUserInput : MonoBehaviour
{
    public GameObject m_selectHighlight;

    public float m_minMove;

    private List<GameObject> m_cubes;

    //private Dictionary<int, Vector3> m_mapCube;

    private VRubiksCubeController m_cubeController;
    private VRubiksCubeMonitor m_cubeMonitor;

    private GameObject m_touched;

    //private LimitedStack<GameObject> m_touchedStack;   // CHANGE THESE TO A UNDO AND REDO LIMITED STACK AND SWITCH TO CUSTOM DATATYPE FOR USER INPUTS
    //private LimitedStack<Vector2> m_moveStack;

    private LimitedStack<UserInput> m_undoStack;
    private LimitedStack<UserInput> m_redoStack;

    private int m_touchedArrayIndex = 0, m_lastMoveArrayIndex = 0;
    
	// Use this for initialization
	void Start ()
    {
        /*
        //Build cube map
        m_mapCube = new Dictionary<int, Vector3>();

        m_mapCube.Add(0, new Vector3(-1.0f, -1.0f, -1.0f)); //left bottom front
        m_mapCube.Add(1, new Vector3(0.0f, -1.0f, -1.0f)); //middle bottom front
        m_mapCube.Add(2, new Vector3(1.0f, -1.0f, -1.0f)); //right bottom front

        m_mapCube.Add(3, new Vector3(-1.0f, -1.0f, 0.0f)); //left bottom middle
        m_mapCube.Add(20, new Vector3(0.0f, -1.0f, 0.0f)); //middle bottom middle *center
        m_mapCube.Add(4, new Vector3(1.0f, -1.0f, 0.0f)); //right bottom middle

        m_mapCube.Add(5, new Vector3(-1.0f, -1.0f, 1.0f)); //left bottom back
        m_mapCube.Add(6, new Vector3(0.0f, -1.0f, 1.0f)); //middle bottom back
        m_mapCube.Add(7, new Vector3(1.0f, -1.0f, 1.0f)); //right bottom back

        m_mapCube.Add(8, new Vector3(-1.0f, 0.0f, -1.0f)); //left middle front
        m_mapCube.Add(21, new Vector3(0.0f, 0.0f, -1.0f)); //middle middle front *center
        m_mapCube.Add(9, new Vector3(1.0f, 0.0f, -1.0f)); //right middle front

        m_mapCube.Add(22, new Vector3(-1.0f, 0.0f, 0.0f)); //left middle middle *center
        //m_mapCube.Add(0, new Vector3(0.0f, 0.0f, 0.0f)); //middle middle middle *base cube
        m_mapCube.Add(23, new Vector3(1.0f, 0.0f, 0.0f)); //right middle middle *center

        m_mapCube.Add(10, new Vector3(-1.0f, 0.0f, 1.0f)); //left middle back 
        m_mapCube.Add(24, new Vector3(0.0f, 0.0f, 1.0f)); //middle middle back *center
        m_mapCube.Add(11, new Vector3(1.0f, 0.0f, 1.0f)); //right middle back

        m_mapCube.Add(12, new Vector3(-1.0f, 1.0f, -1.0f)); //left top front
        m_mapCube.Add(13, new Vector3(0.0f, 1.0f, -1.0f)); //middle top front
        m_mapCube.Add(14, new Vector3(1.0f, 1.0f, -1.0f)); //right top front

        m_mapCube.Add(15, new Vector3(-1.0f, 1.0f, 0.0f)); //left top middle
        m_mapCube.Add(25, new Vector3(0.0f, 1.0f, 0.0f)); //middle top middle *center
        m_mapCube.Add(16, new Vector3(1.0f, 1.0f, 0.0f)); //right top middle

        m_mapCube.Add(17, new Vector3(-1.0f, 1.0f, 1.0f)); //left top back
        m_mapCube.Add(18, new Vector3(0.0f, 1.0f, 1.0f)); //middle top back
        m_mapCube.Add(19, new Vector3(1.0f, 1.0f, 1.0f)); //right top back   
        */

        // Build cube list
        m_cubes = new List<GameObject>();        
        GameObject[] gameObjs = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjs.Length; i++)
        {
            if (gameObjs[i].layer == LayerMask.NameToLayer("Cube"))
            {
                m_cubes.Add(gameObjs[i]);
            }
        }       

        m_undoStack = new LimitedStack<UserInput>(10);
        m_redoStack = new LimitedStack<UserInput>(10);

        m_cubeController = GetComponent<VRubiksCubeController>();
        m_cubeMonitor = GetComponent<VRubiksCubeMonitor>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetTouchInput();	
	}

    private void GetTouchInput ()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {

                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);

                RaycastHit hit;

                // If ray hit
                if (Physics.Raycast(ray, out hit))
                {                    
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TouchPanel"))
                    {
                        //Valid touch
                        m_touched = hit.collider.gameObject;

                        //Debug.Log("m_touched == " + m_touched.name);

                        //Watch for valid move
                        StartCoroutine(GetMoveInput());
                        return;
                    }
                }                    
            }
        }
    }

    IEnumerator GetMoveInput ()
    {
        Vector2 touchStart = Input.GetTouch(0).position;

        //Add touch noise and "visualization" stuff here
        //MeshRenderer touchedRenderer = touched.GetComponent<MeshRenderer>();
        //touchedRenderer.enabled = true;

        m_selectHighlight.transform.position = m_touched.transform.position;
        m_selectHighlight.transform.rotation = m_touched.transform.rotation;
        m_selectHighlight.transform.parent = m_touched.transform;
        m_selectHighlight.SetActive(true);

        Vector2 move;

        do
        {
            move = Input.GetTouch(0).position - touchStart;

            if (move.magnitude > m_minMove)
            {
                // Send m_touched and move to VRubiksCubeController
                m_cubeController.Turn(m_touched, move);
                
                // Prep to find cube location index (there is probably a nicer way to do this...)
                Vector3 mapInput = Vector3.zero, gimbalPos = transform.parent.InverseTransformPoint(m_touched.transform.parent.position);
                if (gimbalPos.x < -0.5f)
                {
                    mapInput.x = -1.0f;
                }
                else if (gimbalPos.x > 0.5f)
                {
                    mapInput.x = 1.0f;
                }
                else
                {
                    mapInput.x = 0.0f;
                }

                if (gimbalPos.y < -0.5f)
                {
                    mapInput.y = -1.0f;
                }
                else if (gimbalPos.y > 0.5f)
                {
                    mapInput.y = 1.0f;
                }
                else
                {
                    mapInput.y = 0.0f;
                }

                if (gimbalPos.z < -0.5f)
                {
                    mapInput.z = -1.0f;
                }
                else if (gimbalPos.z > 0.5f)
                {
                    mapInput.z = 1.0f;
                }
                else
                {
                    mapInput.z = 0.0f;
                }

                // Store user cmd
                int touchedParentLocation = -1;
                bool touchedParentLocationFound = m_cubeMonitor.m_cubeMap.TryGetValue(mapInput, out touchedParentLocation);
                if (touchedParentLocationFound)
                {
                    //UserInput cmd = new UserInput(touchedParentLocation, m_touched.transform.parent.InverseTransformDirection(m_touched.transform.up), move);
                    UserInput cmd = new UserInput(touchedParentLocation, transform.parent.InverseTransformDirection(m_touched.transform.up), move);
                    m_undoStack.Push(cmd);
                    if (m_redoStack.Count > 0)
                    {
                        m_redoStack.Clear();
                    }

                    Debug.Log("storing user command:" + System.Environment.NewLine +
                        "m_touchedParentLocation == " + cmd.m_touchedParentLocation.ToString() + System.Environment.NewLine +
                        "m_touchedUp == " + cmd.m_touchedUp.ToString() + System.Environment.NewLine +
                        "m_move == " + cmd.m_move.ToString() + System.Environment.NewLine + System.Environment.NewLine +
                        "mapInput == " + mapInput.ToString() + System.Environment.NewLine +
                        "m_touched.transform.parent.localPosition == " + m_touched.transform.parent.localPosition.ToString() + System.Environment.NewLine +
                        "m_touched.transform.parent.name == " + m_touched.transform.parent.name + System.Environment.NewLine +
                        "m_touched.transform.position == " + m_touched.transform.position.ToString() + System.Environment.NewLine +
                        "m_touched.transform.parent.position == " + m_touched.transform.parent.position.ToString() +System.Environment.NewLine +
                        "transform.parent.InverseTransformPoint(m_touched.transform.parent.position) == " + transform.parent.InverseTransformPoint(m_touched.transform.parent.position).ToString());
                }
                else
                {
                    Debug.Log("touchedParentLocation not found; undo not possible!!!");
                }                
                
                // Exit loop
                break;
            }

            yield return null;
        } while (Input.touchCount > 0);

        //touchedRenderer.enabled = false;

        m_selectHighlight.SetActive(false);

        yield return null;
    }

    public void Undo()
    {
        if (m_cubeController.m_rotatingFace)
        {
            return;
        }
        if (m_undoStack.Count > 0)
        {
            UserInput cmd = m_undoStack.Pop();

            Debug.Log("cmd to undo:" + System.Environment.NewLine + "m_touchedParentLocation == " + cmd.m_touchedParentLocation.ToString() + System.Environment.NewLine + 
                "m_touchedUp == " + cmd.m_touchedUp.ToString() + System.Environment.NewLine +
                "m_move == " + cmd.m_move.ToString());

            GameObject toTouch = null;
            foreach (GameObject cube in m_cubes)
            {
                // Prep to find cube location (there is probably a nicer way to do this...)
                Vector3 mapInput = Vector3.zero, gimbalPos = transform.parent.InverseTransformPoint(cube.transform.position);
                if (gimbalPos.x < -0.5f)
                {
                    mapInput.x = -1.0f;
                }
                else if (gimbalPos.x > 0.5f)
                {
                    mapInput.x = 1.0f;
                }
                else
                {
                    mapInput.x = 0.0f;
                }

                if (gimbalPos.y < -0.5f)
                {
                    mapInput.y = -1.0f;
                }
                else if (gimbalPos.y > 0.5f)
                {
                    mapInput.y = 1.0f;
                }
                else
                {
                    mapInput.y = 0.0f;
                }

                if (gimbalPos.z < -0.5f)
                {
                    mapInput.z = -1.0f;
                }
                else if (gimbalPos.z > 0.5f)
                {
                    mapInput.z = 1.0f;
                }
                else
                {
                    mapInput.z = 0.0f;
                }

                // Check cube location
                int cubeLocation = -1;
                bool cubeLocationFound = m_cubeMonitor.m_cubeMap.TryGetValue(mapInput, out cubeLocation);
                if (cubeLocationFound)
                {
                    if (cubeLocation == cmd.m_touchedParentLocation)
                    {
                        Debug.Log("found cube location: " + cubeLocation.ToString() + " ; cube name: " + cube.name);

                        // Check panels
                        for (int i = 0; i < cube.transform.childCount; i++)
                        {
                            Debug.Log("this panel up == " + cube.transform.InverseTransformDirection(cube.transform.GetChild(i).up).ToString());
                            if (Vector3.Dot(transform.parent.InverseTransformDirection(cube.transform.GetChild(i).up), cmd.m_touchedUp) > 0.9f)
                            {
                                Debug.Log("found matching panel!");

                                toTouch = cube.transform.GetChild(i).gameObject;
                                break;
                            }
                        }

                        if (toTouch == null)
                        {                            
                            Debug.Log("matching panel not found!");

                            m_undoStack.Push(cmd);
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("cubeLocationFound not found!!!");

                    m_undoStack.Push(cmd);
                    return;
                }
            }

            m_cubeController.Turn(toTouch, -cmd.m_move);
            m_redoStack.Push(cmd);
        }

        return;        
    }

    public void Redo()
    {
        if (m_cubeController.m_rotatingFace)
        {
            return;
        }
        if (m_redoStack.Count > 0)
        {
            UserInput cmd = m_redoStack.Pop();
            m_undoStack.Push(cmd);

            GameObject toTouch = null;
            foreach (GameObject cube in m_cubes)
            {
                // Prep to find cube location (there is probably a nicer way to do this...)
                Vector3 mapInput = Vector3.zero, gimbalPos = transform.parent.InverseTransformPoint(cube.transform.position);
                if (gimbalPos.x < -0.5f)
                {
                    mapInput.x = -1.0f;
                }
                else if (gimbalPos.x > 0.5f)
                {
                    mapInput.x = 1.0f;
                }
                else
                {
                    mapInput.x = 0.0f;
                }

                if (gimbalPos.y < -0.5f)
                {
                    mapInput.y = -1.0f;
                }
                else if (gimbalPos.y > 0.5f)
                {
                    mapInput.y = 1.0f;
                }
                else
                {
                    mapInput.y = 0.0f;
                }

                if (gimbalPos.z < -0.5f)
                {
                    mapInput.z = -1.0f;
                }
                else if (gimbalPos.z > 0.5f)
                {
                    mapInput.z = 1.0f;
                }
                else
                {
                    mapInput.z = 0.0f;
                }

                // Check cube location
                int cubeLocation = -1;
                bool cubeLocationFound = m_cubeMonitor.m_cubeMap.TryGetValue(mapInput, out cubeLocation);
                if (cubeLocationFound)
                {
                    if (cubeLocation == cmd.m_touchedParentLocation)
                    {
                        // Check panels
                        for (int i = 0; i < cube.transform.childCount; i++)
                        {
                            if (Vector3.Dot(transform.parent.InverseTransformDirection(cube.transform.GetChild(i).up), cmd.m_touchedUp) > 0.9f)
                            {
                                toTouch = cube.transform.GetChild(i).gameObject;
                                break;
                            }
                        }

                        if (toTouch == null)
                        {
                            Debug.Log("matching panel not found!");

                            m_redoStack.Push(cmd);
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("cubeLocationFound not found!!!");

                    m_redoStack.Push(cmd);
                    return;
                }
            }

            m_cubeController.Turn(toTouch, cmd.m_move);
        }

        return;
    }
}
