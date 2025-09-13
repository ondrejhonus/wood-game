using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 5;
    public GameObject[] slots; // Items stored
    public Transform handPosition; // Where items appear in hand

    private int selectedSlot = -1; // -1 = empty hand

    void Start()
    {
        slots = new GameObject[inventorySize];
    }

    void Update()
    {
        HandlePickup();
        HandleSlotSwitch();
    }

    void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.CompareTag("Pickup"))
                {
                    Debug.Log("Picking up: " + hit.collider.name);
                    GameObject item = hit.collider.gameObject;
                    if (AddItem(item))
                    {
                        item.transform.parent = null; // Unparent from world
                        item.SetActive(false);        // Hide until selected
                        item.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
                    }
                }
            }
        }
    }

    // Add item to first empty slot
    bool AddItem(GameObject item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                return true;
            }
        }
        Debug.Log("Inventory full!");
        return false;
    }

    void HandleSlotSwitch()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // Hotkeys 1-5
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SelectSlot(i);
            }
        }

        // Empty hand = 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectSlot(-1);
        }
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                if (i == selectedSlot)
                {
                    slots[i].SetActive(true);
                    // Move to hand
                    slots[i].transform.position = handPosition.position;
                    slots[i].transform.rotation = handPosition.rotation;
                    slots[i].transform.parent = handPosition;
                }
                else
                {
                    slots[i].SetActive(false);
                    slots[i].transform.parent = null; // Unparent when not in hand
                }
            }
        }
    }
}
