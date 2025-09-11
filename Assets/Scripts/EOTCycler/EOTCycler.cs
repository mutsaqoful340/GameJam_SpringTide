using UnityEngine;

public class EOTCycler : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform lever;
    public Vector3 neutralRotation = Vector3.zero;       // middle
    public Vector3 upRotation = new Vector3(-30f, 0f, 0f);
    public Vector3 downRotation = new Vector3(30f, 0f, 0f);
    public float returnSpeed = 180f;  // speed back to neutral

    private bool isDragging = false;
    private float dragStartY;

    [SerializeField] private BoatMovement boatMovement;

    private void Awake()
    {

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
                Debug.Log("Raycast hit EOTCycler");
            }
        }   

        // While dragging
        if (isDragging && Input.GetMouseButton(0))
        {
            // Inverted to make "mouse up = lever up"
            float dragDelta = dragStartY - Input.mousePosition.y;

            // Clamp drag (-1 to 1 range)
            float t = Mathf.Clamp(dragDelta / 100f, -1f, 1f);

            // Lerp between up & down
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

            // Trigger & auto-return
            if (t >= 1f)
            {
                CycleEOTDOWN();
                isDragging = false; // stop drag immediately
            }
            else if (t <= -1f)
            {
                CycleEOTUP();
                isDragging = false; // stop drag immediately
            }
        }

        // Smooth auto-return when not dragging
        if (!isDragging && lever.localRotation != Quaternion.Euler(neutralRotation))
        {
            lever.localRotation = Quaternion.RotateTowards(
                lever.localRotation,
                Quaternion.Euler(neutralRotation),
                returnSpeed * Time.deltaTime
            );
        }
    }

    private void CycleEOTUP()
    {
        if (boatMovement.currentTelegraph < boatMovement.maxAhead)
        {
            boatMovement.currentTelegraph++;
        }
        else
        {
            return;
        }
    }

    private void CycleEOTDOWN()
    {
        if (boatMovement.currentTelegraph > boatMovement.maxAstern)
        {
            boatMovement.currentTelegraph--;
        }
        else
        {
            return;
        }
    }
}
