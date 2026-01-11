using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public float fadeDuration = 2f;

    private TextMeshProUGUI text;
    private float lifetime;
    [Header("Popup")]
    public GameObject popupPrefab;
    public Transform popupParent; // ui canvas that we need to assign, because this is on a prefab 
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        popupParent = GetComponentInParent<Canvas>().transform;
    }

    private void Update()
    {
        // Fade out
        lifetime += Time.deltaTime;
        float alpha = 1 - (lifetime / fadeDuration);
        text.alpha = alpha;

        // Remove after fading out
        if (alpha <= 0) Destroy(gameObject);
    }
}

