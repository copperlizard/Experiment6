using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomScrollbar : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public RectTransform m_content, m_scrollView;
    
    private Slider m_slider;

    private float m_curY = 0.0f, m_maxY = 0.0f;

    private bool m_sliding = false;

	// Use this for initialization
	void Start ()
    {
	    if (m_content == null)
        {
            Debug.Log("m_content not assigned!!!");
        }
        
        if (m_scrollView == null)
        {
            Debug.Log("m_scrollView not assigned!!!");
        }
        
        m_maxY = m_content.sizeDelta.y - m_scrollView.sizeDelta.y;        

        m_slider = GetComponent<Slider>();

        if (m_slider == null)
        {
            Debug.Log("m_slider not found!!!");
        }

        //Debug.Log("m_maxY == " + m_maxY.ToString());
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_curY = m_content.localPosition.y;

        if (!m_sliding)
        {
            m_slider.value = m_curY / m_maxY;
        }
        else
        {
            m_content.localPosition = new Vector3(0.0f, m_slider.value * m_maxY, 0.0f);
        }        
	}

    public void OnEndDrag(PointerEventData data)
    {        
        m_sliding = false;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        m_sliding = true;
    }
}
