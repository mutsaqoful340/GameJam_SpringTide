using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BoatUnchartedMap : MonoBehaviour
{
    [Header("Uncharted Map Settings")]
    public float unchartedMapDuration = 5f; // Duration before ExecutePlayer
    public GameObject unchartedMapUI;
    public GameObject krakenSpotUI;
    public GameObject kraken; // Assign the Kraken GameObject in the Inspector
    public Animator krakenAnimator;
    public VolumeProfile volumeProfile;
    public Volume unchartedMapFog;
    private bool isInUnchartedMap = false;
    private BoatMovement boatMovement;
    private BoatDurability boatDurability;
    private Coroutine countdownCoroutine;

    // Store original fog values
    private float originalMeanFreePath;
    private float originalBaseHeight;
    private float originalMaxHeight;
    private float originalMaxDistance;

    private Coroutine fogLerpCoroutine;

    void Start()
    {
        boatMovement = GetComponent<BoatMovement>();
        boatDurability = GetComponent<BoatDurability>();
        unchartedMapUI.SetActive(false);
        krakenSpotUI.SetActive(false);
        kraken.SetActive(false);
        

        if (volumeProfile.TryGet<Fog>(out var fog))
        {
            fog.meanFreePath.value = 14f;
            fog.baseHeight.value = 29.5f;
            fog.maximumHeight.value = 160f;
            fog.maxFogDistance.value = 150f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("KrakenTentacle"))
        {
            boatDurability.currentDurability = 0; // Apply damage to the boat
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UnchartedMap") || (other.CompareTag("KrakenSpotSensor") && !isInUnchartedMap))
        {
            if (other.CompareTag("UnchartedMap"))
            {
                isInUnchartedMap = true;
                unchartedMapUI.SetActive(true);
            }
            else if (other.CompareTag("KrakenSpotSensor"))
            {
                krakenSpotUI.SetActive(true);
            }


            // Start countdown
            countdownCoroutine = StartCoroutine(UnchartedCountdown());

            if (volumeProfile.TryGet<Fog>(out var fog))
            {
                if (fogLerpCoroutine != null) StopCoroutine(fogLerpCoroutine);
                fogLerpCoroutine = StartCoroutine(LerpFog(fog, 3f, 28f, 48f, 220.2f, 612.4f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UnchartedMap") || (other.CompareTag("KrakenSpotSensor") && isInUnchartedMap))
        {
            if (other.CompareTag("KrakenSpotSensor"))
            {
                isInUnchartedMap = false;
                unchartedMapUI.SetActive(false);
            }
            else if (other.CompareTag("UnchartedMap"))
            {
                krakenSpotUI.SetActive(false);
            }


            // Stop countdown if boat leaves before time is up
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            if (volumeProfile.TryGet<Fog>(out var fog))
            {
                if (fogLerpCoroutine != null) StopCoroutine(fogLerpCoroutine);
                fogLerpCoroutine = StartCoroutine(LerpFog(fog, 3f,
                    originalMeanFreePath, originalBaseHeight,
                    originalMaxHeight, originalMaxDistance));
            }
        }
    }

    private IEnumerator LerpFog(Fog fog, float duration,
        float targetMeanFreePath, float targetBaseHeight,
        float targetMaxHeight, float targetMaxDistance)
    {
        float startMeanFreePath = fog.meanFreePath.value;
        float startBaseHeight   = fog.baseHeight.value;
        float startMaxHeight    = fog.maximumHeight.value;
        float startMaxDistance  = fog.maxFogDistance.value;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            fog.meanFreePath.value   = Mathf.Lerp(startMeanFreePath, targetMeanFreePath, t);
            fog.baseHeight.value     = Mathf.Lerp(startBaseHeight, targetBaseHeight, t);
            fog.maximumHeight.value  = Mathf.Lerp(startMaxHeight, targetMaxHeight, t);
            fog.maxFogDistance.value = Mathf.Lerp(startMaxDistance, targetMaxDistance, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        fog.meanFreePath.value   = targetMeanFreePath;
        fog.baseHeight.value     = targetBaseHeight;
        fog.maximumHeight.value  = targetMaxHeight;
        fog.maxFogDistance.value = targetMaxDistance;
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
        kraken.SetActive(true);
        krakenAnimator.Play("Kraken_Slap");
        Debug.Log("Player executed for staying too long in uncharted map!");
        // Put your logic here (damage, teleport, etc.)
    }
}