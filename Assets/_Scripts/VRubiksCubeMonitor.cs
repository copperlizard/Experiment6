using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRubiksCubeMonitor : MonoBehaviour
{
    public int m_stage = 0; // Solved stages

    private List<GameObject> m_cubes; // Does not include centers

    private GameObject m_whiteCenter, m_blueCenter, m_redCenter, m_orangeCenter, m_greenCenter, m_yellowCenter; // Center TouchPanels

    // Use this for initialization
    void Start ()
    {
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
        // For each cube, check if child panels are aligned with center panels
        foreach (GameObject cube in m_cubes)
        {

        }

        return false;
    }
}
