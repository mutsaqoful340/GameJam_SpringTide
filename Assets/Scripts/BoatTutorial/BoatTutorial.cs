using UnityEngine;

public class BoatTutorial : MonoBehaviour
{


    public bool DTActive = false; // Flag to check if Boat Tutorial is active
    //private int currentDTIndex = 0; // Current index of the tutorial object
    public bool notDay1 = false; // Flag to check if it's not Day 1

    public void Start()
    {

        //DTActive = true; // Reset the flag when starting the tutorial

        if (!DTActive)
        {

        }
        else
        {

        }
    }
    
    public void DTOpen()
    {
        if (!DTActive) //&& currentDTIndex < DTObjects.Length && DTObjects[currentDTIndex] != null)
        {
            DTActive = true;

            //DTObjects[currentDTIndex].SetActive(true);
            //Debug.Log("Doom Tutorial activated: " + DTObjects[currentDTIndex].name);
        }
        else
        {
            //Debug.LogWarning("Doom Tutorial is already active or no more objects to activate.");
        }
    }

    public void DTClose()
    {
        DTActive = false;
        //DTObjects[currentDTIndex].SetActive(false);
        //currentDTIndex++;

    }
}
