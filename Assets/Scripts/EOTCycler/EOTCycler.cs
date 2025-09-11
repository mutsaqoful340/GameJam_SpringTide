using UnityEngine;

public class EOTCycler : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform lever;
    public Vector3 neutralRotation = Vector3.zero;       // middle
    public Vector3 upRotation = new Vector3(-30f, 0f, 0f);
    public Vector3 downRotation = new Vector3(30f, 0f, 0f);
    public float returnSpeed = 180f;  // speed back to neutral

    [Header("Audio")]
    public AudioSource audioSource;   // Assign in Inspector
    public AudioClip EOT_IN;
    public AudioClip EOT_OUT;

    [Header("Needle Indicator")]
    public Transform needle;          // assign your needle Transform here
    public float[] stepAngles;        // size = 7, each index corresponds to a telegraph step
    public float needleLerpSpeed = 5f;

    private bool isDragging = false;
    private float dragStartY;

    // Flags to avoid spamming sounds
    private bool hasPlayedIn = false;
    private bool hasPlayedOut = false;

    [SerializeField] private BoatMovement boatMovement;

    void Update()
    {
        HandleLeverDrag();
        HandleLeverReturn();
        UpdateNeedle();
    }

    private void HandleLeverDrag()
    {
        // Start drag
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("EOTCycler"))
            {
                isDragging = true;
                dragStartY = Input.mousePosition.y;

                // reset flags on fresh drag
                hasPlayedIn = false;
                hasPlayedOut = false;

                Debug.Log("Raycast hit EOTCycler");
            }
        }

        // While dragging
        if (isDragging && Input.GetMouseButton(0))
        {
            if (!hasPlayedIn)   // only once per drag
            {
                PlaySound(EOT_IN);
                hasPlayedIn = true;
                hasPlayedOut = false;
            }

            float dragDelta = dragStartY - Input.mousePosition.y;
            float t = Mathf.Clamp(dragDelta / 100f, -1f, 1f);

            if (t > 0)
                lever.localRotation = Quaternion.Lerp(
                    Quaternion.Euler(neutralRotation),
                    Quaternion.Euler(downRotation),
                    t
                );
            else
                lever.localRotation = Quaternion.Lerp(
                    Quaternion.Euler(neutralRotation),
                    Quaternion.Euler(upRotation),
                    -t
                );

            if (t >= 1f)
            {
                CycleEOTDOWN();
                isDragging = false;
            }
            else if (t <= -1f)
            {
                CycleEOTUP();
                isDragging = false;
            }
        }
    }

    private void HandleLeverReturn()
    {
        // Smooth auto-return when not dragging
        if (!isDragging && lever.localRotation != Quaternion.Euler(neutralRotation))
        {
            if (!hasPlayedOut)   // only once per return
            {
                PlaySound(EOT_OUT);
                hasPlayedOut = true;
                hasPlayedIn = false;
            }

            lever.localRotation = Quaternion.RotateTowards(
                lever.localRotation,
                Quaternion.Euler(neutralRotation),
                returnSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateNeedle()
    {
        if (needle == null || stepAngles.Length == 0) return;

        // BoatMovement currentTelegraph is between maxAstern and maxAhead
        int telegraphIndex = boatMovement.currentTelegraph - boatMovement.maxAstern;
        telegraphIndex = Mathf.Clamp(telegraphIndex, 0, stepAngles.Length - 1);

        float targetAngle = stepAngles[telegraphIndex];

        // use signed angle instead of Unity’s wrapped 0–360
        float currentAngle = needle.localEulerAngles.z;
        if (currentAngle > 180f) currentAngle -= 360f;

        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * needleLerpSpeed);
        needle.localEulerAngles = new Vector3(
            needle.localEulerAngles.x,
            needle.localEulerAngles.y,
            newAngle
        );

        // --- Debug info ---
        Debug.Log($"[Needle] Telegraph={boatMovement.currentTelegraph}, Index={telegraphIndex}, " +
                $"Current={currentAngle:F2}, Target={targetAngle:F2}, New={newAngle:F2}");
    }



    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.spatialBlend = 1f; // fully 3D
            audioSource.PlayOneShot(clip);
        }
    }

    private void CycleEOTUP()
    {
        if (boatMovement.currentTelegraph < boatMovement.maxAhead)
        {
            boatMovement.currentTelegraph++;
        }
    }

    private void CycleEOTDOWN()
    {
        if (boatMovement.currentTelegraph > boatMovement.maxAstern)
        {
            boatMovement.currentTelegraph--;
        }
    }
}
