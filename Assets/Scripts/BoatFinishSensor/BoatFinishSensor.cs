using UnityEngine;

public class BoatFinishSensor : MonoBehaviour
{
    [Header("Finish Line Settings")]
    public GameObject finish_UI; // Assign the finish line GameObject in the Inspector
    [SerializeField] private bool hasFinished = false;
    private BoatMovement boatMovement;
    private MarineHorn marineHorn;

    [SerializeField] private float finishStayDuration = 3f; // seconds to stay in trigger
    private float stayTimer = 0f;
    private bool isInsideFinish = false;

    void Start()
    {
        boatMovement = GetComponent<BoatMovement>();
        marineHorn = GetComponent<MarineHorn>();
        hasFinished = false;
    }

    void Update()
    {
        if (isInsideFinish && !hasFinished && boatMovement.currentTelegraph == 0)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= finishStayDuration)
            {
                PlayerHasFinished();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            isInsideFinish = true;
            stayTimer = 0f; // reset when entering
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            isInsideFinish = false;
            stayTimer = 0f; // reset when leaving
        }
    }

    public void PlayerHasFinished()
    {
        finish_UI.SetActive(true);
        hasFinished = true;
        marineHorn.enabled = false;
        boatMovement.enabled = false;
        Debug.Log("Boat stayed on finish line long enough, race completed!");
    }
}
