using UnityEngine;

public class MapNavigation : MonoBehaviour
{
    [Header("Referensi")]
    public Transform ship;         // kapal
    public Transform mapQuad;      // quad peta
    public Transform shipMarker;   // ikon kapal di peta

    [Header("Ukuran Dunia (world space)")]
    public float worldMinX = -500;
    public float worldMaxX = 500;
    public float worldMinZ = -500;
    public float worldMaxZ = 500;

    void Update()
    {
        if (ship == null || mapQuad == null || shipMarker == null) return;

        // Normalisasi posisi kapal (0 - 1)
        float normX = Mathf.InverseLerp(worldMinX, worldMaxX, ship.position.x);
        float normZ = Mathf.InverseLerp(worldMinZ, worldMaxZ, ship.position.z);

        // Hitung ukuran peta dari bounds renderer, bukan localScale saja
        Renderer rend = mapQuad.GetComponent<Renderer>();
        float mapWidth = rend.bounds.size.x;
        float mapHeight = rend.bounds.size.z;

        // Posisi marker relatif terhadap peta (digeser dari center)
        Vector3 localPos = new Vector3(
            (normX - 0.5f) * mapWidth,
            0.01f, // biar ngambang sedikit di atas quad
            (normZ - 0.5f) * mapHeight
        );

        // Apply ke marker, dalam local space map
        shipMarker.localPosition = localPos;
    }
}
