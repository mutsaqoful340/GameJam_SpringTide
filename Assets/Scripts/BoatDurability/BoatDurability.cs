using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BoatDurability : MonoBehaviour
{
    [Header("Boat Collider")]
    public Collider boatCollider;

    [Header("Durability Settings")]
    public int maxDurability = 5;
    public int currentDurability;

    [Header("Light Animator")]
    public Animator lightAnimator;

    [Header("Boat Sinking Settings")]
    public float sinkingDuration = 3f; // Duration of the sinking effect

    private bool isBoatColliding = false;

    [HideInInspector] public bool isBoatDestroyed = false;

    private BoatBuoyancy boatBuoyancy;
    private BoatMovement boatMovement;

    void Start()
    {
        isBoatColliding = false;
        isBoatDestroyed = false;
        currentDurability = maxDurability;

        boatBuoyancy = GetComponent<BoatBuoyancy>();
        boatMovement = GetComponent<BoatMovement>();

        if (boatCollider == null)
        {
            Debug.LogError("Boat Collider is not assigned in the Inspector.");
        }

        if (lightAnimator == null)
        {
            Debug.LogError("Light Animator is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (isBoatColliding && currentDurability > 0)
        {
            HandleDurability();
        }
        else if (currentDurability <= 2)
        {
            lightAnimator.SetInteger("currentBoatDurability", 2);

            if (currentDurability <= 0)
            {
                Debug.Log("Boat is destroyed!");
                isBoatDestroyed = true;
                BoatCapsize();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !isBoatColliding)
        {
            isBoatColliding = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") && isBoatColliding)
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") && isBoatColliding)
        {
            isBoatColliding = false;
            //playerLife.isBoatColliding = false; // Reset the flag in PlayerLife
            Debug.Log("Boat stopped colliding with an obstacle.");
        }
    }
    void HandleDurability()
    {
        currentDurability--;
        isBoatColliding = false; // Reset to prevent multiple hits in one collision
        Debug.Log("Boat hit an obstacle! Current Durability: " + currentDurability);
    }
    
    void BoatCapsize()
    {
        if (boatMovement != null)
        {
            boatMovement.enabled = false; // stop controls
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

        boatBuoyancy.buoyancyStrength = 0f; // fully sunk
    }

}
