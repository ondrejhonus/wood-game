using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 10;
    public GameObject[] slots; // Items stored
    public Transform handPosition; // Where items appear in hand
    private int selectedSlot = -1; // -1 = empty hand
    public LayerMask pickupLayer;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private float firstPersonDistance = 5f;
    [SerializeField] private float thirdPersonDistance = 30f; // needs to be further away, coz the camera is behind the player
    private bool isFP;

    [Header("UI References")]
    public Image[] slotIcons;
    public Sprite defaultIcon; // if no icon is set for an item, not an empty slot
    public RectTransform selectionHighlight; // a glowing border maybe



    void Start()
    {
        slots = new GameObject[inventorySize];
        SelectSlot(-1);
        UpdateInventoryUI();
    }
    // call everytime inventory changes
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (slots[i] != null)
            {
                string itemName = slots[i].name;

                // Remove "(Clone)" from name if its a cloend prefab
                if (itemName.Contains("(Clone)"))
                    itemName = itemName.Replace("(Clone)", "").Trim();

                // load from Assets/Resources/icons/<itemName>
                Sprite loadedSprite = Resources.Load<Sprite>("icons/" + itemName);

                if (loadedSprite != null)
                {
                    slotIcons[i].sprite = loadedSprite;
                    slotIcons[i].enabled = true; // Show the image
                }
                else
                {
                    // If no specific icon found, use default icon
                    slotIcons[i].sprite = defaultIcon;
                    if (defaultIcon == null) slotIcons[i].enabled = false;
                    else slotIcons[i].enabled = true;
                }
            }
            else
            {
                // Slot is empty, dont show any icon
                slotIcons[i].enabled = false;
            }
        }
        if (GetSelectedItem() == null)
        {
            selectionHighlight.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandlePickup();
        HandleSlotSwitch();

        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to drop
        {
            DropSelectedItem();
        }
        isFP = cameraSwitcher.IsFirstPerson;
    }

    public GameObject GetSelectedItem()
    {
        if (selectedSlot < 0 || slots[selectedSlot] == null)
            return null;
        return slots[selectedSlot];
    }

    void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            Camera currentCam = Camera.main;
            float grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;
            Ray ray = isFP ? currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)) : currentCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, pickupLayer))
            {
                // Only pick up if it's a pickupable item or axe
                if (hit.collider.CompareTag("Pickup") || hit.collider.CompareTag("Axe"))
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
    public bool AddItem(GameObject item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                UpdateInventoryUI();
                return true;
            }
        }
        Debug.Log("Inventory full!");
        return false;
    }

    void HandleSlotSwitch()
    {
        for (int i = 0; i < inventorySize-1; i++)
        {
            // Hotkeys 1-9
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SelectSlot(i);
            }
        }

        // slot 10 = 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectSlot(9);
        }
        // empty hand = Backspace
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SelectSlot(-1);
        }
        // Mouse scroll wheel
        else
        {
            // logic for mouse scroll wheel direction to switch slots
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                // Cycle to previous slot
                int nextSlot = selectedSlot + 1;
                if (nextSlot > inventorySize - 1)
                    nextSlot = 0;
                SelectSlot(nextSlot);
            }
            else if (scroll < 0f)
            {
                // Cycle to next slot
                int nextSlot = selectedSlot - 1;
                if (nextSlot < 0)
                    nextSlot = inventorySize - 1;
                SelectSlot(nextSlot);
            }
        }
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;

        if (selectedSlot >= 0 && selectedSlot < slotIcons.Length)
    {
        selectionHighlight.gameObject.SetActive(true);
        selectionHighlight.position = slotIcons[selectedSlot].transform.position;
    }
    else
    {
        selectionHighlight.gameObject.SetActive(false);
    }

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
        UpdateInventoryUI();
    }

    public GameObject[] GetItemsInInventory()
    {
        return slots;
    }

    public int GetItemCount()
    {
        int count = 0;
        foreach (GameObject item in slots)
        {
            if (item != null)
            {
                count++;
            }
        }
        return count;
    }

    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = null;
        }
        selectedSlot = -1; // Empty hand
        UpdateInventoryUI();
    }
}
