using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{

    public void Quit ()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
