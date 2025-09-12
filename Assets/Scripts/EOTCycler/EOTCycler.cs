using System.Runtime.InteropServices;
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

    [Header("Telegraph Needle Settings")]
    public GameObject telegraphNeedle; // Assign in Inspector
    public float[] telegraphAngles;    // Assign in Inspector, should have 7 angles for 7 steps
    public float needleLerpSpeed = 5f; // Speed of needle movement
    public float currentTelegraphStep;

    private bool isDragging = false;
    private float dragStartY;

    // Flags to avoid spamming sounds
    private bool hasPlayedIn = false;
    private bool hasPlayedOut = false;

    [SerializeField] private BoatMovement boatMovement;
    private EOTNeedle eotNeedle;

    void Start()
    {
        currentTelegraphStep = boatMovement.currentTelegraph;
        eotNeedle = GetComponent<EOTNeedle>();
    }

    void Update()
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

            eotNeedle.UpdateNeedle(boatMovement.currentTelegraph);
        }
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