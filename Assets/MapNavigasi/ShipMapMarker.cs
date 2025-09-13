using UnityEngine;

public class ShipMapMarker : MonoBehaviour
{
    public RectTransform mapPanel;   // UI Panel peta
    public RectTransform shipIcon;   // Ikon kapal
    public Transform ship;           // Objek kapal di scene
    public Terrain terrain;          // Terrain utama

    [Header("Manual Adjustments")]
    public Vector2 offset;           // Offset biar bisa geser ikon
    public Vector2 scale = Vector2.one; // Skala biar bisa zoom / stretch

    private Vector2 worldMin;
    private Vector2 worldMax;
    private Vector2 mapSize;

    void Start()
    {
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        worldMin = new Vector2(terrainPos.x, terrainPos.z);
        worldMax = new Vector2(terrainPos.x + terrainSize.x, terrainPos.z + terrainSize.z);

        // Ambil ukuran panel dengan cara yang benar
        mapSize = new Vector2(mapPanel.rect.width, mapPanel.rect.height);

        Debug.Log($"[MAP INIT] mapSize: {mapSize}, worldMin: {worldMin}, worldMax: {worldMax}");
    }


    void Update()
    {
        Vector3 pos = ship.position;

        // Normalisasi world position jadi 0..1
        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, pos.x);
        float normZ = Mathf.InverseLerp(worldMin.y, worldMax.y, pos.z);

        // Konversi ke posisi UI
        float uiX = normX * mapSize.x * scale.x + offset.x;
        float uiY = normZ * mapSize.y * scale.y + offset.y;

        // Update posisi ikon di peta
        shipIcon.anchoredPosition = new Vector2(uiX, uiY);

        // Rotasi ikon sesuai arah kapal
        shipIcon.localEulerAngles = new Vector3(0, 0, -ship.eulerAngles.y);

        Debug.Log($"Ship world: {pos}, norm: ({normX}, {normZ}), ui: ({uiX}, {uiY})");
    }


}
