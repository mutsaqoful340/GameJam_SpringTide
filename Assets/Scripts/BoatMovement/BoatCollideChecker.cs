using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class BoatCollideChecker : MonoBehaviour
{
    //public PlayerLife playerLife; // Reference to the PlayerLife script

    private bool isColliding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !isColliding)
        {
            isColliding = true;
            //playerLife.isBoatColliding = true; // Set the flag in PlayerLife
            Debug.Log("Boat started colliding with an obstacle.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") && isColliding)
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") && isColliding)
        {
            isColliding = false;
            //playerLife.isBoatColliding = false; // Reset the flag in PlayerLife
            Debug.Log("Boat stopped colliding with an obstacle.");
        }
    }
}
