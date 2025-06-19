using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quiz1Scene : MonoBehaviour
{
    public void LoadScene(string Quiz1)
    {
        SceneManager.LoadScene(Quiz1);
    }
}
