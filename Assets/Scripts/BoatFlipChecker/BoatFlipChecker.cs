using UnityEngine;

public class BoatFlipChecker : MonoBehaviour
{
    [Header("Assigned Sensor Collider")]
    public Collider flipSensor; // assign the child BoxCollider here
    public BoatDurability boatDurability;

    [HideInInspector] public bool isBoatFlipped = false;

    private void OnTriggerEnter(Collider e)
    {
        // Make sure the trigger is from the flipSensor only
        if (e.CompareTag("WaterSurface"))
        {
            // Optional: check if the boat is upside down
            isBoatFlipped = true;
            boatDurability.BoatCapsize();
        }
    }
}
