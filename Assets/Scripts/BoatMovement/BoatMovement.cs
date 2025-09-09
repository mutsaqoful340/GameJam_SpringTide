using System.Runtime.Versioning;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Header("Engine Telegraph Settings")]
    public int maxAhead = 3;
    public int maxAstern = -3;
    public float telegraphStepSpeed = 2f; // units per telegraph stage
    [HideInInspector] public int currentTelegraph = 0; // -3 .. 0 .. +3

    [Header("Movement Settings")]
    public float accelerationSpeed = 2f; // how quickly boat matches target speed
    public float rotationSpeed = 20f;

    [Header("Steering Wheel")]
    public GameObject steeringWheel;

    private float currentSpeed = 0f;
    private float currentRotationSpeed = 0f;
    private HandlePlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<HandlePlayerInput>();
    }

    void Update()
    {
        HandleTelegraphInput();
        HandleSteering();
        MoveBoat();
    }

    void HandleTelegraphInput()
    {
        if (playerInput.AcceleratePressed)
        {
            if (currentTelegraph < maxAhead)
            {
                currentTelegraph++;
            }
        }

        if (playerInput.DeceleratePressed)
        {
            if (currentTelegraph > maxAstern)
            {
                currentTelegraph--;
            }
        }
    }

    void HandleSteering()
    {
        float turn = 0f;

        if (playerInput.TurnLeftPressed)
        {
            turn = -1f;
        }
        else if (playerInput.TurnRightPressed)
        {
            turn = 1f;
        }
        
        // Scale rotation based on speed
        float speedFactor = Mathf.Abs(currentSpeed) / (telegraphStepSpeed * maxAhead);
        // Clamp to avoid crazy values
        speedFactor = Mathf.Clamp01(speedFactor);

        // Effective rotation speed depends on how fast boat is moving
        float effectiveRotationSpeed = rotationSpeed * speedFactor;

        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, turn * effectiveRotationSpeed, Time.deltaTime / 2f);
        transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);
    }


    void MoveBoat()
    {
        float targetSpeed = currentTelegraph * telegraphStepSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationSpeed);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        Debug.Log($"Telegraph: {currentTelegraph}, Target Speed: {targetSpeed}, Current Speed: {currentSpeed}");
    }
}
