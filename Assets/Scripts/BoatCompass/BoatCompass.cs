using UnityEngine;

public class Compass : MonoBehaviour
{
    [Header("References")]
    public Transform player;         
    public Transform compassNeedle;  

    void Update()
    {
        if (player == null || compassNeedle == null) return;

        // Player's heading (Y rotation)
        float playerY = player.eulerAngles.y;

        // Rotate needle to always point north
        // Offset by +90 if your needle is modeled along X- instead of Z+
        compassNeedle.localEulerAngles = new Vector3(0, 90f, -playerY);
    }
}
