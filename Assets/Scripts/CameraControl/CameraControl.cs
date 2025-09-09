using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Settings")]
    public float edgeThickness = 50f;   // pixel zone near edges
    public float maxRotateSpeed = 100f; // max speed at far edge
    public Transform pivot;             // camera follow target

    [Header("Vertical Clamp")]
    public float minVerticalAngle = -45f; // look down limit
    public float maxVerticalAngle = 75f;  // look up limit

    private float currentXRotation = 0f;

    void Start()
    {
        // Initialize current vertical rotation
        currentXRotation = pivot.localEulerAngles.x;
        if (currentXRotation > 180f) currentXRotation -= 360f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float horizontalInput = 0f;
        float verticalInput = 0f;

        // --- Horizontal edge detection ---
        if (mousePos.x <= edgeThickness)
            horizontalInput = -GetEdgeFactor(mousePos.x, edgeThickness);
        else if (mousePos.x >= screenWidth - edgeThickness)
            horizontalInput = GetEdgeFactor(screenWidth - mousePos.x, edgeThickness);

        // --- Vertical edge detection ---
        if (mousePos.y <= edgeThickness)
            verticalInput = -GetEdgeFactor(mousePos.y, edgeThickness);
        else if (mousePos.y >= screenHeight - edgeThickness)
            verticalInput = GetEdgeFactor(screenHeight - mousePos.y, edgeThickness);

        // Apply horizontal rotation
        if (horizontalInput != 0f)
        {
            float yaw = horizontalInput * maxRotateSpeed * Time.deltaTime;
            pivot.Rotate(Vector3.up, yaw, Space.World);
        }

        // Apply vertical rotation (with clamping)
        if (verticalInput != 0f)
        {
            float pitch = verticalInput * maxRotateSpeed * Time.deltaTime;
            currentXRotation = Mathf.Clamp(currentXRotation - pitch, minVerticalAngle, maxVerticalAngle);
            pivot.localEulerAngles = new Vector3(currentXRotation, pivot.localEulerAngles.y, 0f);
        }
    }

    /// <summary>
    /// Returns factor (0 to 1) based on how deep the cursor is in the edge zone.
    /// </summary>
    private float GetEdgeFactor(float distance, float edgeZone)
    {
        return Mathf.Clamp01(1f - (distance / edgeZone));
    }
}
