using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Header("Engine Telegraph Settings")]
    public int maxAhead = 3;
    public int maxAstern = -3;
    public float telegraphStepSpeed = 2f;
    [HideInInspector] public int currentTelegraph = 0;

    [Header("Movement Settings")]
    public float accelerationSpeed = 2f;
    public float rotationSpeed = 20f;

    [Header("Steering Settings")]
    public float rudderStep = 5f;          // How many degrees per key press
    public float maxRudderAngle = 45f;     // Max left/right rudder
    public int spinsForMaxRudder = 6;      // Number of full wheel spins required for max rudder

    [Header("Steering Wheel")]
    public GameObject steeringWheel;
    public float steeringWheelMultiplier = 1f;   // visual spin scale
    public float steeringWheelTurnSpeed = 5f;    // how quickly the wheel visual catches up

    private float currentSpeed = 0f;
    private float rudderAngle = 0f;        // actual rudder
    private float speedVelocity;
    private bool isDragging = false;
    private float targetRudder;            // where the rudder *wants* to go
    private float lastMouseAngle;
    private float wheelAngle = 0f;         // accumulated wheel spin in degrees
    private float wheelVisualAngle = 0f;   // smoothed visual wheel angle
    private HandlePlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<HandlePlayerInput>();
    }

    void Update()
    {
        HandleTelegraphInput();
        HandleMouseSteering();
        HandleKeyboardSteering();
        ApplySteering();
        MoveBoat();
    }

    void HandleTelegraphInput()
    {
        if (playerInput.AcceleratePressed && currentTelegraph < maxAhead)
            currentTelegraph++;

        if (playerInput.DeceleratePressed && currentTelegraph > maxAstern)
            currentTelegraph--;
    }

    void HandleMouseSteering()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("SteeringWheel"))
            {
                isDragging = true;

                // Record starting angle
                Vector3 wheelScreenPos = Camera.main.WorldToScreenPoint(steeringWheel.transform.position);
                Vector2 dir = (Vector2)(Input.mousePosition - wheelScreenPos);
                lastMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 wheelScreenPos = Camera.main.WorldToScreenPoint(steeringWheel.transform.position);
            Vector2 dir = (Vector2)(Input.mousePosition - wheelScreenPos);
            float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float delta = Mathf.DeltaAngle(lastMouseAngle, currentAngle);

            // Accumulate wheel rotation
            wheelAngle -= delta;

            // Clamp wheel angle so rudder can't exceed max deflection
            float wheelLimit = 360f * spinsForMaxRudder;
            wheelAngle = Mathf.Clamp(wheelAngle, -wheelLimit, wheelLimit);

            // Convert spins into rudder target
            float spins = wheelAngle / 360f;
            targetRudder = Mathf.Clamp(spins * (maxRudderAngle / spinsForMaxRudder),
                                    -maxRudderAngle, maxRudderAngle);

            lastMouseAngle = currentAngle;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
}


    void HandleKeyboardSteering()
    {
        if (playerInput.TurnLeftPressed)
            targetRudder -= rudderStep * Time.deltaTime * 10f;
        else if (playerInput.TurnRightPressed)
            targetRudder += rudderStep * Time.deltaTime * 10f;

        targetRudder = Mathf.Clamp(targetRudder, -maxRudderAngle, maxRudderAngle);
    }

    void ApplySteering()
    {
        // Rudder gradually catches up to target
        rudderAngle = Mathf.Lerp(rudderAngle, targetRudder, Time.deltaTime * 5f);

        float effectiveRotation = (rudderAngle / maxRudderAngle)
                                * rotationSpeed
                                * (Mathf.Abs(currentSpeed) / (telegraphStepSpeed * maxAhead));

        transform.Rotate(Vector3.up, effectiveRotation * Time.deltaTime);

        // Smooth the visual wheel rotation
        if (steeringWheel != null)
        {
            wheelVisualAngle = Mathf.Lerp(wheelVisualAngle, wheelAngle * steeringWheelMultiplier,
                                          Time.deltaTime * steeringWheelTurnSpeed);

            steeringWheel.transform.localRotation = Quaternion.Euler(0, wheelVisualAngle, 0);
        }

        Debug.Log($"Telegraph: {currentTelegraph}, Speed: {currentSpeed:F2}, Rudder: {rudderAngle:F2}, EffectiveRot: {effectiveRotation:F2}");
    }

    void MoveBoat()
    {
        float targetSpeed = currentTelegraph * telegraphStepSpeed;

        // SmoothDamp: gradual acceleration/deceleration
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 1f / accelerationSpeed);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
