using UnityEngine;
using System.Collections;

public class DisableScreenRotation : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Screen.orientation = ScreenOrientation.Portrait;
	}
}
