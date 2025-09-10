using UnityEngine;

public class FollowWater : MonoBehaviour
{
    public Transform player;  // Assign your player here
    public float waterHeight = 0f; // Fixed Y height for water

    void LateUpdate()
    {
        if (player == null) return;

        // Move water to player's X and Z, keep Y fixed
        transform.position = new Vector3(player.position.x, waterHeight, player.position.z);
    }
}