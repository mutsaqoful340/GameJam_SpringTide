using UnityEngine;
using TMPro;

public class BoatGridCoordinates : MonoBehaviour
{
    public Transform player;   // Your player transform
    public TextMeshProUGUI coordText;     // A UI Text to display coordinates
    public float cellSize = 10f;

    void Update()
    {
        // Get player's grid index
        int gridX = Mathf.FloorToInt(player.position.x / cellSize);
        int gridY = Mathf.FloorToInt(player.position.z / cellSize); // Use Z for forward axis

        // Convert X to letters (A=0, B=1, etc.)
        string column = ConvertToLetters(gridX);

        // Display
        coordText.text = $"{column}-{gridY}";
    }

    string ConvertToLetters(int number)
    {
        string result = "";
        number++; // So 0 = A, 1 = B, etc.

        while (number > 0)
        {
            int remainder = (number - 1) % 26;
            result = (char)(65 + remainder) + result; // 65 = 'A'
            number = (number - 1) / 26;
        }

        return result;
    }
}
