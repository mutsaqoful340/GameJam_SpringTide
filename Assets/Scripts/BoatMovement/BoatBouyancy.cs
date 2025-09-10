using UnityEngine;
using UnityEngine.Rendering.HighDefinition; // HDRP Water

[RequireComponent(typeof(Rigidbody))]
public class BoatBuoyancy : MonoBehaviour
{
    public Transform[] floatPoints;
    public float buoyancyStrength = 10f;
    public float waterDrag = 1f;
    public float waterAngularDrag = 0.5f;

    public WaterSurface waterSurface; // assign in Inspector

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        if (waterSurface == null || floatPoints.Length == 0) return;

        foreach (Transform point in floatPoints)
        {
            Vector3 samplePos = point.position;

            // Setup parameters
            WaterSearchParameters search = new WaterSearchParameters
            {
                startPosition = samplePos,
                targetPosition = samplePos,
                error = 0.01f,
                maxIterations = 8
            };

            WaterSearchResult result;

            // Query water surface
            if (waterSurface.FindWaterSurfaceHeight(search, out result))
            {
                float waterHeight = result.height;
                float depth = waterHeight - samplePos.y;

                if (depth > 0f)
                {
                    // Buoyancy force
                    Vector3 uplift = Vector3.up * buoyancyStrength * depth;
                    rb.AddForceAtPosition(uplift, samplePos, ForceMode.Force);

                    // Drag for stability
                    Vector3 velocity = rb.GetPointVelocity(samplePos);
                    rb.AddForceAtPosition(-velocity * waterDrag, samplePos, ForceMode.Force);
                }
            }
        }

        // Apply angular drag in water
        rb.AddTorque(-rb.angularVelocity * waterAngularDrag, ForceMode.Force);
    }
}
