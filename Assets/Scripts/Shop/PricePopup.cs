using TMPro;
using UnityEngine;


public class PricePopup : MonoBehaviour
{
    public GameObject popupTextPrefab;
    public ShopItem shopItem;
    public PlayerStats playerStats;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private LayerMask PickUpLayerMask;
    [SerializeField] private float firstPersonDistance = 5f;
    [SerializeField] private float thirdPersonDistance = 16f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats != null)
        {
            Camera currentCam = cameraSwitcher.playerCamera;
            bool isFP = cameraSwitcher.IsFirstPerson;
            Ray ray = isFP ? currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)) : currentCam.ScreenPointToRay(Input.mousePosition);
            float grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;
            if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, PickUpLayerMask))
            {
                if (hit.collider != null && hit.collider.CompareTag("ShopItem"))
                {
                    PricePopup pricePopup = hit.collider.GetComponent<PricePopup>();
                    if (pricePopup != null)
                    {
                        playerStats.ShowPopupOneFrame("$" + shopItem.price.ToString(), shopItem.transform.position, Color.yellow);
                    }
                }
            }
        }

    }
}
