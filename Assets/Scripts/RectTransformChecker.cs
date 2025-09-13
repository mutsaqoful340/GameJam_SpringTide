using UnityEngine;

public class RectTransformChecker : MonoBehaviour
{
    void Start()
    {
        RectTransform[] rects = FindObjectsOfType<RectTransform>();
        foreach (var rect in rects)
        {
            Vector3 pos = rect.position;
            if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) ||
                Mathf.Abs(pos.x) > 10000 || Mathf.Abs(pos.y) > 10000 || Mathf.Abs(pos.z) > 10000)
            {
                Debug.LogWarning($"Suspicious RectTransform: {rect.name} at {pos}", rect);
            }
        }
    }
}
