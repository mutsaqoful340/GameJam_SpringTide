using UnityEngine;

public class EOTNeedle : MonoBehaviour
{
    public Transform needle;
    private int currentTelegraphStep;
    private BoatMovement boatMovement;

    public float rotationSpeed = 5f; // Higher = faster movement

    private float targetAngle; // Where the needle wants to go

    void Start()
    {
        boatMovement = FindObjectOfType<BoatMovement>();
    }

    void Update()
    {
        currentTelegraphStep = boatMovement.currentTelegraph;
        UpdateNeedle(currentTelegraphStep);
    }

    public void UpdateNeedle(int currentTelegraphStep)
    {
        if (needle == null) return;

        // Pick target angle based on step
        if (currentTelegraphStep == 0) targetAngle = 32f;
        else if (currentTelegraphStep == 1) targetAngle = 68f;
        else if (currentTelegraphStep == 2) targetAngle = 108.5f;
        else if (currentTelegraphStep == 3) targetAngle = 144.5f;
        else if (currentTelegraphStep == -1) targetAngle = -5f;
        else if (currentTelegraphStep == -2) targetAngle = -40f;
        else if (currentTelegraphStep == -3) targetAngle = -80f;

        // Get current Z
        float currentZ = needle.localEulerAngles.z;

        // Smoothly move Z towards target
        float newZ = Mathf.LerpAngle(currentZ, targetAngle, Time.deltaTime * rotationSpeed);

        // Apply back with the fixed X and Y
        needle.localEulerAngles = new Vector3(0, 90, newZ);
    }
}
