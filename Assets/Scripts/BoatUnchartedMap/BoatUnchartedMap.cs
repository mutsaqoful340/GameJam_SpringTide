using UnityEngine;
using System.Collections;

public class BoatUnchartedMap : MonoBehaviour
{
    [Header("Uncharted Map Settings")]
    public float unchartedMapDuration = 5f; // Duration before ExecutePlayer
    public GameObject unchartedMapUI;
    private bool isInUnchartedMap = false;
    private BoatMovement boatMovement;
    private Coroutine countdownCoroutine;

    void Start()
    {
        boatMovement = GetComponent<BoatMovement>();
        unchartedMapUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UnchartedMap") && !isInUnchartedMap)
        {
            isInUnchartedMap = true;
            unchartedMapUI.SetActive(true);

            // Start countdown
            countdownCoroutine = StartCoroutine(UnchartedCountdown());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UnchartedMap") && isInUnchartedMap)
        {
            isInUnchartedMap = false;
            unchartedMapUI.SetActive(false);

            // Stop countdown if boat leaves before time is up
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }
        }
    }

    private IEnumerator UnchartedCountdown()
    {
        float remainingTime = unchartedMapDuration;

        while (remainingTime > 0f)
        {
            Debug.Log("Uncharted countdown: " + remainingTime.ToString("F2") + "s left");
            yield return null; // wait until next frame
            remainingTime -= Time.deltaTime;
        }

        ExecutePlayer();
    }

    private void ExecutePlayer()
    {
        Debug.Log("Player executed for staying too long in uncharted map!");
        // Put your logic here (damage, teleport, etc.)
    }
}
