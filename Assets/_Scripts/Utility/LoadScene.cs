using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour
{
	public void Load (int i)
    {
        SceneManager.LoadScene(i);
    }

    public void Load (string name)
    {
        SceneManager.LoadScene(name);
    }
}
