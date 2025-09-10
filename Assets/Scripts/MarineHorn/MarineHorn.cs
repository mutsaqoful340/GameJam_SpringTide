using UnityEngine;

public class MarineHorn : MonoBehaviour
{
    [Header("Horn Settings")]
    public AudioSource hornAudioSource;
    public AudioClip hornClip;
    public Transform hornHandle;      // The handle part to move
    public float dragDistance = 100f; // Pixels to fully pull
    public float maxHornDuration = 5f; // Max seconds horn can sound
    public float maxMoveZ = -0.2f;     // How far handle moves along Z-axis
    public float pullThreshold = 0.7f; // Minimum pull (0-1) to trigger horn

    private bool isDragging = false;
    private float dragStartY;
    private float hornTimer = 0f;
    private bool hornActive = false;
    private bool isResetting = false;
    private bool hasReset = true;      // Allow first pull
    private Vector3 initialHandlePos;

    void Start()
    {
        if (hornHandle != null)
            initialHandlePos = hornHandle.localPosition;
    }

    void Update()
    {
        HandleInput();
        HandleHandleReset();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("MarineHorn"))
            {
                isDragging = true;
                isResetting = false; // Cancel any ongoing reset
                dragStartY = Input.mousePosition.y;

                // Remove this line: hasReset = false;
                // Now first pull works
            }
        }


        // While dragging
        if (isDragging && Input.GetMouseButton(0))
        {
            float dragDelta = dragStartY - Input.mousePosition.y;
            float t = Mathf.Clamp01(dragDelta / dragDistance);

            // Smooth handle movement along Z-axis
            if (hornHandle != null)
            {
                Vector3 pos = hornHandle.localPosition;
                float targetZ = initialHandlePos.z + t * maxMoveZ;
                pos.z = Mathf.Lerp(pos.z, targetZ, Time.deltaTime * 10f);
                hornHandle.localPosition = pos;
            }

            // Start horn only if handle has reset and pull threshold met
            if (!hornActive && hasReset && t >= pullThreshold)
                StartHorn();

            // Handle horn max duration
            if (hornActive)
            {
                hornTimer += Time.deltaTime;
                if (hornTimer >= maxHornDuration)
                    StopHorn();
            }
        }

        // Release drag
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StopHorn();
            isResetting = true; // Start smooth reset
        }
    }

    void HandleHandleReset()
    {
        // Smoothly reset handle position along Z-axis
        if (isResetting && hornHandle != null)
        {
            Vector3 pos = hornHandle.localPosition;
            pos.z = Mathf.Lerp(pos.z, initialHandlePos.z, Time.deltaTime * 5f); // Reset slower for natural feel
            hornHandle.localPosition = pos;

            // Stop resetting when close enough
            if (Mathf.Abs(pos.z - initialHandlePos.z) < 0.001f)
            {
                hornHandle.localPosition = initialHandlePos;
                isResetting = false;
                hasReset = true; // Allow horn to play again
            }
        }
    }

    void StartHorn()
    {
        hornActive = true;
        hornTimer = 0f;
        hornAudioSource.clip = hornClip;
        hornAudioSource.loop = true;
        hornAudioSource.Play();
    }

    void StopHorn()
    {
        if (hornActive)
        {
            hornAudioSource.Stop();
            hornActive = false;
        }
    }
}
