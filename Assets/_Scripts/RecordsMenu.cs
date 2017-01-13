using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class RecordsMenu : MonoBehaviour
{
    public Text m_recordsText;

    private GameMode m_mode;

    private Records m_recs = new Records();

    // Use this for initialization
    void Start ()
    {
        if (m_recordsText == null)
        {
            Debug.Log("m_recordsText not assigned!!!");
        }

        UpdateRecordsText();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    private void LoadRecords()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        switch (m_mode)
        {
            case GameMode.LEARN:
                if (File.Exists(Application.persistentDataPath + "/LearnRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/LearnRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/LearnRecords.dat");
                }
                break;
            case GameMode.TIMED:
                if (File.Exists(Application.persistentDataPath + "/TimedRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TimedRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TimedRecords.dat");
                }
                break;
            case GameMode.TURNS:
                if (File.Exists(Application.persistentDataPath + "/TurnsRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TurnsRecords.dat", FileMode.Open);
                    m_recs = (Records)bf.Deserialize(file);
                }
                else
                {
                    file = File.Create(Application.persistentDataPath + "/TurnsRecords.dat");
                }
                break;
            default:
                Debug.Log("m_mode not set (LoadRecords)!!!");
                return;
        }

        file.Close();
    }

    private void SaveRecords()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        switch (m_mode)
        {
            case GameMode.LEARN:
                if (File.Exists(Application.persistentDataPath + "/LearnRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/LearnRecords.dat", FileMode.Open);
                }
                else
                {
                    Debug.Log("error loading records!!!");
                    return;
                }
                break;
            case GameMode.TIMED:
                if (File.Exists(Application.persistentDataPath + "/TimedRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TimedRecords.dat", FileMode.Open);
                }
                else
                {
                    Debug.Log("error loading records!!!");
                    return;
                }
                break;
            case GameMode.TURNS:
                if (File.Exists(Application.persistentDataPath + "/TurnsRecords.dat"))
                {
                    file = File.Open(Application.persistentDataPath + "/TurnsRecords.dat", FileMode.Open);
                }
                else
                {
                    Debug.Log("error loading records!!!");
                    return;
                }
                break;
            default:
                Debug.Log("m_mode not set (SaveRecords)!!!");
                return;
        }

        /*
        Debug.Log("saving records!!!");        
        for (int i = 0; i < m_recs.m_record.Length; i++)
        {
            Debug.Log("recs.m_record[" + i.ToString() + "].time == " + m_recs.m_record[i].time.ToString());
        }
        */

        bf.Serialize(file, m_recs);
        file.Close();
    }

    void UpdateRecordsText ()
    {
        LoadRecords();
        SaveRecords();

        string RecordsString = "";

        switch (m_mode)
        {
            case GameMode.LEARN:
                RecordsString += "<b>MODE: LEARN</b>" + System.Environment.NewLine + System.Environment.NewLine;
                break;
            case GameMode.TIMED:
                RecordsString += "<b>MODE: TIMED</b>" + System.Environment.NewLine + System.Environment.NewLine;
                break;
            case GameMode.TURNS:
                RecordsString += "<b>MODE: TURNS</b>" + System.Environment.NewLine + System.Environment.NewLine;
                break;
        }

        if (m_mode == GameMode.LEARN || m_mode == GameMode.TIMED)
        {
            // #1 - 00:00:00.00 - 000 / 00
            for (int i = 0; i < m_recs.m_record.Length; i++)
            {
                RecordsString += "#" + (i + 1).ToString() + " - ";

                int hours = 0, mins = 0, secs = 0, decsecs = 0;
                float recTime = m_recs.m_record[i].time, secsInHour = 3600.0f, secsInMin = 60.0f;
                while (recTime >= secsInHour)
                {
                    hours++;
                    recTime -= secsInHour;
                }
                while (recTime >= secsInMin)
                {
                    mins++;
                    recTime -= secsInMin;
                }
                secs = (int)recTime;

                decsecs = (int)((recTime - secs) * 100.0f);

                RecordsString += string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hours, mins, secs, decsecs) + " - ";
                RecordsString += m_recs.m_record[i].turns.ToString() + " / " + m_recs.m_record[i].par.ToString();
                RecordsString += System.Environment.NewLine;
            }
        }
        else if (m_mode == GameMode.TURNS)
        {
            // #1 - 000 / 00 - 00:00:00.00
            for (int i = 0; i < m_recs.m_record.Length; i++)
            {
                RecordsString += "#" + (i + 1).ToString() + " - ";

                int hours = 0, mins = 0, secs = 0, decsecs = 0; // Maybe add decsecs (decimal seconds)...
                float recTime = m_recs.m_record[i].time, secsInHour = 3600.0f, secsInMin = 60.0f;
                while (recTime >= secsInHour)
                {
                    hours++;
                    recTime -= secsInHour;
                }
                while (recTime >= secsInMin)
                {
                    mins++;
                    recTime -= secsInMin;
                }
                secs = (int)recTime;

                decsecs = (int)((recTime - secs) * 100.0f);

                RecordsString += m_recs.m_record[i].turns.ToString() + " / " + m_recs.m_record[i].par.ToString() + " - ";
                RecordsString += string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hours, mins, secs, decsecs);
                RecordsString += System.Environment.NewLine;
            }
        }

        m_recordsText.text = RecordsString;
    }

    public void NextMode ()
    {
        switch (m_mode)
        {
            case GameMode.LEARN:
                m_mode = GameMode.TIMED;
                break;
            case GameMode.TIMED:
                m_mode = GameMode.TURNS;
                break;
            case GameMode.TURNS:
                m_mode = GameMode.LEARN;
                break;
            default:
                Debug.Log("error switching modes!!!");
                break;
        }

        UpdateRecordsText();
    }

    public void PrevMode ()
    {
        switch (m_mode)
        {
            case GameMode.LEARN:
                m_mode = GameMode.TURNS;
                break;
            case GameMode.TIMED:
                m_mode = GameMode.LEARN;
                break;
            case GameMode.TURNS:
                m_mode = GameMode.TIMED;
                break;
            default:
                Debug.Log("error switching modes!!!");
                break;
        }

        UpdateRecordsText();
    }
}
