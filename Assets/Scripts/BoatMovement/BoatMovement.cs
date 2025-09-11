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

    [Header("Steing Settings")]
    public float rudderStep = 5f;
    public float maxRudderAngle = 45f;
    public int spinsForMaxRudder = 6;
    public float driftTurnStrength = 0.05f;
    public AnimationCurve driftResponseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Steering Wheel")]
    public GameObject steeringWheel;
    public float steeringWheelMultiplier = 1f;
    public float steeringWheelTurnSpeed = 5f;

    [Header("Collision Settings")]
    public bool isBoatColliding = false;

    private float currentSpeed = 0f;
    private float rudderAngle = 0f;
    private float speedVelocity;
    private bool isDragging = false;
    private float targetRudder;
    private float lastMouseAngle;
    private float wheelAngle = 0f;
    private float wheelVisualAngle = 0f;
    private HandlePlayerInput playerInput;
    private BoatDurability boatDurability;
    private BoatBuoyancy boatBuoyancy;

    private void Awake()
    {
        playerInput = GetComponent<HandlePlayerInput>();
        boatDurability = GetComponent<BoatDurability>();
        boatBuoyancy = GetComponent<BoatBuoyancy>();
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

            wheelAngle -= delta;

            float wheelLimit = 360f * spinsForMaxRudder;
            wheelAngle = Mathf.Clamp(wheelAngle, -wheelLimit, wheelLimit);

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
        rudderAngle = Mathf.Lerp(rudderAngle, targetRudder, Time.deltaTime * 5f);

        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / (telegraphStepSpeed * maxAhead));

        if (normalizedSpeed > 0.01f)
        {
            float baseTurnFactor = normalizedSpeed * (1f - normalizedSpeed);
            float dragBoost = Mathf.Abs(rudderAngle) / maxRudderAngle * (1f - normalizedSpeed);

            float effectiveTurnFactor = baseTurnFactor + dragBoost * 0.5f;
            effectiveTurnFactor = Mathf.Clamp(effectiveTurnFactor, 0f, 1f);

            float effectiveRotation = (rudderAngle / maxRudderAngle)
                                    * rotationSpeed
                                    * effectiveTurnFactor;

            transform.Rotate(Vector3.up, effectiveRotation * Time.deltaTime);
        }
        else if (Mathf.Abs(rudderAngle) > 1f)
        {
            float rudderDeflection = Mathf.Abs(rudderAngle) / maxRudderAngle;
            float curveScale = driftResponseCurve.Evaluate(rudderDeflection);

            float driftTurn = (rudderAngle / maxRudderAngle) * rotationSpeed * driftTurnStrength * curveScale;
            transform.Rotate(Vector3.up, driftTurn * Time.deltaTime);
        }

        if (steeringWheel != null)
        {
            wheelVisualAngle = Mathf.Lerp(wheelVisualAngle, wheelAngle * steeringWheelMultiplier,
                                          Time.deltaTime * steeringWheelTurnSpeed);

            steeringWheel.transform.localRotation = Quaternion.Euler(0, wheelVisualAngle, 0);
        }

        //Debug.Log($"Telegraph: {currentTelegraph}, Speed: {currentSpeed:F2}, Rudder: {rudderAngle:F2}");
    }

    void MoveBoat()
    {
        float targetSpeed = currentTelegraph * telegraphStepSpeed;

        // Apply drag effect to target speed while turning
        if (Mathf.Abs(rudderAngle) > 1f)
        {
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / (telegraphStepSpeed * maxAhead));
            float rudderDeflection = Mathf.Abs(rudderAngle) / maxRudderAngle;
            float telegraphFactor = Mathf.Clamp01((float)Mathf.Abs(currentTelegraph) / maxAhead);

            float dragStrength = rudderDeflection * normalizedSpeed * telegraphFactor * telegraphStepSpeed;
            targetSpeed -= dragStrength;
        }

        float dynamicAccel = accelerationSpeed * Mathf.Max(1, Mathf.Abs(currentTelegraph));

        currentSpeed = Mathf.SmoothDamp(
            currentSpeed,
            targetSpeed,
            ref speedVelocity,
            1f / dynamicAccel
        );

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
