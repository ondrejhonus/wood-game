using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 5;
    public GameObject[] slots; // Items stored
    public Transform handPosition; // Where items appear in hand

    private int selectedSlot = -1; // -1 = empty hand

    public LayerMask pickupLayer;

    void Start()
    {
        slots = new GameObject[inventorySize];
    }

    void Update()
    {
        HandlePickup();
        HandleSlotSwitch();

        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to drop
        {
            DropSelectedItem();
        }

    }

    void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, pickupLayer))
            {
                if (hit.collider.CompareTag("Pickup"))
                {
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
                // Parent to hand so it moves with it
                slots[i].transform.parent = handPosition;
                // Fine-tune the grip position and rotation
                slots[i].transform.localPosition = new Vector3(-0.28f, 0.63f, -0.15f); // Adjust these values as needed
                slots[i].transform.localRotation = Quaternion.Euler(-128.5f, 82.11f, -96.8f); // Adjust these angles as needed
            }
            else
            {
                slots[i].SetActive(false);
                slots[i].transform.parent = null; // Unparent when not in hand
            }
            }
        }
    }

    public void DropSelectedItem()
    {
        if (selectedSlot < 0 || slots[selectedSlot] == null) return; // Nothing to drop

        GameObject item = slots[selectedSlot];

        // Drop from hand
        item.transform.parent = null;
        item.SetActive(true);

        // Enable physics
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse); // Throw it a bit
        }

        // Remove from inventory
        slots[selectedSlot] = null;
        selectedSlot = -1; // Empty hand
    }

}
