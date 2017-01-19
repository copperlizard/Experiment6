using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackroundFXToggle : MonoBehaviour
{
    public Toggle m_bgFXToggle;

    private GameObject m_backgroundParticles;

    private GameObject[] m_bgParticles;

    // Use this for initialization
    void Awake ()
    {
        m_backgroundParticles = GameObject.FindGameObjectWithTag("BackgroundParticles");

        if (m_backgroundParticles == null)
        {
            Debug.Log("m_backgroundParticles not found!!!");
        }

        if (m_bgFXToggle == null)
        {
            Debug.Log("m_bgFXToggle not assigned!!!");
        }

        Transform[] ts = new Transform[m_backgroundParticles.transform.childCount];
        m_bgParticles = new GameObject[m_backgroundParticles.transform.childCount];
        for (int i = 0; i < m_backgroundParticles.transform.childCount; i++)
        {
            ts[i] = m_backgroundParticles.transform.GetChild(i);
            m_bgParticles[i] = ts[i].gameObject;
        }

        int coolBG = PlayerPrefs.GetInt("BackroundFX", 1);
        bool enableFX = (coolBG == 1);
        m_bgFXToggle.isOn = enableFX;
        for (int i = 0; i < m_bgParticles.Length; i++)
        {
            m_bgParticles[i].SetActive(enableFX);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ToggleParticleEffects()
    {
        if (m_bgFXToggle.isOn)
        {
            PlayerPrefs.SetInt("BackroundFX", 1);
        }
        else
        {
            PlayerPrefs.SetInt("BackroundFX", -1);
        }

        for (int i = 0; i < m_bgParticles.Length; i++)
        {
            m_bgParticles[i].SetActive(m_bgFXToggle.isOn);
        }
    }
}
