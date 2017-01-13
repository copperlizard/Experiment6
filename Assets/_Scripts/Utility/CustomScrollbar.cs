using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomScrollbar : MonoBehaviour
{
    public RectTransform m_content;

    private RectTransform m_scrollView;

    private Slider m_slider;

    private float m_curY = 0.0f, m_maxY = 0.0f;

	// Use this for initialization
	void Start ()
    {
	    if (m_content == null)
        {
            Debug.Log("content rect not assigned!!!");
        }

        m_scrollView = transform.parent.parent.gameObject.GetComponent<RectTransform>();

        if (m_scrollView == null)
        {
            Debug.Log("m_scrollView not found!!!");
        }

        m_maxY = m_content.sizeDelta.y - m_scrollView.sizeDelta.y;        

        m_slider = GetComponent<Slider>();

        if (m_slider == null)
        {
            Debug.Log("m_slider not found!!!");
        }

        Debug.Log("m_maxY == " + m_maxY.ToString());
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_curY = m_content.localPosition.y;

        //Debug.Log("m_curY == " + m_curY.ToString() + "; m_maxY = " + m_maxY.ToString());
        

        //Debug.Log((m_curY / m_maxY).ToString());

        m_slider.value = m_curY / m_maxY;
	}
}
