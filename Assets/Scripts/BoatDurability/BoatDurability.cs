using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class BoatDurability : MonoBehaviour
{
    [Header("Collider Settings")]
    public Collider boatCollider;
    public AudioSource boatCrashSound;

    [Header("Durability Settings")]
    public int maxDurability = 5;
    public int currentDurability;

    [Header("Light Animator")]
    public Animator lightAnimator;

    [Header("Boat Sinking Settings")]
    public float sinkingDuration = 3f; // Duration of the sinking effect
    public CameraControl cameraControl;
    public Collider EOTCollider;
    public GameObject waterBlocker;
    public float wobbleDuration = 0.5f; // Duration of the wobble effect
    public float wobbleAngle = 5f; // Angle of the wobble effect
    public AudioSource seaAmbience;
    public AudioSource underwaterAmbience;
    public AudioSource boatEngine;
    public GameObject BoatDestroyedUI;
    public GameObject krakenSlapCollider;
    public FollowWater krakenFollow;
    public GameObject BriefcaseOpenedUI;

    private bool isBoatColliding = false;

    [HideInInspector] public bool isBoatDestroyed = false;

    private BoatBuoyancy boatBuoyancy;
    private BoatMovement boatMovement;
    //private EOTCycler eotCycler;
    private MarineHorn marineHorn;
    private BoatBriefCase boatBriefCase;

    void Start()
    {
        isBoatColliding = false;
        isBoatDestroyed = false;
        currentDurability = maxDurability;

        boatBuoyancy = GetComponent<BoatBuoyancy>();
        boatMovement = GetComponent<BoatMovement>();
        //eotCycler = GetComponent<EOTCycler>();
        marineHorn = GetComponent<MarineHorn>();
        boatBriefCase = GetComponent<BoatBriefCase>();

        if (boatCollider == null)
        {
            Debug.LogError("Boat Collider is not assigned in the Inspector.");
        }

        if (lightAnimator == null)
        {
            Debug.LogError("Light Animator is not assigned in the Inspector.");
        }

        seaAmbience.mute = false;
        underwaterAmbience.mute = true;
    }

    void Update()
    {
        if (isBoatColliding && currentDurability > 0)
        {

        }
        else if (currentDurability <= 2)
        {
            lightAnimator.SetInteger("currentBoatDurability", 2);

            if (currentDurability <= 0 && boatBriefCase.isBriefcaseOpened == false)
            {
                Debug.Log("Boat is destroyed!");
                isBoatDestroyed = true;
                BoatCapsize();
                GameOver();
            }
            else if (currentDurability <= 0 && boatBriefCase.isBriefcaseOpened == true)
            {
                Debug.Log("Boat is destroyed!");
                isBoatDestroyed = true;
                BoatCapsize();
                BriefCaseOpened();
            }
        }

        if (boatBriefCase.isBriefcaseOpened)
        {
            currentDurability = 0; // Set durability to 0 when briefcase is opened
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Shark") && !isBoatColliding)
        {
            isBoatColliding = true;
            HandleDurability();
            BoatShake();
            boatCrashSound.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Shark") && isBoatColliding)
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Shark") && isBoatColliding)
        {
            isBoatColliding = false;
            //playerLife.isBoatColliding = false; // Reset the flag in PlayerLife
            Debug.Log("Boat stopped colliding with an obstacle.");
        }
    }

    public void GameOver()
    {
        BoatDestroyedUI.SetActive(true);
    }

    private void BriefCaseOpened()
    {
        BriefcaseOpenedUI.SetActive(true);
    }

    void HandleDurability()
    {
        if (isBoatColliding)
        {
            currentDurability--;
            Debug.Log("Boat hit an obstacle! Current Durability: " + currentDurability);
        }
        else
        {
            return;
        }
    }
    private void BoatShake()
    {
        StartCoroutine(TiltBoat(wobbleDuration, wobbleAngle)); 
    }

    private IEnumerator TiltBoat(float duration, float angle)
    {
        Quaternion originalRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration; // goes from 0 → 1
            float falloff = 1f - t;       // amplitude decreases from 1 → 0

            // Oscillate with sine wave, but reduce amplitude over time
            float z = Mathf.Sin(elapsed * 40f) * angle * falloff;

            transform.localRotation = originalRot * Quaternion.Euler(0, 0, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRot; // Reset rotation at the end
    }

    public void BoatCapsize()
    {
        if (boatMovement != null)
        {
            boatEngine.Stop(); // stop movement
            boatMovement.enabled = false; // stop controls
            marineHorn.enabled = false; // stop horn sound
            cameraControl.enabled = false; // stop camera follow
            waterBlocker.SetActive(false); // disable water blocker
            seaAmbience.mute = true;
            underwaterAmbience.mute = false;
            krakenFollow.enabled = false;
            //krakenSlapCollider.SetActive(false);

        }

        if (boatBuoyancy != null)
        {
            StartCoroutine(ReduceBuoyancyOverTime());
        }
        

        Debug.Log("Boat has capsized and is no longer operational.");
    }

    IEnumerator ReduceBuoyancyOverTime()
    {
        float startStrength = boatBuoyancy.buoyancyStrength;
        float elapsed = 0f;

        while (elapsed < sinkingDuration)
        {
            elapsed += Time.deltaTime;

            // t goes from 0 → 1 across sinkingDuration
            float t = elapsed / sinkingDuration;

            // Buoyancy weakens gradually
            boatBuoyancy.buoyancyStrength = Mathf.Lerp(startStrength, 0f, t);

            yield return null;
        }
        EOTCollider.enabled = false; 
        boatBuoyancy.buoyancyStrength = 0f; // fully sunk
    }
}
