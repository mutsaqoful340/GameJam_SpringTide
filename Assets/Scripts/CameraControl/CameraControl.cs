using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    [Header("Settings")]
    public float edgeThickness = 50f;   // pixel zone near edges
    public float maxRotateSpeed = 100f; // normal speed
    public float zoomRotateSpeed = 40f; // reduced speed when zoomed
    public Transform pivot;             // camera follow target

    [Header("Vertical Clamp")]
    public float minVerticalAngle = -45f; // look down limit
    public float maxVerticalAngle = 75f;  // look up limit

    [Header("Cinemachine Control")]
    public CinemachineVirtualCamera vcam;
    public float zoomSpeed = 5f;
    public float normalFOV = 60f;
    public float zoomFOV = 40f;

    private float currentXRotation = 0f;
    private bool isZoomed = false;

    void Start()
    {
        currentXRotation = pivot.localEulerAngles.x;
        if (currentXRotation > 180f) currentXRotation -= 360f;

        // ✅ Load sensitivitas dari PlayerPrefs
        float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", maxRotateSpeed);
        maxRotateSpeed = savedSensitivity;
        zoomRotateSpeed = savedSensitivity * 0.4f; // atau sesuai kebutuhan
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click pressed
        {
            isZoomed = !isZoomed; // Toggle zoom state
        }

        // Smooth transition of FOV
        float targetFOV = isZoomed ? zoomFOV : normalFOV;
        vcam.m_Lens.FieldOfView = Mathf.Lerp(
            vcam.m_Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );

        // ✅ Adjust rotate speed based on zoom state
        float currentRotateSpeed = isZoomed ? zoomRotateSpeed : maxRotateSpeed;

        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Horizontal edge detection
        if (mousePos.x <= edgeThickness)
            horizontalInput = -GetEdgeFactor(mousePos.x, edgeThickness);
        else if (mousePos.x >= screenWidth - edgeThickness)
            horizontalInput = GetEdgeFactor(screenWidth - mousePos.x, edgeThickness);

        // Vertical edge detection
        if (mousePos.y <= edgeThickness)
            verticalInput = -GetEdgeFactor(mousePos.y, edgeThickness);
        else if (mousePos.y >= screenHeight - edgeThickness)
            verticalInput = GetEdgeFactor(screenHeight - mousePos.y, edgeThickness);

        // Apply horizontal rotation
        if (horizontalInput != 0f)
        {
            float yaw = horizontalInput * currentRotateSpeed * Time.deltaTime;
            pivot.Rotate(Vector3.up, yaw, Space.World);
        }

        // Apply vertical rotation (with clamping)
        if (verticalInput != 0f)
        {
            float pitch = verticalInput * currentRotateSpeed * Time.deltaTime;
            currentXRotation = Mathf.Clamp(currentXRotation - pitch, minVerticalAngle, maxVerticalAngle);
            pivot.localEulerAngles = new Vector3(currentXRotation, pivot.localEulerAngles.y, 0f);
        }
    }

    private float GetEdgeFactor(float distance, float edgeZone)
    {
        return Mathf.Clamp01(1f - (distance / edgeZone));
    }
}
