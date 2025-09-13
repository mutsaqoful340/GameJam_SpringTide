using UnityEngine;

public class BoatFlipChecker : MonoBehaviour
{
    [Header("Assigned Sensor Collider")]
    public Collider flipSensor; // drag child BoxCollider di sini
    public BoatDurability boatDurability;

    public bool isBoatFlipped = false;

    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        // Hanya jalan kalau yang nabrak adalah WaterSurface DAN collider kita adalah sensor
        if (other.CompareTag("BoatFlipSensor"))
        {
            isBoatFlipped = true;
        }
    }

    void Update()
    {
        if (isBoatFlipped)
        {
            CapsizeBoat();
        }
    }

    private void CapsizeBoat()
    {
        boatDurability.currentDurability = 0;
    }
}
