using UnityEngine;

public class HideInThirdPerson : MonoBehaviour
{
    [SerializeField] public CameraSwitcher cameraSwitcher;


    private void Update()
    {
        // Hide the RawImage component if it exists
        var rawImage = GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null)
        {
            rawImage.enabled = cameraSwitcher.IsFirstPerson;
        }
    }
}
