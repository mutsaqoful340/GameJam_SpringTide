using UnityEngine;
using UnityEngine.Rendering.HighDefinition; // HDRP Water

[RequireComponent(typeof(Rigidbody))]
public class BoatBuoyancy : MonoBehaviour
{
    public Transform[] floatPoints;
    public float buoyancyStrength = 10f;
    public float maxDepthEffect = 2f;         // clamp per float point (prevents huge forces)
    public float waterDrag = 1f;
    public float waterAngularDrag = 0.5f;

    public WaterSurface waterSurface; // assign in Inspector

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
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

            if (waterSurface.FindWaterSurfaceHeight(search, out result))
            {
                float waterHeight = result.height;
                float depth = waterHeight - samplePos.y;

                if (depth > 0f)
                {
                    // Clamp depth effect so buoyancy won't explode
                    float clampedDepth = Mathf.Min(depth, maxDepthEffect);

                    // Apply buoyancy, damped if already moving upward fast
                    float verticalVel = rb.GetPointVelocity(samplePos).y;
                    float damping = Mathf.Clamp01(1f - verticalVel * 0.25f);

                    Vector3 uplift = Vector3.up * buoyancyStrength * clampedDepth * damping;
                    rb.AddForceAtPosition(uplift, samplePos, ForceMode.Force);

                    // Add drag for stability
                    Vector3 velocity = rb.GetPointVelocity(samplePos);
                    rb.AddForceAtPosition(-velocity * waterDrag, samplePos, ForceMode.Force);
                }
            }
        }

        // Apply angular drag in water
        rb.AddTorque(-rb.angularVelocity * waterAngularDrag, ForceMode.Force);
    }
}
