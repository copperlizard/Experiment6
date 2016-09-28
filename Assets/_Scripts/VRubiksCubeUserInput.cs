using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VRubiksCubeController))]
public class VRubiksCubeUserInput : MonoBehaviour
{
    public float m_minMove;

    private VRubiksCubeController m_cubeController;

    private GameObject m_touched;

	// Use this for initialization
	void Start ()
    {
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
        MeshRenderer touchedRenderer = touched.GetComponent<MeshRenderer>();
        touchedRenderer.enabled = true;

        Vector2 move;

        do
        {
            move = Input.GetTouch(0).position - touchStart;

            if (move.magnitude > m_minMove)
            {
                //Send m_touched and move to VRubiksCubeController
                m_cubeController.Turn(m_touched, move);

                //Exit loop
                break;
            }

            yield return null;
        } while (Input.touchCount > 0);

        touchedRenderer.enabled = false;

        yield return null;
    }
}
