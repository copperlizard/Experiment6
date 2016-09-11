using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VRubiksCubeUserInput))]
public class VRubiksCubeController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Turn(GameObject touched, Vector2 move)
    {
        //Front/top/bottom or side...
        float upCheck, forwardCheck, rightCheck;
        upCheck = Vector3.Dot(touched.transform.up, transform.up);
        forwardCheck = Vector3.Dot(touched.transform.up, transform.forward);
        rightCheck = Vector3.Dot(touched.transform.up, transform.right);

        bool front = false, side = false;
        if (Mathf.Abs(upCheck) >= 0.9f || Mathf.Abs(forwardCheck) >= 0.9f)
        {
            front = true;
        }
        else if (Mathf.Abs(rightCheck) >= 0.9f)
        {
            side = true;
        }

        Debug.Log("front/top/bottom == " + front.ToString() + " ; side == " + side.ToString());

        //Center touch?
        if (touched.transform.parent.gameObject.tag == "center")
        {
            if (front)
            {

            }

            if (side)
            {

            }
        }
        else
        {
            if (front)
            {

            }

            if (side)
            {

            }
        }
    }
}
