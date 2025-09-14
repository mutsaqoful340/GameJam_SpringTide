using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApp : MonoBehaviour
{

    public void QuitApplication()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}
