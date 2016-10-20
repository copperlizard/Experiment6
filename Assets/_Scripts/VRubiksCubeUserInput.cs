using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

[RequireComponent(typeof(VRubiksCubeController))]
public class VRubiksCubeUserInput : MonoBehaviour
{
    public GameObject m_selectHighlight;

    public float m_minMove;

    private VRubiksCubeController m_cubeController;

    private GameObject m_touched;

    private LimitedStack<GameObject> m_touchedStack;   // CHANGE THESE TO A UNDO AND REDO LIMITED STACK AND SWITCH TO CUSTOM DATATYPE FOR USER INPUTS
    private LimitedStack<Vector2> m_moveStack;

    private int m_touchedArrayIndex = 0, m_lastMoveArrayIndex = 0;

    private bool m_undid = false;

	// Use this for initialization
	void Start ()
    {
        m_touchedStack = new LimitedStack<GameObject>(10);
        m_moveStack = new LimitedStack<Vector2>(10);

        m_cubeController = GetComponent<VRubiksCubeController>();
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
                        StartCoroutine(GetMoveInput(m_touched));
                        return;
                    }
                }                    
            }
        }
    }

    IEnumerator GetMoveInput (GameObject touched)
    {
        Vector2 touchStart = Input.GetTouch(0).position;

        //Add touch noise and "visualization" stuff here
        //MeshRenderer touchedRenderer = touched.GetComponent<MeshRenderer>();
        //touchedRenderer.enabled = true;

        m_selectHighlight.transform.position = touched.transform.position;
        m_selectHighlight.transform.rotation = touched.transform.rotation;
        m_selectHighlight.transform.parent = touched.transform;
        m_selectHighlight.SetActive(true);

        Vector2 move;

        do
        {
            move = Input.GetTouch(0).position - touchStart;

            if (move.magnitude > m_minMove)
            {
                //Send m_touched and move to VRubiksCubeController
                m_cubeController.Turn(m_touched, move);

                m_touchedStack.Push(m_touched);
                m_moveStack.Push(move);

                if (m_undid)
                {
                    m_undid = false;
                }

                //Exit loop
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
        // GOALS: ALOW USER TO UNDO 10ISH TIMES
    }

    public void Redo()
    {
        // GOALS: ALLOW USER TO REDO ALL UNDO'S
    }
}
