using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class AudioMenu : MonoBehaviour
{
    public AudioMixer m_mainMixer;

    public Slider m_masterVolumeSlider, m_musicVolumeSlider, m_sfxVolumeSlider;
            
	// Use this for initialization
	void Start ()
    {
        if (m_masterVolumeSlider == null)
        {
            Debug.Log("m_masterVolumeSlider not assigned!!!");
        }

        if (m_musicVolumeSlider == null)
        {
            Debug.Log("m_musicVolumeSlider not assigned!!!");
        }

        if (m_sfxVolumeSlider == null)
        {
            Debug.Log("m_sfxVolumeSlider not assigned!!!");
        }

        LoadPlayerPrefs();        
    }
	
    private void LoadPlayerPrefs()
    {       
        float prefMasterVol = PlayerPrefs.GetFloat("MasterVol", -1.0f);
        if (prefMasterVol != -1.0f)
        {
            m_musicVolumeSlider.value = prefMasterVol;
        }

        float prefMusicVol = PlayerPrefs.GetFloat("MusicVol", -1.0f);
        if (prefMusicVol != -1.0f)
        {
            m_musicVolumeSlider.value = prefMusicVol;
        }

        float prefSFXVol = PlayerPrefs.GetFloat("SFXVol", -1.0f);
        if (prefSFXVol != -1.0f)
        {
            m_sfxVolumeSlider.value = prefSFXVol;
        }

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMasterVolume ()
    {
        m_mainMixer.SetFloat("MasterVol", m_masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MasterVol", m_masterVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        m_mainMixer.SetFloat("MusicVol", m_musicVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVol", m_musicVolumeSlider.value);
    }

    public void SetSFXVolume()
    {
        m_mainMixer.SetFloat("SFXVol", m_sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVol", m_sfxVolumeSlider.value);
    }
}
