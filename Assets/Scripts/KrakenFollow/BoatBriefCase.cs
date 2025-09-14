using UnityEngine;

public class BoatBriefCase : MonoBehaviour
{
    public bool isBriefcaseOpened = false;
    private BoatDurability boatDurability;

    void Start()
    {
        boatDurability = GetComponent<BoatDurability>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Briefcase"))
            {
                isBriefcaseOpened = true;
                boatDurability.currentDurability = 0; // Set durability to 0 when briefcase is opened
                Debug.Log("Briefcase opened!");
            }
        }
    }
    
    
}